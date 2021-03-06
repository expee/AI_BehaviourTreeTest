﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class FiringMechanism : MonoBehaviour
	{
		public GameObject muzzle;
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
			isReloading = false;
			_ammoInReceiver = true;
			magazineEmpty = false;
		}

		public bool Fire()
		{
			if(!magazineEmpty && _ammoInReceiver)
			{
                Vector2 randomCircle = Random.insideUnitCircle * inherentAccuracy;
                Vector3 randomCone = new Vector3(randomCircle.x, randomCircle.y, 10.0f);
				Bullet ejectedAmmo = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
                ejectedAmmo.gameObject.transform.LookAt(transform.TransformPoint(randomCone));
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
			magazineEmpty = false;
			_ammoInReceiver = true;
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
