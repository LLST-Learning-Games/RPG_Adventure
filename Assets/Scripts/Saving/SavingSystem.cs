using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {

        private const string LAST_SCENE_INDEX = "lastSceneBuildIndex";

        public void Save(string saveFile)
        {
            // First, load what has been saved to this point. 
            // This will allow us to append the data from our current scene onto 
            // the end of what has already been saved (theoretically from other scenes).
            Dictionary<string, object> stateSoFar = LoadFile(saveFile);

            // Update the stateSoFar with the current scene state.
            CaptureState(stateSoFar);

            // Then, proceed with saving.
            SaveFile(saveFile, stateSoFar);
    
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        internal void ResetGame(string defaultSaveFile)
        {
            string path = GetPathFromSaveFile(defaultSaveFile);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            SceneManager.LoadScene(1);
        }


        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);

            // First, open the file stream. using() {...} ensures the stream is closed afterward.
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);

            }
        }

        

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            // First, open the file stream. using() {...} ensures the stream is closed afterward.
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        // Saves the SaveableEntity's state
        private void CaptureState(Dictionary<string, object> stateSoFar)
        {
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                stateSoFar[saveable.GetUniqueIndentifier()] = saveable.CaptureState();
            }
            stateSoFar[LAST_SCENE_INDEX] = SceneManager.GetActiveScene().buildIndex;
        }

        // Restores the SaveableEntity's state
        private void RestoreState(Dictionary<string, object> state)
        {
            Dictionary<string, object> savedState = state;
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {

                string id = saveable.GetUniqueIndentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(savedState[id]);
                }
            }

        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }

        public IEnumerator LoadLastScene(string saveFile)
        {
            
            Dictionary<string, object> savedState = LoadFile(saveFile);

            int lastSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (savedState.ContainsKey(LAST_SCENE_INDEX))
            {
                lastSceneIndex = (int)savedState[LAST_SCENE_INDEX];
            }

            yield return SceneManager.LoadSceneAsync(lastSceneIndex);
            RestoreState(savedState);
        }

    }
}


//
// For reference:
// Combine(String, String, String, ...) returns a string which uses the proper combine symbol in your system
//