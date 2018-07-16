using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class Bullet : MonoBehaviour
	{
		public float lifeTime = 3;

		private void Awake()
		{
			
		}

		// Use this for initialization
		void Start()
		{
			Destroy(gameObject, lifeTime);
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
