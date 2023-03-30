using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    [System.Serializable]
    public class Blackboard
    {
        public Vector3 moveToPosition;

        [Space, Header("Detection")]
        public GameObject target;
        [HideInInspector] public bool isDetected;
    }
}