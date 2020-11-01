using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        //[SerializeField] Transform projectileTarget = null;
        [SerializeField] float projectileSpeed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 3f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        // Update is called once per frame
        void Update()
        {
            if (isHoming && target.GetIsAlive()) 
                LockOnTarget();

            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

       

        public void SetTarget(Health newTarget, GameObject newInstigator, float newDamage)
        {
            target = newTarget;
            damage = newDamage;
            instigator = newInstigator;
            LockOnTarget();
            Destroy(gameObject, maxLifeTime);
        }

        private void LockOnTarget()
        {
            if (target == null && !target.GetIsAlive()) return;
            transform.LookAt(GetAimLocation());
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule != null)
                return target.transform.position + Vector3.up * targetCapsule.height * 0.66f;
            else
                return target.transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            Health otherHealth = other.gameObject.GetComponent<Health>();
            if (otherHealth != target || !target.GetIsAlive()) return;

            otherHealth.TakeDamage(instigator, damage);
            projectileSpeed = 0;

            onHit.Invoke();

            if (hitEffect != null)
            {

                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
            
        }
    }
}