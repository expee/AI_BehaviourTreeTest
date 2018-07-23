using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterNode : Node
{
    public InverterNode(Node tgtNode)
    {
        //Inverter node will always only have 1 child node
        childs = new List<Node>();
        childs.Add(tgtNode);
        state = NodeState.FAILED;
    }

    public override NodeState Evaluate()
    {
        childs[0].Evaluate();
        state = (childs[0].state == NodeState.FAILED) ? NodeState.SUCCESS : (childs[0].state == NodeState.RUNNING) ? NodeState.RUNNING : NodeState.FAILED;
        return state;
    }
}
