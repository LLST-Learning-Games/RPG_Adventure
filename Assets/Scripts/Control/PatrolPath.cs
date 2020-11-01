using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{



    public class PatrolPath : MonoBehaviour
    {
        
        const float waypointGizmoRadius = 0.3f;


        

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

        public int GetNextIndex(int i)
        {
            if (i < transform.childCount-1)
            {
                return i + 1;
            }
            else return 0;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)));

            }

        }
    }

}