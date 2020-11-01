using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {

        [SerializeField] DamageText damageTextPrefab;


        public void SpawnDamageText(float damText)
        {
            Debug.Log("spawning damage text");
            DamageText newDamText = Instantiate(damageTextPrefab,transform);
            if(newDamText)
                newDamText.SetText(damText.ToString());
        }
    }
}