using Game.Systems;
using Game.SystemsManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class CameraSwipeRotator : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private float _sensetivity = 0.1f;
        private CameraSystem _cameraSystem;
        private Vector2 _previousFrameMousePosition;
        private Vector2 _currentMousePosition;
        Vector2 _delta;

        private void Awake()
        {
            // Ensure the script is attached to a UI Image
            if (GetComponent<Image>() == null)
            {
                Debug.LogError("SwipeDetector must be attached to a UI Image.");
                return;
            }
            _cameraSystem = SystemsManager.GetSystemOfType<CameraSystem>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _previousFrameMousePosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Calculate the horizontal delta of the swipe
            _currentMousePosition = eventData.position;
            _delta = _currentMousePosition - _previousFrameMousePosition;
            _previousFrameMousePosition = _currentMousePosition;
            _cameraSystem.RotateAroundTarget(_delta.x * _sensetivity);
            _cameraSystem.RotateVertically(-_delta.y * _sensetivity);
        }
    }
}
