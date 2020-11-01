using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextDestroyer : MonoBehaviour
    {
        [SerializeField] GameObject parentToDestroy;

        public void DestroyMe()
        {
            Destroy(parentToDestroy);
        }

    }
}