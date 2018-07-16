using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class Trigger : MonoBehaviour
	{
		public enum FireMode
		{
			SINGLE,
			BURST,
			AUTO,
			MODE_COUNT
		};

		//There's no point in using the trigger without its firing mechanism


		private int _firingSuccessCount;

		private void Awake()
		{
			firingMechanism = GetComponent<FiringMechanism>();
		}

		void Start()
		{
			triggerPulled = false;
			hammerLocked = false;
		}

		void Update()
		{

		}

		public void Pull()
		{
			triggerPulled = true;
			switch (mode)
			{
				case FireMode.AUTO:
					{
						fireSuccess = firingMechanism.Fire();
						hammerLocked = false;
					}
					break;
				case FireMode.BURST:
					{
						if(!hammerLocked)
						{
							fireSuccess = firingMechanism.Fire();
							if (fireSuccess)
								_firingSuccessCount++;
							if (_firingSuccessCount >= 3)
								hammerLocked = true;
						}

					}
					break;
				case FireMode.SINGLE:
					{
						if(!hammerLocked)
						{
							fireSuccess = firingMechanism.Fire();
							if (fireSuccess)
								hammerLocked = true;
						}
					}
					break;
			}
		}

		public void Release()
		{
			triggerPulled = false;
			hammerLocked = false;
		}

		#region Properties
		public FireMode mode { get; set; }
		public FiringMechanism firingMechanism { get; private set; }
		public bool fireSuccess { get; private set; }
		public bool hammerLocked { get; private set; }
		public bool triggerPulled { get; private set; }
		#endregion
	}
}
