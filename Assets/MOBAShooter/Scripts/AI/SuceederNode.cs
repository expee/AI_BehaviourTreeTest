using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuceederNode : Node
{

    public SuceederNode(Node tgtNode)
    {
        childs = new List<Node>
        {
            tgtNode
        };
        state = NodeState.SUCCESS;
    }

    public override NodeState Evaluate()
    {
        childs[0].Evaluate();
        state = NodeState.SUCCESS;
        return state;
    }
}
