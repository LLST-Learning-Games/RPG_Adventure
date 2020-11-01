using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using RPG.Saving;
using RPG.Core;
using RPG.Control;

namespace RPG.SceneManagement
{

    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A, B, C, D, E
        }


        [SerializeField] int sceneIndexToLoad = 0;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination = DestinationIdentifier.A;

        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeWaitTime = .25f;
        [SerializeField] float fadeInTime = .5f;

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.tag == "Player")
            {
                StartCoroutine(Transition());
            }

        }
        

        private IEnumerator Transition()
        {

            if (sceneIndexToLoad < 0)
            {
                Debug.LogError("Scene Index not set");
                yield break;
            }

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();


            PlayerControlToggle(false);
            yield return fader.FadeOut(fadeOutTime);

            DontDestroyOnLoad(gameObject);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneIndexToLoad);

            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            
            UpdatePlayer(otherPortal);


            // ==================================================
            // Why do we save twice? Well, we saved the state of the scene we left
            // before closing it. Now we've arrived in the new scene and updated
            // the player's location. What if we exit and then load the game?
            // It would return us to the old scene (and likely the player would have run
            // beyond the portal trigger as well. Oops! So, we save the fact that
            // the player has arrived in the new scene.
            // ==================================================
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);

            yield return fader.FadeIn(fadeInTime);
            PlayerControlToggle(true);

            Destroy(gameObject);
        }

        private void PlayerControlToggle(bool v)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = v;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");

            //player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);

            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal != this && portal.destination == destination)
                    return portal;
            }

            return null;
        }
    }
}