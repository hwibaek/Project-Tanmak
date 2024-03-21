using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.UI
{
    public partial class UIManager : MonoBehaviour
    {
        [SerializeField] private Vector3 targetImageOffset;
        public Vector3 TargetImageOffset => targetImageOffset;

        [SerializeField] private Image targetImagePrefab;
        public Image TargetImagePrefab => targetImagePrefab;

        [SerializeField] private RectTransform targetParent;
        public RectTransform TargetParent => targetParent;

        [SerializeField] private VariableJoystick joy;
        public VariableJoystick Joy => joy;

        [SerializeField] private InputBtn jumpBtn;
        public InputBtn JumpBtn => jumpBtn;
        
        [SerializeField] private InputBtn attackBtn;
        public InputBtn AttackBtn => attackBtn;

        public List<Image> targetImages;
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

        private void Update()
        {
            var enemies = GameManager.Instance.player.Instance.GetTargetScreenPos().ToArray();
            
            if (targetImages.Count != enemies.Length)
            {
                for (var i = 0; i < targetImages.Count - enemies.Length; i++)
                {
                    var rm = targetImages[i];
                    Destroy(rm.gameObject);
                    targetImages.Remove(rm);
                }
                
                if (enemies.Length <= 0) return;
                
                for (var i = targetImages.Count; i < enemies.Length; i++)
                {
                    var img = Instantiate(TargetImagePrefab, TargetParent);
                    targetImages.Add(img);
                }
            }

            for (var i = 0; i < targetImages.Count; i++)
            {
                targetImages[i].rectTransform.anchoredPosition = enemies[i] + TargetImageOffset;
            }
        }
    }
}