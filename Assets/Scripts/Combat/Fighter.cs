using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using System;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Weapon currentWeapon = null;
        BaseStats baseStats = null;

        bool hasWeapon = false;



        private void Start()
        {
            if(currentWeapon == null)
                EquipWeapon(defaultWeapon);

            baseStats = GetComponent<BaseStats>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            

            if (target == null) return;
            if (!target.GetIsAlive()) return;
            
            if (Vector3.Distance(target.transform.position, transform.position) > currentWeapon.GetWeaponRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else 
            {
                AttackBehaviour();
                GetComponent<Mover>().Cancel();
            }

        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
            hasWeapon = true;
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack >= currentWeapon.GetTimeBetweenAttacks())
            {
                transform.LookAt(target.transform);

                GetComponent<Animator>().ResetTrigger("CancelAttack");
                //This animation will trigger the Hit() event
                GetComponent<Animator>().SetTrigger("Attack");

                timeSinceLastAttack = 0;
            }
        }

        //Hit() is an Animation Event!
        void Hit()
        {
            if (target == null) return;

            float damage = baseStats.GetStat(Stat.Damage);
            

            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
                target.TakeDamage(gameObject, damage);

            Cancel();
        }

        // This is b/c the projectile weapon animations have events labeled "shoot" instead of "hit"
        void Shoot()
        {
            Hit();
        }

        public void Attack(GameObject newCombatTarget)
        {
            
            GetComponent<ActionScheduler>().StartAction(this);
            target = newCombatTarget.GetComponent<Health>();

            
        }

        public void Cancel()    //he calls this "StopAttack()"
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("CancelAttack");
            GetComponent<Mover>().Cancel();
            target = null;
            //print("Cancelling " + this);

        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamageModAdd();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamageModPercent();
            }
        }

        //returns true if the entity can be attacked (ie, is alive and an enemy)
        public bool CanAttack(GameObject myTarget)
        {
            return ((myTarget != null) && myTarget.GetComponent<Health>().GetIsAlive());
        }

        public object CaptureState()
        {
            if (currentWeapon != null)
              return currentWeapon.name;

            return null;
        }

        public void RestoreState(object state)
        {
            string savedWeaponeName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(savedWeaponeName);
            EquipWeapon(weapon);
        }

        
    }

}

