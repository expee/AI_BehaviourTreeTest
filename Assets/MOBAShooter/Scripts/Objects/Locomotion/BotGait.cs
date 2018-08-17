using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Locomotion
{
    [RequireComponent(typeof(NavMeshAgent))]
	public class BotGait : MonoBehaviour
	{
        public enum LocomotionState
        {
            MOVING,
            BLOCKED,
            ARRIVED,
            IDLE,
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

        public LocomotionState CheckLocomotionState()
        {
            if(_nmAgent.pathPending)
            {
                state = LocomotionState.MOVING;
                return state;
            }
            else if(_nmAgent.velocity.magnitude != 0 && _nmAgent.remainingDistance > _nmAgent.stoppingDistance)
            {
                state = LocomotionState.MOVING;
                return state;
            }
            else if(_nmAgent.velocity.magnitude == 0 && _nmAgent.remainingDistance <= _nmAgent.stoppingDistance)
            {
                state = LocomotionState.ARRIVED;
                _nmAgent.ResetPath();
                tgtPos = Vector3.zero;
                return state;
            }
            else if (_nmAgent.pathStatus == NavMeshPathStatus.PathInvalid || _nmAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                state = LocomotionState.BLOCKED;
                tgtPos = Vector3.zero;
                return state;
            }
            else if(_nmAgent.velocity.magnitude == 0 && !_nmAgent.hasPath)
            {
                state = LocomotionState.IDLE;
                tgtPos = Vector3.zero;
                return state;
            }
            else
            {
                state = LocomotionState.IDLE;
                tgtPos = Vector3.zero;
                return state;
            }
        }

        public Vector3 tgtPos { get; private set; }
        public LocomotionState state { get; private set; }
	}
}
