using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Player.Weapon
{
    public abstract class WeaponBase : ScriptableObject
    {
        [SerializeField] private string weaponName;
        public string WeaponName => weaponName;

        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;

        [SerializeField, Min(0)] private int targetCount;
        public int TargetCount => targetCount;

        [SerializeField] private int damage;
        public int Damage => damage;

        public abstract UniTask Attack(PlayerController controller);
    }
}