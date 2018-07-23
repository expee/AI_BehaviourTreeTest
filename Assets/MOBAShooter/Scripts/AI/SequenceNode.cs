using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : Node
{
	public SequenceNode(List<Node> tgtChilds, bool isForceCheck)
    {
		forceCheck = isForceCheck;
        currentRunningChild = null;
        isAnyChildRunning = false;
        state = NodeState.FAILED;
        childs = tgtChilds;
    }

    public override NodeState Evaluate()
    {
        if (!isAnyChildRunning)
        {
            foreach (Node child in childs)
            {
                state = child.Evaluate();
                switch (state)
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        currentRunningChild = child;
                        isAnyChildRunning = true;
                        return state;
                    case NodeState.FAILED:
						isAnyChildRunning = false;
						currentRunningChild = null;
						return state;
                }
            }
            //All success
            isAnyChildRunning = false;
            currentRunningChild = null;
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            int startIdx = childs.FindIndex(nd => nd.Equals(currentRunningChild));
			for (int i = 0; i < startIdx; i++)
			{
				if(childs[i].forceCheck)
				{
					childs[i].Evaluate();
					switch (state)
					{
						case NodeState.SUCCESS:
							continue;
						case NodeState.RUNNING:
							currentRunningChild = childs[i];
							isAnyChildRunning = true;
							return state;
						case NodeState.FAILED:
							isAnyChildRunning = false;
							currentRunningChild = null;
							return state;
					}
				}
			}
			for (int i = startIdx; i < childs.Count; i++)
            {
                state = childs[i].Evaluate();
                switch (state)
                {
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        currentRunningChild = childs[i];
                        isAnyChildRunning = true;
                        return state;
                    case NodeState.FAILED:
						isAnyChildRunning = false;
						currentRunningChild = null;
						return state;
                }
            }
            //All success
            isAnyChildRunning = false;
            currentRunningChild = null;
            state = NodeState.SUCCESS;
            return state;
        }
    }

    public bool isAnyChildRunning { get; private set; }
    public Node currentRunningChild { get; private set; }
}
