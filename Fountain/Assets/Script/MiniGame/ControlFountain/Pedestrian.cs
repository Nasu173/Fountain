using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 路人控制脚本,杂糅了功能
    /// </summary>
    public class Pedestrian : MonoBehaviour
    {
        [SerializeField] private AudioClip hitClip;

        [Header("路人相关设置")]
        [Tooltip("速度数值")]
        public float speed;
        [Tooltip("速度数值")]
        [SerializeField]
        private float limitX; // 走到边缘会折返

        private int direction = 1; // 1代表向右, -1代表向左
        private bool isHit = false;
    
        void Start()
        {
            // 随机初始移动方向
            direction = Random.Range(0, 2) >= 1 ? -1 : 1;
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<ControlFountainEndEvent>(Stop);
            GameEventBus.Subscribe<ControlFountainEndEvent>(DelayDie);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(Stop);
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(DelayDie);
        }

        private void Update()
        {
            //if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
            if (isHit) return;
    
            // 获取当前由GameManager控制的逐渐递增的速度
            //currentSpeed = GameManager.Instance.GetCurrentPedestrianSpeed();
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
    
            // 在屏幕边缘折返
            if (transform.position.x >= limitX)
            {
                direction = -1;
            }
            else if (transform.position.x <= -limitX)
            {
                direction = 1;
            }
        }
    
        public void GetHit()
        {
            GameEventBus.Publish(new PlaySoundEvent
            {
                Clip = hitClip,
                Track = AudioTrack.Other
            });

            isHit = true;
            // if (GameManager.Instance != null)
            // {
            //     GameManager.Instance.AddScore(1);
            // }

            // 被击中后消失
            GameEventBus.Publish<ControlFountainHit>(null);
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(DelayDie);
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(Stop);

            Destroy(gameObject);
        }
        private void Stop(ControlFountainEndEvent e)
        {
            this.enabled = false;
        }
        /// <summary>
        /// 由于某些已知原因,不得不在场景切换的时候手动销毁这个物体
        /// </summary>
        private void DelayDie(ControlFountainEndEvent e)
        {
            Destroy(this.gameObject, 5f); //这个5只用比切回办公室的延迟长就行了
        }
    }
}
