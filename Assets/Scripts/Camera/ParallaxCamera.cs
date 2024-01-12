using System;
using UnityEngine;

    public class ParallaxCamera : MonoBehaviour
    {
        public delegate void ParallaxCameraDelegate(float deltaMovement);
        public ParallaxCameraDelegate onCameraTranslate;
     
        private float oldPosition;
     
        void Start()
        {
            oldPosition = transform.position.y;
        }
     
        void FixedUpdate()
        {
            if (Math.Abs(transform.position.y - oldPosition) > 0.001f) //if there is minimal change
            {
                var delta = oldPosition - transform.position.y;
                onCameraTranslate?.Invoke(delta);
     
                oldPosition = transform.position.y;
            }
        }
    }