using System.Collections.Generic;
using UnityEngine;

public class Commander : Actor
{
    public Gun.Assembly gun;

    private SelectorNode _rootNode;
    private LeafNode _run;
    private LeafNode _celebrate;
    private LeafNode _alreadyHasCover;
    private LeafNode _pickNearestCover;
    private SequenceNode _goToNearestCover;
    private SelectorNode _checkCoverPicked;

    private Locomotion.BotGait _gait;

    private Obstacle.BoxColliderDescriptor _selectedCover;
    private Vector3 _coverSpot;
    private Vector3 _standingSpot;

    private bool _isInCover;
    private bool _isFiring;
    private List<Commander> _enemies;

    private Vector3 cornerA;
    private Vector3 cornerB;
    private bool _targetingCoverOrStandingSpot; //true = cover spot; false = standing spot

    private void Awake()
    {
        transformRef = GetComponent<Transform>();
        _gait = GetComponent<Locomotion.BotGait>();
        _enemies = new List<Commander>();
        CreateBehaviourTree();
        SetCharacteristic(1,1);
    }

    private void Start ()
    {
        state = State.ALIVE;
        //Reset suppression, there's no point keeping it suppressed upon re-activation
        suppression = 0;

        GameObject[] botList = GameObject.FindGameObjectsWithTag("Bot");
        for(int i = 0; i < botList.Length; i++)
        {
            Commander bot = botList[i].GetComponent<Commander>();
            if (allegiance != bot.allegiance)
            {
                _enemies.Add(bot);
            }
        }

        PrepareGun();
	}
	
	private void Update ()
    {
        if (isCharacteristicSet)
            _rootNode.Evaluate();
        //else
        //    Debug.LogError("ERROR!! Characteristics for " + gameObject.name + " haven't been set");
	}

    private void CreateBehaviourTree()
    {
        _run = new LeafNode(Run);
        _pickNearestCover = new LeafNode(RandomlyTryToPickNewCover);
        _alreadyHasCover = new LeafNode(AlreadyHasCover);
        _checkCoverPicked = new SelectorNode(new List<Node> { _pickNearestCover });
        _goToNearestCover = new SequenceNode(new List<Node> { _checkCoverPicked, _run});
        _rootNode = new SelectorNode(new List<Node> { _goToNearestCover});
    }

    public void SetCharacteristic(int inBravery, int inAccuracy)
    {
        bravery = inBravery;
        accuracy = inAccuracy;
        isCharacteristicSet = true;
    }

    private void ResetCharacteristic()
    {
        bravery = 0;
        accuracy = 0;
        isCharacteristicSet = false;
    }

