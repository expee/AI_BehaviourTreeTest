using System.Collections.Generic;
using UnityEngine;

public class Commander : Actor
{
    public Gun.Assembly gun;

    private LeafNode _run;
    private LeafNode _celebrate;
    private LeafNode _alreadyHasCover;
    private LeafNode _pickNearestCover;
    private LeafNode _takeCover;
    private LeafNode _reload;
    private LeafNode _randomlyTryToPickNewCover;
    private LeafNode _hasStandingSpot;
    private LeafNode _pickStandingSpot;
    private LeafNode _aimAndFire;

    private SequenceNode _goToNearestCover;
    private SequenceNode _takeCoverAndReload;
    private SequenceNode _getOutOfCover;
    private SequenceNode _tryToFireAtEnemy;

    private SelectorNode _checkCoverPicked;
    private SelectorNode _checkStandingSpotPicked;
    private SelectorNode _engage;
    private SelectorNode _rootNode;

    private TestNode _isNotInCover;
    private TestNode _isNotFiring1;
    private TestNode _isNotFiring2;
    private TestNode _shouldReload;
    private TestNode _isNotSuppresed;
    private TestNode _isMagazineNotEmpty;
    private TestNode _isEnemyPresent;
    private TestNode _isSuppresed;
    private TestNode _isMagazineEmpty;

    private SuceederNode _considerationTaken;

    private Locomotion.BotGait _gait;

    private Obstacle.BoxColliderDescriptor _selectedCover;
    private Vector3 _coverSpot;
    private Vector3 _standingSpot;

    private bool _isInCover;
    private bool _isFiring;
    private List<Commander> _enemies;

