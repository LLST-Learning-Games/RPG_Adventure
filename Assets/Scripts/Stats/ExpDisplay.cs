using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{

    public class ExpDisplay : MonoBehaviour
    {
        Experience playerExp;
        Text playerExpText;

        // Start is called before the first frame update
        void Awake()
        {
            playerExp = GameObject.FindWithTag("Player").GetComponent<Experience>();
            playerExpText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            playerExpText.text = Mathf.Round(playerExp.GetExp()).ToString();
        }
    }
}