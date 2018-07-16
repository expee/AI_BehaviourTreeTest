using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class FiringMechanism : MonoBehaviour
	{
		public Vector3 muzzlePosition;
		public Bullet bullet;
		public int ammoCapacity;
		public float fireRate;
		public float reloadSpeed;
		public float inherentAccuracy;

		private bool _ammoInReceiver;

		private void Awake()
		{
			_ammoInReceiver = false;
		}

		void Start()
		{
			ammoRemaining = ammoCapacity;
		}

		void Update()
		{

		}

		public bool Fire()
		{
			if(!magazineEmpty && _ammoInReceiver)
			{
				Instantiate(bullet, muzzlePosition, transform.rotation);
				ammoRemaining--;
				magazineEmpty = ammoRemaining == 0 ? true : false;
				_ammoInReceiver = false;
				StartCoroutine("BreechLoading");
				return true;
			}
			//TODO Jam Mechanism
			return false;
		}

		public bool Reload()
		{
			if(!isReloading)
			{
				StartCoroutine("ReloadMagazine");
				isReloading = true;
				return true;
			}
			return false;
		}

		IEnumerator BreechLoading()
		{
			yield return new WaitForSeconds(fireRate);
			_ammoInReceiver = true;
		}

		IEnumerator ReloadMagazine()
		{
			yield return new WaitForSeconds(reloadSpeed);
			ammoRemaining = ammoCapacity;
			isReloading = false;
		}

		#region Properties
		public bool magazineEmpty { get; private set; }
		public int ammoRemaining { get; private set; }
		public bool isReloading { get; private set; }
		#endregion
	}
}
