using UnityEngine;

namespace Platformer.View
{
    public class ParallaxLayer : MonoBehaviour
    {
        public Vector3 movementScale = Vector3.one;

        private Transform _camera;
        private Vector3 _originalPosition;

        void Awake()
        {
            _camera = Camera.main.transform;
            _originalPosition = transform.position; // Save the original position
        }

        void LateUpdate()
        {
            Vector3 newPosition = Vector3.Scale(_camera.position, movementScale) + _originalPosition;
            transform.position = newPosition;
        }
    }
}
