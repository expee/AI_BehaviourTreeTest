using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class Assembly : MonoBehaviour
	{
		public enum GunAssemblyState
		{
			OK,
			MAGAZINE_EMPTY,
			HAMMER_LOCKED,
			STATE_COUNT
		}

		private Trigger _trigger;

		private void Awake()
		{
			_trigger = GetComponent<Trigger>();
		}

		void Start()
		{

		}

		void Update()
		{

		}

		public void Fire()
		{
			_trigger.Pull();
		}

		public void StopFire()
		{
			_trigger.Release();
		}

		public GunAssemblyState CheckGun()
		{
			if (IsMagazineEmpty())
			{
				state = GunAssemblyState.MAGAZINE_EMPTY;
				return state;
			}
			else if(isHammerLocked())
			{
				state = GunAssemblyState.HAMMER_LOCKED;
				return state;
			}
			else
			{
				state = GunAssemblyState.OK;
				return state;
			}
		}

		public bool IsFiringSuccess()
		{
			return _trigger.fireSuccess;
		}

		public bool ReloadGun()
		{
			return _trigger.firingMechanism.Reload();
		}

		public bool IsMagazineEmpty()
		{
			return _trigger.firingMechanism.magazineEmpty;
		}

		public bool isHammerLocked()
		{
			return _trigger.hammerLocked;
		}

		#region Properties
		public GunAssemblyState state { get; private set; }
		#endregion
	}
}
