using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNode : Node
{
	public delegate bool NodeTest();
	private NodeTest _testMethod;

	public TestNode(Node tgtNode, NodeTest testMethod)
	{
		_testMethod = testMethod;
		childs = new List<Node>
		{
			tgtNode
		};
		state = NodeState.FAILED;
	}

	public override NodeState Evaluate()
	{
		if(_testMethod())
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
}
