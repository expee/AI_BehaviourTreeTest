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
		public float inherentAccuracy;

		private bool _ammoInReceiver;

		private void Awake()
		{
			_ammoInReceiver = false;
		}

		void Start()
		{

		}

		void Update()
		{

		}

		public bool Fire()
		{
			if(_ammoInReceiver)
			{
				Instantiate(bullet, muzzlePosition, transform.rotation);
				_ammoInReceiver = false;
				StartCoroutine("BreechLoading");
				return true;
			}
			//TODO Jam Mechanism
			return true;
		}

		public bool Reload()
		{
			return false;
		}

		IEnumerator BreechLoading()
		{
			yield return new WaitForSeconds(fireRate);
			_ammoInReceiver = true;
		}
	}
}
