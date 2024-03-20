using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.UI
{
    public class InputBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool onPointerDown;
        public bool onPointerUp;
        public Action onDoubleClick;

        public float delay;
        
        private bool _touched;
        
        public async void OnPointerDown(PointerEventData eventData)
        {
            if (_touched)
            {
                _touched = false;
                onDoubleClick?.Invoke();
                return;
            }

            Delay().Forget();
            _touched = true;
            onPointerDown = true;
            onPointerUp = false;
            await UniTask.Yield();
            onPointerDown = false;
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            onPointerDown = false;
            onPointerUp = true;
            await UniTask.Yield();
            onPointerUp = false;
        }

        private async UniTaskVoid Delay()
        {
            await UniTask.WaitForSeconds(delay);
            _touched = false;
        }
    }
}