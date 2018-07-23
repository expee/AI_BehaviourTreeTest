using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    public SelectorNode(List<Node> tgtChilds)
    {
        currentRunningChild = null;
        isAnyChildRunning = false;
        state = NodeState.FAILED;
        childs = tgtChilds;
    }
    public override NodeState Evaluate()
    {
        if(!isAnyChildRunning)
        {
            foreach (Node child in childs)
            {
                state = child.Evaluate();
                switch (state)
                {
                    case NodeState.SUCCESS:
						isAnyChildRunning = false;
						currentRunningChild = null;
						return state;
                    case NodeState.RUNNING:
                        currentRunningChild = child;
                        isAnyChildRunning = true;
                        return state;
                    case NodeState.FAILED:
                        continue;
                }
            }
            //All failed
            isAnyChildRunning = false;
            currentRunningChild = null;
            state = NodeState.FAILED;
            return state;
        }
        else
        {
            int startIdx = childs.FindIndex(nd => nd.Equals(currentRunningChild));
            for (int i = startIdx; i < childs.Count; i++)
            {
                state = childs[i].Evaluate();
                switch (state)
                {
                    case NodeState.SUCCESS:
						isAnyChildRunning = false;
						currentRunningChild = null;
						return state;
                    case NodeState.RUNNING:
                        currentRunningChild = childs[i];
                        isAnyChildRunning = true;
                        return state;
                    case NodeState.FAILED:
                        continue;
                }
            }
            //All failed
            isAnyChildRunning = false;
            currentRunningChild = null;
            state = NodeState.FAILED;
            return state;
        }
    }

    public bool isAnyChildRunning { get; private set; }
    public Node currentRunningChild { get; private set; }
}
