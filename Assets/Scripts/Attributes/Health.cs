using UnityEngine;
using System.Collections;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using UnityEngine.Events;

namespace RPG.Attributes
{

    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float currHealth = 100f;
        [SerializeField] bool isAlive = true;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }
        [SerializeField] TakeDamageEvent takeDamage;

        private BaseStats baseStats;


        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            

            if (baseStats != null)
            {
                currHealth = baseStats.GetStat(Stat.Health);
            }
        }

        private void OnEnable()
        {
            baseStats.onLevelUp += RestoreHealth;

        }

        private void OnDisable()
        {
            baseStats.onLevelUp -= RestoreHealth;

        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (isAlive)
            {
                //heath is set to the largest of (health - damage) or 0
                currHealth = Mathf.Max(currHealth - damage, 0);
                takeDamage.Invoke(damage);

                print(gameObject.name + " damaged for " + damage.ToString() + " by " + instigator.name + " leaving health of " + currHealth.ToString());

                if (currHealth <= 0 && isAlive == true)
                {
                    onDie.Invoke();
                    Die();
                    AwardExperience(instigator);
                }
            }
        }

        public void RestoreHealth()
        {
            currHealth = baseStats.GetStat(Stat.Health);
        }

        public void HealMe(float healAmt)
        {
            currHealth += healAmt;
            if (currHealth > baseStats.GetStat(Stat.Health))
            {
                currHealth = baseStats.GetStat(Stat.Health);
            }
            Debug.Log(gameObject.name + " healed for " + healAmt);
        }
       

        public float GetHealthPercent()
        {
            return currHealth / baseStats.GetStat(Stat.Health);
        }

        public float GetHealthPoints()
        {
            return currHealth;
        }

        public float GetMaxHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public void Die()
        {
            isAlive = false;
            GetComponent<Animator>().SetTrigger("Die");
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience expTarget = instigator.GetComponent<Experience>();
            if (expTarget == null) return;

            expTarget.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExpReward));

        }

        public bool GetIsAlive()
        {
            return isAlive;
        }

        public object CaptureState()
        {
            return currHealth;
        }

        public void RestoreState(object state)
        {
            currHealth = (float)state;

            if (currHealth <= 0 && isAlive == true)
            {
                Die();

            }
        }
    }
}