using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame.UI
{
    public class InputBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public bool onPointerDown;
        public bool onPointerUp;
        public Action onDoubleClick;
        
        public async void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                onPointerDown = false;
                onPointerUp = false;
                return;
            }
            onPointerDown = true;
            await UniTask.Yield();
            onPointerDown = false;
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                onPointerDown = false;
                onPointerUp = false;
                return;
            }
            onPointerUp = true;
            await UniTask.Yield();
            onPointerUp = false;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;
            onPointerUp = false;
            onPointerDown = false;
            onDoubleClick?.Invoke();
        }
    }
}