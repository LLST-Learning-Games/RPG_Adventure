using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour
    {

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            internal Vector2 hotspot;
        }




        Health health;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDist = 1.0f;
        [SerializeField] float maxNavPathLength = 30.0f;


        private void Awake()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (InteractWithUI()) return;
            if (Input.GetKeyDown(KeyCode.H))
            {
                health.HealMe(50);
                Debug.Log("H key pressed");
            }

            if (health.GetIsAlive())
            {
                if (InteractWithComponent()) return;
                if (InteractWithMovement()) return;
            }

            SetCursor(CursorType.None);



        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = SortRaycastHits();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        //This function ensures that the raycast hits are sorted by distance
        private RaycastHit[] SortRaycastHits()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            float[] distances = new float[hits.Length];

            for(int i = 0; i <distances.Length; i++)
            {
                distances[i] = hits[i].distance;

            }

            //This function sorts the 2nd parameter using the first as an index.
            Array.Sort(distances, hits);

            return hits;
        }


        private bool InteractWithUI()
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        

        private bool InteractWithMovement()
        {
            
            Vector3 target;

            bool hasHit = RaycastNavMesh(out target);


            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    SetCursor(CursorType.MovementClick);
                    GetComponent<Mover>().StartMovementAction(target, 1f);
                }
                else SetCursor(CursorType.Movement);

                return true;
            }
            return false;
        }


        private bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit hit;
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDist, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);

            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;

            if (GetPathLength(path) > maxNavPathLength) return false;

            return hasHit;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float distance = 0.0f;

            Vector3 lastCorner = path.corners[0];
            for (int i = 1; i < path.corners.Length; i++)
            {
                distance += Vector3.Distance(lastCorner, path.corners[i]);
                lastCorner = path.corners[i];
            }

            return distance;
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = SetCursorType(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping SetCursorType(CursorType cursorType)
        {
            foreach (CursorMapping item in cursorMappings)
            {
                if (item.type == cursorType)
                {
                    return item;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

