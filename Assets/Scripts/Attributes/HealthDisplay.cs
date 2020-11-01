using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{

    public class HealthDisplay : MonoBehaviour
    {
        Health playerHealth;
        Text playerHealthText;

        // Start is called before the first frame update
        void Awake()
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
            playerHealthText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            // Old percentage display style:
            //playerHealthText.text = Mathf.Round(playerHealth.GetHealthPercent()).ToString() + "%";


            playerHealthText.text = playerHealth.GetHealthPoints() + " / " + playerHealth.GetMaxHealth();
        }
    }
}