using System;
using UnityEngine;

namespace Util
{
    [Serializable]
    public class Singleton<T> where T : MonoBehaviour
    {
        private T _instance;

        public T Instance
        {
            get => _instance;
            set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
                else
                {
                    Debug.Assert(_instance != value, $"Error : Instance has been exist! {_instance.gameObject.name} => {value.gameObject.name}");
                }
            }
        }

        public static implicit operator T(Singleton<T> singleton) => singleton.Instance;
        public static implicit operator bool(Singleton<T> singleton) => singleton.Instance != null;
    }
}