    #region Action Nodes
    Node.NodeState Run()
    {
        Locomotion.BotGait.LocomotionState locoState = _gait.CheckLocomotionState();
        if(locoState == Locomotion.BotGait.LocomotionState.MOVING)
        {
            return Node.NodeState.RUNNING;
        }
        else if(locoState == Locomotion.BotGait.LocomotionState.ARRIVED)
        {
            if (Vector3.Distance(transformRef.position, _coverSpot) <= 0.05f)
                _isInCover = true;
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState AlreadyHasCover()
    {
        if (_selectedCover != null)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    Node.NodeState PickNearestCover()
    {
        LeaveLastCover();
        FindNearestCover();
        PickCoverSpot();
        _gait.SetBotDestination(new Vector2(_coverSpot.x, _coverSpot.z));
        return Node.NodeState.SUCCESS;
    }

    Node.NodeState HasStandingSpot()
    {
        if (_standingSpot != Vector3.zero)
            return Node.NodeState.SUCCESS;
        else
            return Node.NodeState.FAILED;
    }

    Node.NodeState PickStandingSpot()
    {
        _gait.SetBotDestination(_standingSpot);
        return Node.NodeState.FAILED;
    }

    Node.NodeState AimAndFire()
    {
        return Node.NodeState.FAILED;
    }

    Node.NodeState TakeCover()
    {
        _gait.SetBotDestination(_coverSpot);
        Locomotion.BotGait.LocomotionState locoState = _gait.CheckLocomotionState();
        if (locoState == Locomotion.BotGait.LocomotionState.MOVING)
        {
            return Node.NodeState.RUNNING;
        }
        else if (locoState == Locomotion.BotGait.LocomotionState.ARRIVED)
        {
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState RandomlyTryToPickNewCover()
    {
        LeaveLastCover();
        FindRandomCover();
        PickCoverSpot();
        _gait.SetBotDestination(new Vector2(_coverSpot.x, _coverSpot.z));
        return Node.NodeState.SUCCESS;
    }
    #endregion

    #region Test Nodes
    bool IsNotInCover()
    {
        return !_isInCover;
    }

    bool IsEnemyPresent()
    {
        _enemies.RemoveAll(en => en.state == State.DEAD);
        return _enemies.Count > 0;
    }

    bool IsNotFiring()
    {
        return !_isFiring;
    }

    bool IsNotSuppressed()
    {
        return suppression > 0;
    }

    bool IsMagazineNotEmpty()
    {
        return false;
    }
    #endregion

    #region Aux Methods
    private void LeaveLastCover()
    {
        if (_selectedCover != null)
            _selectedCover.isOccupied = false;
        _selectedCover = null;
    }

    private void FindNearestCover()
    {
        float nearestDist = float.MaxValue;
        foreach (Obstacle.BoxColliderDescriptor cover in Game.Instance.covers)
        {
            float dist = Vector3.Distance(cover.transformRef.position, transformRef.position);
            if (dist < nearestDist && cover.isOccupied == false)
            {
                nearestDist = dist;
                _selectedCover = cover;
            }
        }
        _selectedCover.isOccupied = true;
    }

    private void FindRandomCover()
    {
        int trial = 0;
        do
        {
            int randomIdx = Random.Range(0, Game.Instance.covers.Count);
            if (!Game.Instance.covers[randomIdx].isOccupied)
                _selectedCover = Game.Instance.covers[randomIdx];
        } while (_selectedCover == null || ++trial < Game.Instance.covers.Count);

        if (_selectedCover != null)
            _selectedCover.isOccupied = true;
    }

    private void PickCoverSpot()
    {
        Commander nearestEnemy = FindNearestEnemy();
        Vector3 enemyDir = (nearestEnemy.transformRef.position - transformRef.position).normalized;
        Vector3 tgtFace = FindFarthestFace(nearestEnemy, _selectedCover);
        _coverSpot = tgtFace + -enemyDir;
    }

    private Commander FindNearestEnemy()
    {
        float nearestDist = float.MaxValue;
        Commander nearestEnemy = null;
        for(int i = 0; i <  _enemies.Count; i++)
        {
            float dist = Vector3.Distance(_enemies[i].transformRef.position, transformRef.position);
            if(dist < nearestDist)
            {
                nearestDist = dist;
                nearestEnemy = _enemies[i];
            }
        }
        return nearestEnemy;
    }

    private Vector3 FindFarthestFace(Commander enemy, Obstacle.BoxColliderDescriptor obstacle)
    {
        Vector3 corner1 = Vector3.zero;
        Vector3 corner2 = Vector3.zero;
        float farthestDist1 = 0;
        float farthestDist2 = 0;
        for(int i = 0; i < obstacle.xzBoundaries.Count; i++)
        {
            float dist = Vector3.Distance(enemy.transformRef.position, new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y));

            if (dist > farthestDist1)
            {
                farthestDist2 = farthestDist1;
                farthestDist1 = dist;
                corner2 = corner1;
                corner1 = new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y);
            }

            else if (dist > farthestDist2)
            {
                farthestDist2 = dist;
                corner2 = new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y);
            }

        }
        Vector3 faceMidPoint = corner2 + (corner1 - corner2) / 2;
        cornerA = corner1;
        cornerB = corner2;
        return faceMidPoint;
    }

    private void PrepareGun()
    {

    }
    #endregion

    #region Properties
    public int bravery { get; set; }
    public int suppression { get; private set; }
    public int accuracy { get; set; }
    public bool isCharacteristicSet { get; private set; }
    public Transform transformRef { get; private set; }
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if(_coverSpot != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transformRef.position, _coverSpot);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_coverSpot, .5f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_selectedCover.transformRef.position, 5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cornerA, .5f);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(cornerB, .5f);

        }
    }
    #endregion
}
