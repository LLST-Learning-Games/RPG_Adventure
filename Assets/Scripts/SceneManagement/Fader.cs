using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {

        CanvasGroup canvasGroup;
        private bool fadingInProgress = false;
        

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            //StartCoroutine(FadeOut(3f));
        }


        public IEnumerator FadeOut(float time)
        {
            //===================================
            // How does the math here work?
            //
            // fps = time / deltaTime
            // deltaAlpha = 1 / fps
            // deltaAlpha = 1 * deltaTime / time
            //===================================

            fadingInProgress = true;
            while (canvasGroup.alpha < 1) 
            {
                canvasGroup.alpha += Time.deltaTime / time; // note this is the paramater, not Time.time 
                yield return null; // returning null means this runs every frame    
            }
            fadingInProgress = false;

        }

        public IEnumerator FadeIn(float time)
        {
            //===================================
            // How does the math here work?
            //
            // fps = time / deltaTime
            // deltaAlpha = 1 / fps
            // deltaAlpha = 1 * deltaTime / time
            //===================================


            while (canvasGroup.alpha > 0 && fadingInProgress == false)
            {
                canvasGroup.alpha -= Time.deltaTime / time; // note this is the paramater, not Time.time 
                yield return null; // returning null means this runs every frame    
            }


        }

        public void InstaFade()
        {
            canvasGroup.alpha = 1;
        }
    }
}