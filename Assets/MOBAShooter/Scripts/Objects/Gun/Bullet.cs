using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{
	public class Bullet : MonoBehaviour
	{
        public GameObject impactVFX;
        public List<MeshRenderer> meshRenderers;
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

        private void OnCollisionEnter(Collision collision)
        {
            foreach(MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = false;
            }

            GameObject bulletImpact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity);
            bulletImpact.transform.LookAt(transform.position + collision.contacts[0].normal);
            Destroy(gameObject);
        }
    }
}