    private Vector3 cornerA;
    private Vector3 cornerB;

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
        suppression = 100;

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
        //    //Debug.LogError("ERROR!! Characteristics for " + gameObject.name + " haven't been set");
	}

    private void CreateBehaviourTree()
    {
        _run              = new LeafNode(Run);
        _celebrate = new LeafNode(Celebrate);
        _pickNearestCover = new LeafNode(PickNearestCover);
        _alreadyHasCover  = new LeafNode(AlreadyHasCover);
        _takeCover = new LeafNode(TakeCover);
        _reload = new LeafNode(Reload);
        _randomlyTryToPickNewCover = new LeafNode(RandomlyTryToPickNewCover);
        _hasStandingSpot = new LeafNode(HasStandingSpot);
        _pickStandingSpot = new LeafNode(PickStandingSpot);
        _aimAndFire = new LeafNode(AimAndFire);
        //_considerationTaken = new SuceederNode(_randomlyTryToPickNewCover);
        //_shouldReload = new TestNode(_reload, ShouldReload);

        _checkCoverPicked = new SelectorNode(new List<Node> { _alreadyHasCover ,_pickNearestCover });
        _goToNearestCover = new SequenceNode(new List<Node> { _checkCoverPicked, _run});
        _isNotInCover = new TestNode(_goToNearestCover, IsNotInCover);


        _takeCoverAndReload = new SequenceNode(new List<Node> { _takeCover, _reload });
        //_isNotFiring1 = new TestNode(_takeCoverReloadAndConsiderFlanking, IsNotFiring);


        _isMagazineNotEmpty = new TestNode(_aimAndFire, IsMagazineNotEmpty);
        _isMagazineEmpty = new TestNode(_takeCoverAndReload, () => !IsMagazineNotEmpty());
        _isSuppresed = new TestNode(_takeCoverAndReload, () => !IsNotSuppressed());
        _isNotSuppresed = new TestNode(_isMagazineNotEmpty, IsNotSuppressed);
        _checkStandingSpotPicked = new SelectorNode(new List<Node> { _hasStandingSpot, _pickStandingSpot});
        _getOutOfCover = new SequenceNode(new List<Node> { _checkStandingSpotPicked, _run });
        //_isNotFiring2 = new TestNode(_getOutOfCover, IsNotFiring);
        _tryToFireAtEnemy = new SequenceNode(new List<Node> { _getOutOfCover, _isNotSuppresed });

        _engage = new SelectorNode(new List<Node> { _isNotInCover, _isSuppresed, _isMagazineEmpty, _randomlyTryToPickNewCover, _tryToFireAtEnemy });
        _isEnemyPresent = new TestNode(_engage, IsEnemyPresent);

        _rootNode         = new SelectorNode(new List<Node> { _isEnemyPresent, _celebrate });
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
            //Debug.Log(name + "Is Running");
            return Node.NodeState.RUNNING;
        }
        else if(locoState == Locomotion.BotGait.LocomotionState.ARRIVED)
        {
            //Debug.Log(name + "Is Arrived");
            if (Vector3.Distance(transformRef.position, _coverSpot) <= 0.05f)
            {
                //Debug.Log(name + " is in cover");
                _isInCover = true;
            }
            return Node.NodeState.SUCCESS;
        }
        else
        {
            //Debug.Log(name + "Got trouble en route = " + locoState);
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
        _isInCover = false;
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
        Commander nearestEnemy = FindNearestEnemy();
        List<Vector3> nearestCorners = FindNearestCorners(nearestEnemy, _selectedCover);
        Vector3 selectedCorner = Vector3.zero;
        for(int i = 0; i < nearestCorners.Count; i++)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(nearestCorners[i], nearestEnemy.transformRef.position - nearestCorners[i], out hitInfo, 10000.0f))
            {
                //Debug.Log(name + "Raycasting toward enemy, Got = " + hitInfo.collider.tag);

                if (hitInfo.collider.tag == "BotBody")
                {
                    selectedCorner = nearestCorners[i];
                    break;
                }
            }
        }
        if (selectedCorner != Vector3.zero)
        {
            Vector3 perpendicularVec90 =  Quaternion.AngleAxis(90, Vector3.up) * (nearestEnemy.transformRef.position - selectedCorner);
            perpendicularVec90 = perpendicularVec90.normalized;
            Vector3 perpendicularVec270 = Quaternion.AngleAxis(270, Vector3.up) * (nearestEnemy.transformRef.position - selectedCorner);
            perpendicularVec270 = perpendicularVec270.normalized;
            float totalDist90 = 0;
            for (int i = 0; i < _selectedCover.xzBoundaries.Count; i++)
            {
                totalDist90 += Vector3.Distance((selectedCorner + perpendicularVec90), new Vector3(_selectedCover.xzBoundaries[i].x, nearestEnemy.transformRef.position.y, _selectedCover.xzBoundaries[i].y));
            }
            float totalDist270 = 0;
            for (int i = 0; i < _selectedCover.xzBoundaries.Count; i++)
            {
                totalDist270 += Vector3.Distance((selectedCorner + perpendicularVec270), new Vector3(_selectedCover.xzBoundaries[i].x, nearestEnemy.transformRef.position.y, _selectedCover.xzBoundaries[i].y));
            }

            if(totalDist270 > totalDist90)
                _standingSpot = selectedCorner + perpendicularVec270;
            else
                _standingSpot = selectedCorner + perpendicularVec90;
            _gait.SetBotDestination(new Vector2(_standingSpot.x, _standingSpot.z));
            //Debug.Log(name + "Success Finding Standing spot");
            return Node.NodeState.SUCCESS;
        }
        else
        {
            //Debug.Log(name + "Failed Finding Standing spot");
            return Node.NodeState.FAILED;
        }

    }

    Node.NodeState AimAndFire()
    {
        //Debug.Log(name + " FIRING");
        Commander nearestEnemy = FindNearestEnemy();
        transformRef.LookAt(nearestEnemy.transformRef);
        gun.Fire();
        _isFiring = true;
        return Node.NodeState.FAILED;
    }

    Node.NodeState TakeCover()
    {
        _gait.SetBotDestination(new Vector2(_coverSpot.x, _coverSpot.z));
        Locomotion.BotGait.LocomotionState locoState = _gait.CheckLocomotionState();
        if (locoState == Locomotion.BotGait.LocomotionState.MOVING)
        {
            return Node.NodeState.RUNNING;
        }
        else if (locoState == Locomotion.BotGait.LocomotionState.ARRIVED)
        {
            Debug.Log(name + " Take Cover => SUCCESS");
            _standingSpot = Vector3.zero;
            return Node.NodeState.SUCCESS;
        }
        else
        {
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState RandomlyTryToPickNewCover()
    {
       
        bool shouldChangeCover = Random.Range(0.0f, 1.0f) < 0.01f;
        if (shouldChangeCover)
        {
            Debug.Log(name + " Trying to pick new cover => SUCCESS");
            LeaveLastCover();
            FindRandomCover();
            PickCoverSpot();
            _isInCover = false;
            _gait.SetBotDestination(new Vector2(_coverSpot.x, _coverSpot.z));
            return Node.NodeState.SUCCESS;
        }
        else
        {
            Debug.Log(name + " Trying to pick new cover => FAILED");
            return Node.NodeState.FAILED;
        }
            
    }

    Node.NodeState Reload()
    {
        Gun.Assembly.GunAssemblyState state = gun.CheckGun();
        if(state == Gun.Assembly.GunAssemblyState.MAGAZINE_EMPTY)
        {
            Debug.Log(name + " Start Reload");
            gun.ReloadGun();
            return Node.NodeState.RUNNING;
        }
        else if(state == Gun.Assembly.GunAssemblyState.RELOADING)
        {
            Debug.Log(name + " Reloading");
            return Node.NodeState.RUNNING;
        }
        else if (state == Gun.Assembly.GunAssemblyState.OK)
        {
            Debug.Log(name + " Reload => SUCCESS");
            return Node.NodeState.SUCCESS;
        }
        else
        {
            Debug.Log(name + " Reload => FAILED with " + state);
            return Node.NodeState.FAILED;
        }
    }

    Node.NodeState Celebrate()
    {
        return Node.NodeState.SUCCESS;
    }
    #endregion

    #region Test Nodes
    bool IsNotInCover()
    {
        //Debug.Log(name + " testing is not in cover = " + !_isInCover);
        return !_isInCover;
    }

    bool IsEnemyPresent()
    {
        _enemies.RemoveAll(en => en.state == State.DEAD);
        return _enemies.Count > 0;
    }

    bool IsNotFiring()
    {
        //Debug.Log(name + " testing is not firing = " + !_isFiring);
        return !_isFiring;
    }

    bool IsNotSuppressed()
    {
        if (suppression > 0)
        {
            //Debug.Log(name + " Not Suppresed");
            return true;
        }

        else
        {
            //Debug.Log(name + " Suppresed");
            _isFiring = false;
            gun.StopFire();
            return false;
        }
    }

    bool IsMagazineNotEmpty()
    {
        Gun.Assembly.GunAssemblyState state = gun.CheckGun();
        if (state == Gun.Assembly.GunAssemblyState.MAGAZINE_EMPTY)
        {
            //Debug.Log(name + " Magazine empty");
            _isFiring = false;
            gun.StopFire();
            return false;
        }
        else
        {
            //Debug.Log(name + " Magazine not empty");
            return true;
        }

    }

    bool ShouldReload()
    {
        Gun.Assembly.GunAssemblyState state = gun.CheckGun();
        if (state == Gun.Assembly.GunAssemblyState.MAGAZINE_EMPTY)
            return true;
        else
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
        Vector3 binormal = Vector3.zero;
        foreach(Vector2 corner in _selectedCover.xzBoundaries)
        {
            binormal += new Vector3(corner.x, tgtFace.y, corner.y) - tgtFace;
        }
        binormal = binormal.normalized;

        _coverSpot = tgtFace + -binormal * 0.8f;
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

    private List<Vector3> FindNearestCorners(Commander enemy, Obstacle.BoxColliderDescriptor obstacle)
    {
        Vector3 corner1 = Vector3.zero;
        Vector3 corner2 = Vector3.zero;
        float nearestDist1 = float.MaxValue;
        float nearestDist2 = float.MaxValue;
        for (int i = 0; i < obstacle.xzBoundaries.Count; i++)
        {
            float dist = Vector3.Distance(enemy.transformRef.position, new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y));

            if (dist < nearestDist1)
            {
                nearestDist2 = nearestDist1;
                nearestDist1 = dist;
                corner2 = corner1;
                corner1 = new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y);
            }

            else if (dist < nearestDist2)
            {
                nearestDist2 = dist;
                corner2 = new Vector3(obstacle.xzBoundaries[i].x, enemy.transformRef.position.y, obstacle.xzBoundaries[i].y);
            }

        }
        return new List<Vector3> { corner1, corner2 }; ;
    }

    private void PrepareGun()
    {
        gun.SetFireMode(Gun.Trigger.FireMode.AUTO);
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
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(_standingSpot, .5f);
        }
    }
    #endregion
}
