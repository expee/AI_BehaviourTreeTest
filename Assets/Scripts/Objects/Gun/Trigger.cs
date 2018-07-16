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
		private FiringMechanism _firingMechanism;

		private void Awake()
		{
			_firingMechanism = GetComponent<FiringMechanism>();
		}

		void Start()
		{

		}

		void Update()
		{

		}

		public void PullTrigger()
		{
			switch(mode)
			{
				case FireMode.AUTO:
					{ }
					break;
				case FireMode.BURST:
					{ }
					break;
				case FireMode.SINGLE:
					{ }
					break;
			}
		}

		#region Properties
		public FireMode mode { get; set; }

		#endregion
	}
}
