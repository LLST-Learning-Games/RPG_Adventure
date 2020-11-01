using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Image healthBarFG;
        [SerializeField] Canvas healthBar;
        [SerializeField] Health health;

        // Update is called once per frame
        void Update()
        {
            float healthScale = health.GetHealthPercent();
            if (Mathf.Approximately(healthScale, 0) || Mathf.Approximately(healthScale, 1))
            {
                healthBar.enabled = false;
                return;
            }
            else
                healthBar.enabled = true;


            healthBarFG.transform.localScale = new Vector3(healthScale, 1, 1);
        }
    }
}