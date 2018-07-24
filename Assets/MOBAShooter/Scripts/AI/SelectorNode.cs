using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    public SelectorNode(List<Node> tgtChilds)
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
					return state;
				case NodeState.RUNNING:
					return state;
				case NodeState.FAILED:
					continue;
			}
		}
		//All failed
		state = NodeState.FAILED;
		return state;
	}
}
