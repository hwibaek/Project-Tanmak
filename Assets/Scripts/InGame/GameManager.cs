using InGame.Player;
using InGame.UI;
using UnityEngine;
using Util;

namespace InGame
{
    #region 싱글톤
    public partial class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get => _instance;
            private set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
                else if (_instance != value)
                {
                    Destroy(value.gameObject);
                }
            }
        }
    }
    #endregion

    #region 인스턴스

    public partial class GameManager
    {
        public Singleton<PlayerController> player = new();
        public Singleton<UIManager> uim = new();
    }

    #endregion

    #region 변수 모음

    public partial class GameManager
    {
        [SerializeField] private float coyoteTime;
        public float CoyoteTime => coyoteTime;
        
        [SerializeField] private float jumpBufferTime;
        public float JumpBufferTime => jumpBufferTime;
    }

    #endregion

    #region 이벤트 함수 모음

    public partial class GameManager
    {
        private void Awake()
        {
            Instance = this;
        }
    }

    #endregion
}