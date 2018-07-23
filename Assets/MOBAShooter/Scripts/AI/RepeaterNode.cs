using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeaterNode : Node
{
	private int _repeatCount;

	public RepeaterNode(Node tgtNode, int maxRepeat)
	{
		//Repeater node will always only have 1 child node
		maxRepeatCount = maxRepeat;
		_repeatCount = 0;
		childs = new List<Node>
		{
			tgtNode
		};
		state = NodeState.FAILED;
	}

    public override NodeState Evaluate()
    {
		if(++_repeatCount < maxRepeatCount)
		{
			state = childs[0].Evaluate();
			return state;
		}
		else
		{
			state = NodeState.FAILED;
			return state;
		}
    }

	public int maxRepeatCount { get; private set; }
}
