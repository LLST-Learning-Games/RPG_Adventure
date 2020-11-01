using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    

    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 1f;
        float timeSinceLastSawPlayer = Mathf.Infinity;

        

        [SerializeField] PatrolPath myPatrolPath;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = .4f;
        [SerializeField] float waypointDistanceTolerance = 1f;
        [SerializeField] int startingWaypoint = 0;
        int currentWayPoint;
        [SerializeField] float waypointHesitateTime = .5f;
        float timeSinceArrivedAtWaypoint = 0;


        GameObject myPlayer;
        Fighter myFighter;
        Mover myMover;

        Vector3 guardPosition;


        private void Awake()
        {
            myPlayer = GameObject.FindWithTag("Player");
            myFighter = GetComponent<Fighter>();
            myMover = GetComponent<Mover>();
            currentWayPoint = startingWaypoint;
        }

        private void Start()
        {
            guardPosition = transform.position;     //best practice not to access transform in Awake()
        }

        private void Update()
        {

            if (GetComponent<Health>().GetIsAlive())
            {
                CheckForAttackRoutine();
            }
            else
                myFighter.Cancel();

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void CheckForAttackRoutine()
        {
            //attack state
            if (CheckDistanceToPlayer())
            {
                AttackBehaviour();
            }
            //suspicion state
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                SuspicionBehaviour();
            }
            //return to guard position state
            else
            {
                PatrolBehaviour();
            }
        }


        //==========================
        //== State logic
        //==========================

        private void AttackBehaviour()
        {
            //myMover.Cancel();
            myFighter.Attack(myPlayer.gameObject);
            timeSinceLastSawPlayer = 0;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {

            //if we don't have a patrol path, just guard
            if (myPatrolPath == null)
                myMover.StartMovementAction(guardPosition, patrolSpeedFraction);

            //if we DO have a patrol path, then patrol
            else
            {
                Vector3 nextPosition; 
                if(AtWaypoint())
                {
                    CycleWaypoint();
                    timeSinceArrivedAtWaypoint = 0;
                }

                if (timeSinceArrivedAtWaypoint > waypointHesitateTime)
                {
                    nextPosition = GetCurrentWaypoint();
                    myMover.StartMovementAction(nextPosition, patrolSpeedFraction);
                }
                else
                    timeSinceArrivedAtWaypoint += Time.deltaTime;
            }

        }
        
        //=========================
        //== Patrol Logic
        //=========================

        private bool AtWaypoint()
        {
            return (Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointDistanceTolerance);
        }

        private void CycleWaypoint()
        {
            currentWayPoint = myPatrolPath.GetNextIndex(currentWayPoint);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return myPatrolPath.GetWaypoint(currentWayPoint);
        }

        private bool CheckDistanceToPlayer()
        {
            float distToPlayer = Vector3.Distance(myPlayer.transform.position, transform.position);

            return (distToPlayer <= chaseDistance);
        }




        //Called by the Unity Editor
        private void OnDrawGizmosSelected()
        {
            //Set the Gizmo colour to blue
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}