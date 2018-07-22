using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class Bullet : MonoBehaviour
	{
		public float lifeTime = 3;
		public float bulletSpeed;
		private Rigidbody _rb;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody>();
		}

		// Use this for initialization
		void Start()
		{
			Destroy(gameObject, lifeTime);
			_rb.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
		}

	}
}
