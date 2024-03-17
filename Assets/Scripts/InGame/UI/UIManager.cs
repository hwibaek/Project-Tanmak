using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.UI
{
    public partial class UIManager : MonoBehaviour
    {
        [SerializeField] private VariableJoystick joy;
        public VariableJoystick Joy => joy;

        [SerializeField] private InputBtn jumpBtn;
        public InputBtn JumpBtn => jumpBtn;
        
        [SerializeField] private InputBtn attackBtn;
        public InputBtn AttackBtn => attackBtn;
    }

    public partial class UIManager
    {
        private void Start()
        {
            RegisterPlayer();
            GameManager.Instance.uim.Instance = this;
        }

        private async void RegisterPlayer()
        {
            await UniTask.WaitUntil(() => GameManager.Instance.player);
            JumpBtn.onDoubleClick += GameManager.Instance.player.Instance.DashAsync;
        }
    }
}