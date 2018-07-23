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
		while(_repeatCount < maxRepeatCount)
		{
			state = childs[0].Evaluate();
			switch (state)
			{
				case NodeState.RUNNING:
					continue;
				case NodeState.FAILED:
					_repeatCount++;
					break;
				case NodeState.SUCCESS:
					_repeatCount = 0;
					return state;
			}
		}
		//repeat count is max
		_repeatCount = 0;
		state = NodeState.FAILED;
		return state;
    }

	public int maxRepeatCount { get; private set; }
}
