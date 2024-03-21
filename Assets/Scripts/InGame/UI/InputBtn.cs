using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.UI
{
    public class InputBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public Action onDoubleClick;

        [SerializeField] private bool enableDoubleClick;

        public bool onPointer;
        public bool onPointerDown;
        public bool onPointerUp;
        [SerializeField] private float doubleClickTime = 0.5f;

        private bool _isClicked;

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointer = true;
            onPointerDown = true;
            ResetPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointer = false;
            onPointerUp = true;
            ResetPointerUp();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!enableDoubleClick) return;
            if (_isClicked)
            {
                onDoubleClick?.Invoke();
                _isClicked = false;
            }
            else
            {
                _isClicked = true;
                ResetClicked();
            }
        }

        private async void ResetClicked()
        {
            await UniTask.WaitForSeconds(doubleClickTime);
            _isClicked = false;
        }

        private async void ResetPointerDown()
        {
            await UniTask.Yield();
            onPointerDown = false;
        }

        private async void ResetPointerUp()
        {
            await UniTask.Yield();
            onPointerUp = false; 
        }
    }
}