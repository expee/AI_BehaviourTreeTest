using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Locomotion
{
	public class BotGait : MonoBehaviour
	{
        public enum LocomotionState
        {
            MOVING,
            BLOCKED,
            STATE_COUNT
        }
        private NavMeshAgent _nmAgent;
       
        private void Awake()
        {
            _nmAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            
        }

        public void SetBotDestination(Vector2 tgt)
        {
            tgtPos = new Vector3(tgt.x, transform.position.y, tgt.y);
            _nmAgent.SetDestination(tgtPos);
        }

        public void CheckLocomotionState()
        {

        }

        public Vector3 tgtPos { get; private set; }
        public LocomotionState state { get; private set; }
	}
}
