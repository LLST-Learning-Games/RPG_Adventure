using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{

    public class LvlDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        Text playerLvlText;

        // Start is called before the first frame update
        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            playerLvlText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            playerLvlText.text = baseStats.GetLevel().ToString();
        }
    }
}