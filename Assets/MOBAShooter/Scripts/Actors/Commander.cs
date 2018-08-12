using UnityEngine;

public class Commander : Actor
{
    private SelectorNode _rootNode;


    private LeafNode _run;
    private LeafNode _celebrate;


    private Locomotion.BotGait _gait;

    private GameObject _selectedCover;
    private Vector3 _coverSpot;
    private Vector3 _standingSpot;
    private bool _isInCover;
    private bool _isFiring;

    private bool _targetingCoverOrStandingSpot; //true = cover spot; false = standing spot
    private void Awake()
    {
        _gait = GetComponent<Locomotion.BotGait>();
        CreateBehaviourTree();
        ResetCharacteristic();
    }

    private void Start ()
    {
        //Reset suppression, there's no point keeping it suppressed upon re-activation
        suppression = 0;
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
        if(_targetingCoverOrStandingSpot) //targeting cover spot
            _gait.SetBotDestination(_coverSpot);
        else //targeting standing spot
            _gait.SetBotDestination(_standingSpot);

        Locomotion.BotGait.LocomotionState locoState = _gait.CheckLocomotionState();
        if(locoState == Locomotion.BotGait.LocomotionState.MOVING)
        {
            return Node.NodeState.RUNNING;
        }
        else if(locoState == Locomotion.BotGait.LocomotionState.ARRIVED)
        {
            if (_targetingCoverOrStandingSpot)
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
        return Node.NodeState.FAILED;
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
        return Node.NodeState.FAILED;
    }
    #endregion

    #region Test Nodes
    bool IsNotInCover()
    {
        return !_isInCover;
    }

    bool IsEnemyPresent()
    {
        return false;
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
        return false
    }
    #endregion
    
    #region Properties
    public int bravery { get; set; }
    public int suppression { get; private set; }
    public int accuracy { get; set; }
    public bool isCharacteristicSet { get; private set; }
    #endregion
}
