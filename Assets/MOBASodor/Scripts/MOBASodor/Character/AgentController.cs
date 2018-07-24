using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AgentController : MonoBehaviour {

    protected enum E_Action {IDLE, BACK_TO_TOWER, GOTO_CHECKPOINT, ACTION_COUNT}

    public GameManager.Team Team;
    public float MaxPower = 1000;
    public float RangeToFindEnemy = 10f;
    public GameObject TeamTower;

    protected GameObject m_enemyTower = null;
    protected GameObject m_teamTower = null;
    protected Vector3 t_goal;
    protected float CurrentPower;
    protected bool b_isPowerDraining;
    protected NavMeshAgent agent;
    protected GameObject ChaseObject;
    protected E_Action currentAction;
    protected GameManager manager;

    SelectorNode n_root;

    // Running from enemy node 
    LeafNode n_GetClosestEnemy;
    LeafNode n_IsPlayerPowerHigerThanEnemy;
    LeafNode n_MeetWithDestination;
    LeafNode n_BackToTowerAction;

    InverterNode n_IsPlayerPowerLowerThanEnemy;

    SelectorNode n_BackToTower;

    SequenceNode n_RunningFromEnemy;
    // end running

    LeafNode n_IsAlreadyHaveDestination;
    LeafNode n_GetNearestCheckPoint;
    LeafNode n_MeetWithDestinationCheckPoint;
    LeafNode n_GotoCheckPointAction;

    SelectorNode n_SetDestinationCheckpoint;
    SelectorNode n_GotoCheckPoint;

    SequenceNode n_GotoNearestCheckpoint;

    //Back to tower If Power is critical
    LeafNode n_CheckIfPowerisCritical;

    SequenceNode n_BackToTowerifPowerIsCritical;
    //end back

	bool b_isProcessing;
    TowerManagement tm_teamTower;
    GameObject nearestEnemy;
    GameObject m_destinationCheckpoint;
    Communicator m_communicator;

    // Use this for initialization
    protected void Start()
    {
        CurrentPower = MaxPower;
        b_isPowerDraining = false;
        currentAction = E_Action.IDLE;
        m_destinationCheckpoint = null;
        manager = GameManager.GetInstance();

        //register Events
        GameManager.GetInstance().PauseEvent.AddListener(OnPause);

        //define agent
        agent = GetComponent<NavMeshAgent>();

        //get enemy tower object
        foreach (GameObject tower in GameManager.GetInstance().Towers)
        {
            TowerManagement man = tower.GetComponent<TowerManagement>();
            if (man != null)
            {
                if (man.Team != Team)
                    m_enemyTower = tower;
                else
                    m_teamTower = tower;
            }
        }
        //define node AI
        DefineNode();
        m_communicator = m_teamTower.GetComponent<Communicator>();
    }

    protected void FixedUpdate()
    {
        if (b_isPowerDraining)
            CurrentPower -= Time.deltaTime;

        Action();
        switch(currentAction)
        {
            case E_Action.BACK_TO_TOWER:
                agent.destination = m_teamTower.transform.position;
                break;
            case E_Action.GOTO_CHECKPOINT:
                if(m_destinationCheckpoint != null)
                    agent.destination = m_destinationCheckpoint.transform.position;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangeToFindEnemy);
    }

    public void OnPause()
    {
        agent.isStopped = true;
    }

    public void RefreshPower()
    {
        CurrentPower = MaxPower;
        b_isPowerDraining = false;
    }

    public void DrainPower()
    {
        b_isPowerDraining = true;
    }

	protected void DefineNode()
	{
        // RunningFromEnemy
        n_GetClosestEnemy = new LeafNode(GetClosestEnemy_node);
        n_IsPlayerPowerHigerThanEnemy = new LeafNode(IsPlayerPowerHigerThanEnemy);
        n_MeetWithDestination = new LeafNode(MeetWithDestination);
        n_BackToTowerAction = new LeafNode(BackToTowerAction);

        n_IsPlayerPowerLowerThanEnemy = new InverterNode(n_IsPlayerPowerHigerThanEnemy);

        n_BackToTower = new SelectorNode(new List<Node>
        {
            n_MeetWithDestination,
            n_BackToTowerAction
        });

        n_RunningFromEnemy = new SequenceNode(new List<Node>
        {
            n_GetClosestEnemy,
            n_IsPlayerPowerLowerThanEnemy,
            n_BackToTower
        });
        //end running

        // goto nearest checkpoint

        n_IsAlreadyHaveDestination = new LeafNode(IsAlreadyHaveDestination);
        n_GetNearestCheckPoint = new LeafNode(GetNearestCheckPoint);
        n_MeetWithDestinationCheckPoint = new LeafNode(MeetWithDestination);
        n_GotoCheckPointAction = new LeafNode(GotoCheckPointAction);

        n_SetDestinationCheckpoint = new SelectorNode(new List<Node>
        {
            n_IsAlreadyHaveDestination, 
            n_GetNearestCheckPoint
        });
        n_GotoCheckPoint = new SelectorNode(new List<Node>
        {
            n_MeetWithDestination,
            n_GotoCheckPointAction
        });

        n_GotoNearestCheckpoint = new SequenceNode(new List<Node>
        {
            n_SetDestinationCheckpoint, 
            n_GotoCheckPoint
        });

        //end nearest

        //Back to tower if power is critical
        n_CheckIfPowerisCritical = new LeafNode(CheckIfPowerIsCritical);

        n_BackToTowerifPowerIsCritical = new SequenceNode(new List<Node>()
        {
            n_CheckIfPowerisCritical,
            n_BackToTower
        });
        //end back

        n_root = new SelectorNode(new List<Node>
        {
            n_BackToTowerifPowerIsCritical,
            n_RunningFromEnemy,
            n_GotoNearestCheckpoint
        });
    }

    protected void Action()
    {
        if (!b_isProcessing)
        {
            b_isProcessing = true;
            StartCoroutine("EvaluateBehaviour");
        }
    }

    //Chase Enemy -> Selector/ Sequence
    //Running From Enemy -> Inverter
    Node.NodeState IsPlayerPowerHigerThanEnemy()
    {
        if (nearestEnemy != null && nearestEnemy.GetComponent<AgentController>() != null)
        {
            if (nearestEnemy.GetComponent<AgentController>().Power <= Power)
                return Node.NodeState.SUCCESS;
            else
                return Node.NodeState.FAILED;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState GetClosestEnemy_node()
    {
        if (GetClosestEnemy())
        {
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState MeetWithDestination()
    {
        Debug.Log(" MeetWithDestination()");
        switch (currentAction)
        {
            case E_Action.GOTO_CHECKPOINT:
                if(m_destinationCheckpoint != null)
                {
                    Debug.Log(" MeetWithDestination()");
                    float distanceC = Vector3.Distance(transform.position, m_destinationCheckpoint.transform.position);
                    if (distanceC <= agent.stoppingDistance)
                    {
                        m_destinationCheckpoint = null;
                        return Node.NodeState.SUCCESS;
                    }
                }
                break;
            case E_Action.BACK_TO_TOWER:
                float distance = Vector3.Distance(transform.position, m_teamTower.transform.position);
                if(distance <= agent.stoppingDistance)
                {
                    return Node.NodeState.SUCCESS;
                }
                break;
        }

        return Node.NodeState.FAILED;
    }

    Node.NodeState BackToTowerAction()
    {
        Debug.Log("Back to tower action");
        currentAction = E_Action.BACK_TO_TOWER;
        m_communicator.DeleteMessage(gameObject);
        return Node.NodeState.RUNNING;
    }
    /////


    Node.NodeState IsAlreadyHaveDestination()
    {
        Debug.Log("IsAlreadyHaveDestination()");
        if(m_destinationCheckpoint != null)
        {
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState GetNearestCheckPoint()
    {
       
        List<Communicator.Message> checkpointMassage = m_communicator.Find(Communicator.Message.CommunicationType.GOTO_CHECKPOINT);

        GameObject nearestCheckpoint = null;
        float nearestDistance = 0;

        foreach(GameObject ch in manager.CheckPoints)
        {
            CheckPointEngine check = ch.GetComponent<CheckPointEngine>();
            if (check != null && check.TeamOwner != Team)
            {
                Communicator.Message ch_obj = checkpointMassage.Find(x => x.target == ch);
                if (checkpointMassage.IndexOf(ch_obj) < 0)
                {
                    float distance = Vector3.Distance(transform.position, ch.transform.position);
                    if (nearestDistance == 0 || nearestDistance > distance)
                    {
                        nearestDistance = distance;
                        nearestCheckpoint = ch;
                    }
                }
            }
        }

        if(nearestCheckpoint != null)
        {
            
            m_destinationCheckpoint = nearestCheckpoint;
            m_communicator.AddMessage(gameObject, nearestCheckpoint, Communicator.Message.CommunicationType.GOTO_CHECKPOINT);
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState GotoCheckPointAction()
    {
        Debug.Log("Goto checkpoint");
        currentAction = E_Action.GOTO_CHECKPOINT;
        return Node.NodeState.RUNNING;
    }

    Node.NodeState RandomValue()
    {
        float randomValue = Random.value;
        if (randomValue > 0.7)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    Node.NodeState DeleteAllAction()
    {
        currentAction = E_Action.IDLE;
        return Node.NodeState.SUCCESS;
    }

    //Back to tower if power is critical
    Node.NodeState CheckIfPowerIsCritical()
    {
        if(Power < 100)
        {
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

	bool GetClosestEnemy()
	{
		Collider[] objects = Physics.OverlapSphere(transform.position, RangeToFindEnemy);

		if (objects.Length > 0)
		{
            
            GameObject closestEnemy = null;
            float closestDistance = 0;
			foreach(Collider col in objects)
            {
                if (col.tag == "Player")
                {
                    
                    AgentController control = col.GetComponent<AgentController>();
                    if (control == null)
                        continue;
                    if (control.Team == Team)
                        continue;
                    Debug.Log("musuh ni");
                    float dist = Vector3.Distance(col.gameObject.transform.position, transform.position);
                    if (closestDistance < dist)
                    {
                        closestEnemy = col.gameObject;
                        closestDistance = dist;
                    }
                }
            }
			nearestEnemy = closestEnemy;
            if (nearestEnemy != null)
                return true;
            else
                return false;
		}
		return false;
	}

	


	IEnumerator EvaluateBehaviour()
    {
        yield return new WaitForSeconds(0);

        n_root.Evaluate();

      
        b_isProcessing = false;
    }

    public float Power
    {
        get { return CurrentPower; }
    }
}
