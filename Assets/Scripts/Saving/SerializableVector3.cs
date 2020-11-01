using UnityEngine;


namespace RPG.Saving
{
    [System.Serializable]
    public class SerializableVector3
    {
        float x, y, z;

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public Vector3 DeserializeVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}