using Fountain.Common;
using Fountain.InputManagement;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 玩家枪的脚本控制类,杂糅了很多东西
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("移动设置")]
        [Tooltip("枪移动速度")]
        public float moveSpeed = 5f;
        [Tooltip("限制水枪移动范围")]
        private float screenLimitX = 8f;
        
        [Header("射击设置")]
        [SerializeField]
        [Tooltip("子弹预制件")]
        private GameObject ProjectilePrefab;
        [Tooltip("射击频率")]
        [SerializeField]
        private float fireRate = 0.3f;

        private float nextFireTime = 0f;
        private Transform firePoint;

        private GunInputProvider gunInput;
        private PlayerSightInputProvider sightInput;
        private void Start()
        {
            gunInput = GameInputManager.Instance.GetProvider<GunInputProvider>();
            sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();
            firePoint = this.transform.FindChildByName(nameof(firePoint));
            gunInput.enabled = false;
            sightInput.HideCursor();
            GameEventBus.Subscribe<ControlFountainStartEvent>((e) =>
            {
                this.gunInput.enabled = true;
            });
            GameEventBus.Subscribe<ControlFountainEndEvent>((e) =>
            {
                sightInput.ShowCursor(); 
                this.gunInput.enabled = false;
            });
        }

        private void Update()
        {
            Move();

            if (gunInput.GetFire()&& Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }

        private void Shoot()
        {
            if (ProjectilePrefab != null && firePoint != null)
            {
                GameObject go = Instantiate
                        (ProjectilePrefab, firePoint.position, Quaternion.identity);
                go.GetComponent<WaterProjectile>().direction = firePoint.up;
            }
        }
        /// <summary>
        /// 移动
        /// </summary>
        private void Move()
        {
            Vector3 moveInput = gunInput.GetMove().normalized;
            transform.Translate( moveInput * moveSpeed * Time.deltaTime);

            // 限制在屏幕内
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, -screenLimitX, screenLimitX);
            transform.position = clampedPos;
            
        }
    }
}
