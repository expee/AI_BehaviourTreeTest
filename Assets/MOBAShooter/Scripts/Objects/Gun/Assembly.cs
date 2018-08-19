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
			RELOADING,
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
			else if(isReloading())
			{
				state = GunAssemblyState.RELOADING;
				return state;
			}
			else
			{
				state = GunAssemblyState.OK;
				return state;
			}
		}

        public void SetFireMode(Trigger.FireMode mode)
        {
            if (mode >= Trigger.FireMode.SINGLE && mode < Trigger.FireMode.MODE_COUNT)
                _trigger.mode = mode;
            else
                _trigger.mode = Trigger.FireMode.AUTO;
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

		public bool isReloading()
		{
			return _trigger.firingMechanism.isReloading;
		}

		#region Properties
		public GunAssemblyState state { get; private set; }
		#endregion
	}
}
