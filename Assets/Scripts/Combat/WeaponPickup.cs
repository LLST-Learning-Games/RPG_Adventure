using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;

namespace RPG.Combat {

    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] float timeToRespawn = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && weaponPrefab != null)
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter other)
        {
            other.EquipWeapon(weaponPrefab);
            StartCoroutine(HideForRespawn());
        }

        private IEnumerator HideForRespawn()
        {
            ShowPickup(false);
            yield return new WaitForSeconds(timeToRespawn);
            ShowPickup(true);
        }


        private void ShowPickup(bool isShowing)
        {
            GetComponent<Collider>().enabled = isShowing;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isShowing);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<Fighter>());
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

