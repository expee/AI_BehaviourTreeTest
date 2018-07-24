using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafNode : Node
{
    public delegate NodeState NodeAction ();
    private NodeAction action;

    public LeafNode(NodeAction delegatedAction)
    {
        action = delegatedAction;
        state = NodeState.FAILED;
    }
    public override NodeState Evaluate()
    {
        state = action();
        return state;
    }
}
