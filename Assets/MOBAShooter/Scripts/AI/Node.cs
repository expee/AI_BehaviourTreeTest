using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public enum NodeState
    {
        FAILED,
        SUCCESS,
        RUNNING,
        STATE_COUNT
    };

    public Node(){}
    public abstract NodeState Evaluate();

    #region Properties
    public List<Node> childs { get; protected set; }
    public NodeState state { get; protected set; }
	public bool forceCheck { get; protected set; }
    #endregion
}
