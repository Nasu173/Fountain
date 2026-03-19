using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 子弹的移动脚本
    /// </summary>
    public class WaterProjectile : MonoBehaviour
    {
        public float speed = 10f;
        public float maxHeight = 6f; // 到达该高度后消失
        [HideInInspector]
        public Vector3 direction;
        public string targerTag;
        void Update()
        {
            // 水柱向上移动
            transform.Translate(direction * speed * Time.deltaTime);
    
            // 到达一定高度消失
            if (transform.position.y >= maxHeight)
            {
                Destroy(gameObject);
            }
        }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            // 碰到路人则消失
            if (collision.CompareTag(targerTag))
            {
                Pedestrian p = collision.GetComponent<Pedestrian>();
                if (p != null)
                {
                    p.GetHit();
                }
                Destroy(gameObject); // 水柱自身也消失
            }
        }
    }
    
}
