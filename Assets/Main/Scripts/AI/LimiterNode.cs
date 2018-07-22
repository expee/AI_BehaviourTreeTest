using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterNode : Node {

    public LimiterNode()
    {
        state = NodeState.FAILED;
    }

    public override NodeState Evaluate()
    {
        return state;
    }
}
