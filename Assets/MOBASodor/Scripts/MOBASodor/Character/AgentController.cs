using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AgentController : MonoBehaviour {

    public GameManager.Team Team;
    public float MaxPower = 1000;

    protected GameObject m_enemyTower = null;
    protected GameObject m_teamTower = null;

    protected Vector3 t_goal;
    
    protected float CurrentPower;
    protected bool b_isPowerDraining;

    protected NavMeshAgent agent;
    protected GameObject ChaseObject;

    SelectorNode n_root;
    SequenceNode n_backToTower;
    SequenceNode n_lureEnemy;
    SequenceNode n_chaseEnemy;
    InverterNode n_runningFromEnemy;
    InverterNode n_NoEnemyNearTower;
    LeafNode n_isTowerEmpty;
    LeafNode n_isEnemyNearTower;
    LeafNode n_isPlayerClosestWithTower;
    LeafNode n_isPlayerPowerHigerThanEnemy;
    LeafNode n_RandomValue;
    bool b_isProcessing;
    TowerManagement tm_teamTower;

    GameObject nearestEnemy;


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

    public float Power
    {
        get { return CurrentPower; }
    }


    // Use this for initialization
    protected void Start()
    {
        CurrentPower = MaxPower;
        b_isPowerDraining = false;

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
        //set agent destination
        //if (m_enemyTower != null)
        //{
        //    goal = m_enemyTower.transform.position;
        //    t_goal = goal;
        //    agent.destination = goal;
        //}
    }

    protected void FixedUpdate()
    {
        if (b_isPowerDraining)
            CurrentPower -= Time.deltaTime;

        Action();
        if (ChaseObject != null && t_goal != ChaseObject.transform.position)
        {
            agent.destination = ChaseObject.transform.position;
            t_goal = ChaseObject.transform.position;
        }


        //if (goal != t_goal)
        //{
        //    agent.destination = goal;
        //    t_goal = goal;
        //}
    }

    protected void DefineNode()
    {
        //leaf node
        n_isTowerEmpty = new LeafNode(IsTowerEmpty);
        n_isEnemyNearTower = new LeafNode(IsEnemyNearTower);
        n_isPlayerClosestWithTower = new LeafNode(IsPlayerClosesWithTower);
        n_isPlayerPowerHigerThanEnemy = new LeafNode(IsPlayerPowerHigerThanEnemy);
        n_RandomValue = new LeafNode(RandomValue);

        //sequence
        n_backToTower = new SequenceNode(new List<Node>
        {
            n_isTowerEmpty,
            n_isEnemyNearTower,
            n_isPlayerClosestWithTower
        });

        //n_lureEnemy = new SequenceNode(new List<Node>
        //{
        //    n_RandomValue,
        //    n_NoEnemyNearTower
        //});

        n_chaseEnemy = new SequenceNode(new List<Node>
        {
            n_RandomValue,
            n_isPlayerPowerHigerThanEnemy
        });

        //inverter
        n_runningFromEnemy = new InverterNode(n_isPlayerPowerHigerThanEnemy);
        //n_NoEnemyNearTower = new InverterNode(n_isEnemyNearTower);

        b_isProcessing = false;
        tm_teamTower = m_teamTower.GetComponent<TowerManagement>();

        //root
        n_root = new SelectorNode(new List<Node>
        {
            n_backToTower,
            n_chaseEnemy,
            n_runningFromEnemy,
            //n_lureEnemy
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

    // Back to tower -> Sequence
    Node.NodeState IsTowerEmpty()
    {
        if (tm_teamTower.ListPlayerInside.Count == 0)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    // Lure enemy -> Inverter
    // Back to Tower -> Sequence
    Node.NodeState IsEnemyNearTower()
    {
        nearestEnemy = tm_teamTower.GetNearestEnemy();
        if (nearestEnemy != null)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    // Back to Tower -> Sequence
    Node.NodeState IsPlayerClosesWithTower()
    {
        if (tm_teamTower.GetPlayerNearestWithTower() == gameObject)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    //Chase Enemy -> Selector/ Sequence
    //Running From Enemy -> Inverter
    Node.NodeState IsPlayerPowerHigerThanEnemy()
    {
        nearestEnemy = tm_teamTower.GetNearestEnemy();
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

    Node.NodeState RandomValue()
    {
        float randomValue = Random.value;
        if (randomValue > 0.7)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }


    IEnumerator EvaluateBehaviour()
    {
        yield return new WaitForSeconds(0);

        n_root.Evaluate();

        if (n_backToTower.state == Node.NodeState.SUCCESS)
        {
            Debug.Log("Back to tower");
            ChaseObject = tm_teamTower.TeamSpawn.gameObject;
        }
        else if (n_chaseEnemy.state == Node.NodeState.SUCCESS)
        {
            Debug.Log("Chase enemy");
            ChaseObject = nearestEnemy;
        }
        else if (n_runningFromEnemy.state == Node.NodeState.SUCCESS)
        {
            Debug.Log("Running from enemy");
            ChaseObject = tm_teamTower.TeamSpawn.gameObject;
        }
        //else if (n_lureEnemy.state == Node.NodeState.SUCCESS)
        //{
        //    Debug.Log("Lure enemy");
        //    ChaseObject = m_enemyTower;
        //}
        b_isProcessing = false;
    }

}
