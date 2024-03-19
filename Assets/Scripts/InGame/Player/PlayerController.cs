using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Player
{
    public partial class PlayerController : MonoBehaviour
    {
        
    }

    public partial class PlayerController
    {
        [SerializeField] private int currentJumpCnt;
        [SerializeField] private float coyoteTime;
        [SerializeField] private float jumpBufferCnt;
        [SerializeField] private bool fallChecker;

        public bool canDash;
        private bool dashing;
        
        [SerializeField] private float dashPower;
        [SerializeField] private float dashDistance;

        public bool moveLock;
        
        private float _targetRot;
        private float _rotV;
        private float _vv;

        [SerializeField] private float speed;

        [SerializeField] private CharacterController cc;
        public CharacterController Cc => cc;

        [SerializeField] private Transform body;
        public Transform Body => body;
        
        [SerializeField] private Transform camArm;
        public Transform CamArm => camArm;
    }

    public partial class PlayerController
    {
        private void Rotate(float x, float y)
        {
            _targetRot = Mathf.Atan2(x, y) * Mathf.Rad2Deg + CamArm.transform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(Body.eulerAngles.y, _targetRot, ref _rotV,
                0.05f);
            Body.rotation = Quaternion.Euler(0, rotation, 0);
        }
        private void Move()
        {
            if (moveLock) return;
            var targetSpeed = GameManager.Instance.uim.Instance.Joy.Direction.magnitude != 0 ? GameManager.Instance.uim.Instance.Joy.Direction.magnitude * speed : 0;
            if (targetSpeed != 0)
            {
                Rotate(GameManager.Instance.uim.Instance.Joy.Direction.x, GameManager.Instance.uim.Instance.Joy.Direction.y);
            }
            var targetDir = Quaternion.Euler(0, _targetRot, 0) * Vector3.forward;
            Cc.Move(targetDir * (targetSpeed * Time.fixedDeltaTime) + new Vector3(0, _vv * Time.fixedDeltaTime, 0));
        }
        
        private void GroundCheck()
        {
            if (!Cc.isGrounded)
            {
                if (coyoteTime > 0)
                {
                    coyoteTime -= Time.deltaTime;
                }
                else if (!fallChecker && currentJumpCnt <= 0)
                {
                    fallChecker = true;
                    currentJumpCnt++;
                }
                _vv += Physics.gravity.y * Time.deltaTime * 2;
                return;
            }

            canDash = true;
            coyoteTime = GameManager.Instance.CoyoteTime;
            currentJumpCnt = 0;
            fallChecker = false;
            _vv = Mathf.Max(_vv, 0);
        }

        private void Jump()
        {
            if (GameManager.Instance.uim.Instance.JumpBtn.onPointerDown)
            {
                jumpBufferCnt = GameManager.Instance.JumpBufferTime;
            }
            else if (jumpBufferCnt > 0)
            {
                jumpBufferCnt -= Time.deltaTime;
            }
            
            if (jumpBufferCnt > 0 && currentJumpCnt < 2)
            {
                currentJumpCnt++;
                _vv = 15;
                jumpBufferCnt = 0;
            }

            if (GameManager.Instance.uim.Instance.JumpBtn.onPointerUp)
            {
                coyoteTime = GameManager.Instance.CoyoteTime;
                if (Cc.velocity.y > 0)
                {
                    _vv *= 0.5f;
                }
            }
        }

        public async void DashAsync()
        {
            if (!canDash || dashing || moveLock) return;
            canDash = false;
            moveLock = true;
            _vv = 0;
            dashing = true;
            var dir = Body.forward.normalized;
            var dashTime = dashDistance / (dashPower * speed);
            var timer = 0f;
            while (timer < dashTime)
            {
                timer += Time.deltaTime;
                Cc.Move(dir * (dashPower * speed * dashDistance * Time.deltaTime));
                await UniTask.WaitForSeconds(Time.deltaTime);
            }
            dashing = false;
            _vv = 0;
            moveLock = false;
        }
    }

    public partial class PlayerController
    {
        private void Start()
        {
            GameManager.Instance.player.Instance = this;
        }

        private void Update()
        {
            GroundCheck();
        }

        private void FixedUpdate()
        {
            Move();
            Jump();
        }
    }
}
