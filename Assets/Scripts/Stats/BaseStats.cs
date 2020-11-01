using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpFX = null;
        [SerializeField] bool usePlayerModifiers = false;

        int currentLevel = 0;

        Experience exp;

        public event Action onLevelUp;

        private void Awake()
        {
            exp = GetComponent<Experience>();
        }

        private void OnEnable()
        {
            if(exp != null)
            {
                exp.onExpGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (exp != null)
            {
                exp.onExpGained -= UpdateLevel;
            }
        }

        private void Start()
        {
            currentLevel = CalculateLevel();
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                Debug.Log("Level up!");
                LevelupEffect();
                onLevelUp();
            }
        }

        private void LevelupEffect()
        {
            if (levelUpFX != null)
                Instantiate(levelUpFX, transform);
        }

        public float GetStat(Stat stat)
        {
            if (usePlayerModifiers)     //if we are the player, add the modifiers
                return (GetBase(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat));
            else
                return GetBase(stat);   //if we are not (ie, we are an enemy) then skip on the modifiers
        }

        

        public int GetLevel()
        {
            if(currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }

            return currentLevel;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null)
                return startingLevel;

            float currentXP = experience.GetExp();
            int penultimateLevel = progression.GetLevels(Stat.ExpLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float expToLevelUp = progression.GetStat(Stat.ExpLevelUp, characterClass, level);
                if(expToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        private float GetBase(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            //Debug.Log(stat.ToString() + " additive modifier is " + total);
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            //Debug.Log(stat.ToString() + " percentage modifier is " + total);

            return total;
        }
    }

    

}
