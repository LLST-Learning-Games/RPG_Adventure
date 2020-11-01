using UnityEngine;
using System.Collections;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float expPoints = 0;

        //public delegate void ExpGainedDelegate();
        //public event ExpGainedDelegate onExpGained;

        // an Action is a pre-defined delegate that has no return type or variables
        // so, we've commented out the above
        public event Action onExpGained;
   

        public object CaptureState()
        {
            return expPoints;
        }

        public void GainExperience(float expToGain)
        {
            expPoints += expToGain;
            onExpGained();
        }

        public void RestoreState(object state)
        {
            expPoints = (float)state;
        }

        public float GetExp()
        {
            return expPoints;
        }
    }
}