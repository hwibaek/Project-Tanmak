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
        public bool onPointer;
        public Action onDoubleClick;
        
        public async void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                onPointerDown = false;
                onPointerUp = false;
                Debug.Log("두번 누름");
                return;
            }
            onPointerDown = true;
            onPointer = true;
            await UniTask.Yield();
            onPointerDown = false;
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                onPointerDown = false;
                onPointerUp = false;
                Debug.Log("두번 땜");
                return;
            }
            onPointerUp = true;
            onPointer = false;
            await UniTask.Yield();
            onPointerUp = false;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount != 2) return;
            onPointer = false;
            onPointerUp = false;
            onPointerDown = false;
            Debug.Log("따따블");
            onDoubleClick?.Invoke();
        }
    }
}