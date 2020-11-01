using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using RPG.Core;
using System.Collections.Generic;
using System;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        const string UNSET_KEY = "(UNSET)";
        [SerializeField] string myUUID = UNSET_KEY;
        static Dictionary<string, SaveableEntity> globalSaveUUIDLookup = new Dictionary<string, SaveableEntity>();


#if UNITY_EDITOR
        private void Update()
        {
            // If the application is running (ie, not in editor) then return
            if (Application.IsPlaying(gameObject)) return;

            // This snippet ensures that we don't assign a UUID if we are on a prefab
            // Basically, if you're not in a scene, the path is null... or maybe empty
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            // This madness here is the way we convince Unity to assign a UUID once and only once.
            // Things I don't understand: does this persist across scenes? When we open & close unity? 
            // When does this actually get executed? Where is it saved?
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("myUUID");

            //print("in edit mode");
            if (property.stringValue == UNSET_KEY || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalSaveUUIDLookup[property.stringValue] = this;
        }
#endif


        public string GetUniqueIndentifier()
        {
            return myUUID;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if(stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
            

        }

        private bool IsUnique(string candidateForUniqueness)
        {
            if (!globalSaveUUIDLookup.ContainsKey(candidateForUniqueness)) return true;

            if(globalSaveUUIDLookup[candidateForUniqueness] == this) return true;

            if(globalSaveUUIDLookup[candidateForUniqueness] == null)
            {
                globalSaveUUIDLookup.Remove(candidateForUniqueness);
                return true;
            }

            if (globalSaveUUIDLookup[candidateForUniqueness].GetUniqueIndentifier() != candidateForUniqueness)
            {
                globalSaveUUIDLookup.Remove(candidateForUniqueness);
                return true;
            }

            return false;
        }

    }
}