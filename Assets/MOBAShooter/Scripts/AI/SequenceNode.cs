using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : Node
{
	public SequenceNode(List<Node> tgtChilds)
    {
        state = NodeState.FAILED;
        childs = tgtChilds;
    }

    public override NodeState Evaluate()
    {
		foreach (Node child in childs)
		{
			state = child.Evaluate();
			switch (state)
			{
				case NodeState.SUCCESS:
					continue;
				case NodeState.RUNNING:
					return state;
				case NodeState.FAILED:
					return state;
			}
		}
		//All success
		state = NodeState.SUCCESS;
		return state;
    }
}
