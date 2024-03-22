using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InGame.Player.Weapon;
using UnityEngine;

namespace InGame.Player
{
    public partial class PlayerController : MonoBehaviour
    {
        
    }

    public partial class PlayerController
    {
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        
        [SerializeField] private float coyoteTime;
        [SerializeField] private float jumpBufferTime;
        [SerializeField] private bool fallChecker;

        public bool canDash;
        private bool dashing;
        
        [SerializeField] private float dashPower;
        [SerializeField] private float dashDistance;

        public bool moveLock;
        
        private float _targetRot;
        private float _rotV;
        private float _vv;

        private bool attacking;

        [SerializeField] private float speed;

        [SerializeField] private WeaponBase weapon;
        public WeaponBase Weapon => weapon;

        [SerializeField] private CharacterController cc;
        public CharacterController Cc => cc;
        
        [SerializeField] private Animator anim;
        public Animator Anim => anim;

        [SerializeField] private Transform body;
        public Transform Body => body;
        
        [SerializeField] private Transform camArm;
        public Transform CamArm => camArm;
    }

    public partial class PlayerController
    {
        public void Rotate(float x, float y, float smoothTime = 0.05f)
        {
            _targetRot = Mathf.Atan2(x, y) * Mathf.Rad2Deg + CamArm.transform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(Body.eulerAngles.y, _targetRot, ref _rotV,
                smoothTime);
            Body.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public void ImmediateRotate(float x, float y)
        {
            Rotate(x, y, 0);
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
            Anim.SetFloat(_animIDSpeed, targetSpeed);
        }

        private bool IsGround()
        {
            var spherePosition = transform.position;
            var grounded = Physics.CheckSphere(spherePosition, 0.28f, GameManager.Instance.GroundLayer,
                QueryTriggerInteraction.Ignore);
            return grounded;
        }
        
        private void GroundCheck()
        {
            var grounded = IsGround();
            
            Anim.SetBool(_animIDGrounded, grounded);
            
            if (!grounded)
            {
                if (coyoteTime > 0)
                {
                    coyoteTime -= Time.deltaTime;
                }
                else if (!fallChecker)
                {
                    fallChecker = true;
                }
                _vv += Physics.gravity.y * Time.deltaTime * 2;
                Anim.SetBool(_animIDFreeFall, true);
                return;
            }
            
            Anim.SetBool(_animIDJump, false);
            Anim.SetBool(_animIDFreeFall, false);

            canDash = true;
            coyoteTime = GameManager.Instance.CoyoteTime;
            fallChecker = false;
            _vv = Mathf.Max(_vv, 0);
        }

        private void Jump()
        {
            if (moveLock) return;
            if (GameManager.Instance.uim.Instance.JumpBtn.onPointerDown)
            {
                jumpBufferTime = GameManager.Instance.JumpBufferTime;
            }
            else if (jumpBufferTime > 0)
            {
                jumpBufferTime -= Time.deltaTime;
            }
            
            if (jumpBufferTime > 0 && IsGround())
            {
                _vv = 15;
                jumpBufferTime = 0;
                Anim.SetBool(_animIDJump, true);
            }

            if (GameManager.Instance.uim.Instance.JumpBtn.onPointerUp && Cc.velocity.y > 0)
            {
                coyoteTime = 0;
                _vv *= 0.5f;
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
                Cc.Move(dir * (dashPower * speed * Time.deltaTime));
                await UniTask.WaitForSeconds(Time.deltaTime);
            }
            dashing = false;
            _vv = 0;
            moveLock = false;
        }

        private void Attack()
        {
            if (!GameManager.Instance.uim.Instance.AttackBtn.onPointerDown || attacking || GetTarget().ToList().Count <= 0) return;
            AttackAsync();
        }
        public async void AttackAsync()
        {
            attacking = true;
            await Weapon.Attack(this);
            attacking = false;
        }

        public IEnumerable<Collider> GetTarget() =>
            Physics.OverlapSphere(transform.position, weapon.AttackRange, GameManager.Instance.EnemyLayer, QueryTriggerInteraction.Ignore)
                .OrderBy(col => Vector3.Distance(transform.position, col.transform.position))
                .Take(Weapon.TargetCount);

        public IEnumerable<Vector3> GetTargetScreenPos() => GetTarget().Select(col => GameManager.Instance.MainCam.WorldToScreenPoint(col.transform.position));
    }

    public partial class PlayerController
    {
        private void Start()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            
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
            Attack();
        }
    }
}
