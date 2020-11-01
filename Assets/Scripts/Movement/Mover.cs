using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{

    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6;
        NavMeshAgent myNavMeshAgent;
        Health myHeatlh; 

        private void Awake()
        {
            myNavMeshAgent = GetComponent<NavMeshAgent>();
            myHeatlh = GetComponent<Health>();
        }

        void Update()
        {
            myNavMeshAgent.enabled = myHeatlh.GetIsAlive();
            UpdateAnimator();
        }

        public void StartMovementAction(Vector3 myDestination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(myDestination, speedFraction);
        }

        public void MoveTo(Vector3 myDestination, float speedFraction)
        {
            myNavMeshAgent.isStopped = false;
            myNavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            myNavMeshAgent.destination = myDestination;
        }

        public void Cancel()
        {
            if (myHeatlh.GetIsAlive() == true)
            {
                myNavMeshAgent.isStopped = true;
            }
            //print("Cancelling " + this);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            GetComponent<Animator>().SetFloat("forwardSpeed", speed);

        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.DeserializeVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}