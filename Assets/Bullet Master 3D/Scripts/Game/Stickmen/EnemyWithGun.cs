using System.Collections;
using Bullet_Master_3D.Scripts.Singleton;
using UnityEngine;

namespace Bullet_Master_3D.Scripts.Game
{
    public class EnemyWithGun : Stickman
    {
        [Header("WEAPON")]
        [Tooltip("Type of weapon used by bot")] 
        [SerializeField] private WeaponType weaponTypeType;
        [Tooltip("Link to weapon in GunParent")] 
        [SerializeField] private Weapon _weapon;
        [Tooltip("Number of cartridges that a bot can fire")] 
        [SerializeField][Range(1, Constants.MAXIMUM_CARTRIDGES)] private int _cartridgesCount;

        [Header("AIMING")]
        [Tooltip("Turning speed towards player")]
        [SerializeField][Range(0f, 250f)] private float _rotationSpeed;
        [Tooltip("Angle of turn adjustment to player")]
        [SerializeField][Range(0f, 15f)] private float _correctionAngle;
        [Tooltip("Shooting scatter angle")]
        [SerializeField][Range(0f, 60f)] private float _maxScatterAngle;
        [Tooltip("The time bot takes to aim")]
        [SerializeField][Range(0f, 3f)] private float _aimingTime;
        
        private static readonly int WEAPON_ID_KEY = Animator.StringToHash("WeaponId");
        private static readonly int SHOOT_KEY = Animator.StringToHash("Shoot");
        private const float ANGLE_ROUND = 1f;
        
        private Player _player;

         private void Start()
         {
             Setup();
             
            //Setup the weapon
            _weapon.CartridgesCount = _cartridgesCount;
            _weapon.IsBotWeapon = true;
            //Subscribe to action to start shooting after the player’s first shot
            _player = Boostrap.Instance.GameManager.PlayerSpawnerService.Player;
            _player.OnFirstShoot += () => { StartCoroutine(Shooting()); };
            //Set animation for current weapon type
            Animator.SetInteger(WEAPON_ID_KEY, (int)weaponTypeType);
        }

        private IEnumerator Shooting()
        {
            //Fire as long as these conditions are met
            while (Boostrap.Instance.GameState == GameStates.InProgress && IsAlive && _weapon.HaveCartridges())
            {
                var rotation = CalculateAngleToTarget();
                
                while (Quaternion.Angle(transform.rotation, rotation) > ANGLE_ROUND && IsAlive)
                {
                    //Rotate enemy to calculated angle
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotationSpeed * Time.fixedDeltaTime);
                    //Draw laser sight
                    _weapon.ShowLaser();
                    yield return new WaitForFixedUpdate();
                }

                if (!IsAlive) break;
                //Wait aiming time
                yield return new WaitForSeconds(_aimingTime);
                _weapon.Shoot(OnShoot);
            } 
            //When the shooting’s over, turn off the laser
            _weapon.HideLaser();
        }

        private Quaternion CalculateAngleToTarget()
        {
            //Get turning corners to player
            var eulerAngles = LookAt(_player.transform.position).eulerAngles;
            var maxScatterAngle = _maxScatterAngle *  0.5f;
            //Use a corrective angle to aim directly at the player and a random variation angle to allow for scatter when shooting
            eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y - _correctionAngle - Random.Range(-maxScatterAngle, maxScatterAngle), eulerAngles.z);
            //Convert to quaternion
            return Quaternion.Euler(eulerAngles);
        }

        private void OnShoot(int _)
        {
            //When fired, animate the shooting
            Animator.SetTrigger(SHOOT_KEY);
        }
    }
}