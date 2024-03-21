using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Player.Weapon
{
    [CreateAssetMenu]
    public class Bash : WeaponBase
    {
        [SerializeField] private float dashPower;
        public float DashPower => dashPower;
        
        public override async UniTask Attack(PlayerController controller)
        {
            var target = controller.GetTarget().First();
            var dir = target.transform.position - controller.transform.position;
            var distance = dir.magnitude;
            var time = distance / DashPower;
            controller.moveLock = true;
            controller.ImmediateRotate(dir.normalized.x, dir.normalized.z);
            var timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;
                controller.Cc.Move(dir.normalized * (DashPower * Time.deltaTime));
                await UniTask.WaitForSeconds(Time.deltaTime);
            }
            controller.moveLock = false;
        }
    }
}