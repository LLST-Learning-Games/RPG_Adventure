using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {

        [SerializeField] Text damText;


        internal void SetText(string v)
        {
            damText.text = v;
        }
    }
}
