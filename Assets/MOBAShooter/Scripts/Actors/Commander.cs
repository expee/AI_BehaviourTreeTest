using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : Actor
{
    struct ActionStates
    {
        public Node.NodeState fireOnTheRun;
        public Node.NodeState run;
        public Node.NodeState getOutOfCover;
        public Node.NodeState aim;
        public Node.NodeState fireGun;
        public Node.NodeState setTargetToMove;
        public Node.NodeState searchEnemy;
        public Node.NodeState celebrate;
        public Node.NodeState aimAtEnemyGeneralDirection;
        public Node.NodeState panic;
    }
    private ActionStates actionStates;

    private SelectorNode _rootNode;
    private SelectorNode _selectMovement;
    private SelectorNode _stayPut;
    private SelectorNode _tryToKillEnemy;

    private SequenceNode _move;
    private SequenceNode _fireAtEnemy;
    private SequenceNode _forceMove;
    private SequenceNode _blindFire;

    private LeafNode _fireOnTheRun;
    private LeafNode _run;
    private LeafNode _getOutOfCover;
    private LeafNode _aim;
    private LeafNode _fireGun;
    private LeafNode _setTargetToMove;
    private LeafNode _searchEnemy;
    private LeafNode _celebrate;
    private LeafNode _aimAtEnemyGeneralDirection;
    private LeafNode _panic;


    private Locomotion.BotGait _gait;
    private void Awake()
    {
        _gait = GetComponent<Locomotion.BotGait>();
        CreateBehaviourTree();
        ResetCharacteristic();
    }

    private void Start ()
    {
        //Reset Fear, there's no point keeping it afraid upon re-activation
        fear = 0;
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
        _fireOnTheRun = new LeafNode(FireOnTheRun);
        _run = new LeafNode(Run);
        _getOutOfCover = new LeafNode(GetOutOfCover);
        _aim = new LeafNode(Aim);
        _fireGun = new LeafNode(FireGun);
        _setTargetToMove = new LeafNode(SetTargetToMove);
        _searchEnemy = new LeafNode(SearchEnemy);
        _celebrate = new LeafNode(Celebrate);
        _aimAtEnemyGeneralDirection = new LeafNode(AimAtEnemyGeneralDirection);
        _panic = new LeafNode(Panic);

        _move = new SequenceNode(new List<Node> {_setTargetToMove, _selectMovement});
        _fireAtEnemy = new SequenceNode(new List<Node> { _getOutOfCover, _aim, _fireGun});
        _forceMove = new SequenceNode(new List<Node> {_setTargetToMove, _selectMovement});
        _blindFire = new SequenceNode(new List<Node> {_aimAtEnemyGeneralDirection, _fireGun});

        _selectMovement = new SelectorNode(new List<Node> {_fireOnTheRun, _run});
        _stayPut = new SelectorNode(new List<Node> {_blindFire, _panic});
        _tryToKillEnemy = new SelectorNode(new List<Node> { _move, _fireAtEnemy, _forceMove, _stayPut });
        _rootNode = new SelectorNode(new List<Node> { _tryToKillEnemy, _searchEnemy, _celebrate});
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

    #region LeafNode Actions
    Node.NodeState Run()
    {
        Locomotion.BotGait.LocomotionState locoState = _gait.CheckLocomotionState();
        if(locoState == Locomotion.BotGait.LocomotionState.IDLE)
        {
        }
        return actionStates.run;
    }

    Node.NodeState FireOnTheRun()
    {
        actionStates.fireOnTheRun = Node.NodeState.FAILED;
        return actionStates.fireOnTheRun;
    }

    Node.NodeState GetOutOfCover()
    {
        return actionStates.fireOnTheRun;
    }

    Node.NodeState Aim()
    {
        return actionStates.aim;
    }

    Node.NodeState FireGun()
    {
        return actionStates.fireGun;
    }

    Node.NodeState SetTargetToMove()
    {
        return actionStates.setTargetToMove;
    }

    Node.NodeState SearchEnemy()
    {
        return actionStates.searchEnemy;
    }

    Node.NodeState Celebrate()
    {
        return actionStates.celebrate;
    }

    Node.NodeState AimAtEnemyGeneralDirection()
    {
        return actionStates.aimAtEnemyGeneralDirection;
    }

    Node.NodeState Panic()
    {
        return actionStates.panic;
    }

    #region Action Coroutines
    IEnumerator Action_Run()
    {
        actionStates.run = Node.NodeState.RUNNING;
        yield return null;
    }
    #endregion
    #endregion

    #region Properties
    public int bravery { get; set; }
    public int fear { get; private set; }
    public int accuracy { get; set; }
    public bool isCharacteristicSet { get; private set; }
    #endregion
}
