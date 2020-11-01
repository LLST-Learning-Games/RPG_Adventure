using System;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG_Adventure Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damageModAdd = 5f;
        [SerializeField] float damageModPercent = 0.05f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;

        const string WEAPON_NAME = "weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (weaponPrefab != null)
            {
                GameObject weapon = Instantiate(weaponPrefab, GetHandedness(rightHand, leftHand));
                weapon.name = WEAPON_NAME;
            }

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else
            {
                // This is a tricky bit of work! The variable 'overrideController' will be null if we cannot 
                // make this cast--which means it is at the default w/o overrides. If we CAN make the cast, we aren't 
                // at the default... which means we can look up the parent controller and reset 
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

                if(overrideController != null)
                {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WEAPON_NAME);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "destroying the old weapon";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandedness(Transform rightHand, Transform leftHand)
        {
            if (isRightHanded) return rightHand;
            else return leftHand;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandedness(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }

        public float GetDamageModAdd()
        {
            return damageModAdd;
        }

        public float GetDamageModPercent()
        {
            return damageModPercent;
        }

    }
}