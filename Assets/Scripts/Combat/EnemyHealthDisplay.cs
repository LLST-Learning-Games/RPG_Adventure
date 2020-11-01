using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Attributes;

namespace RPG.Combat
{

    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health enemyHealth;
        Text enemyHealthText;
        const string NO_TARGET = "No Target";

        // Start is called before the first frame update
        void Awake()
        {
            enemyHealthText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            enemyHealth = GameObject.FindWithTag("Player").GetComponent<Fighter>().GetTarget();


            if (enemyHealth != null)
            {
                enemyHealthText.text = enemyHealth.GetHealthPoints() + " / " + enemyHealth.GetMaxHealth();
                //enemyHealthText.text = Mathf.Round(enemyHealth.GetHealthPercent()).ToString() + "%";
            }
            else
                enemyHealthText.text = NO_TARGET;
        }
    }
}