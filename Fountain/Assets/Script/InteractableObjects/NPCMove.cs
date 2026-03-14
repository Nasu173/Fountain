using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// npc移动的脚本,似乎不用什么移动方式,就单纯的平移即可,否则要重构一下PlayerMove
    /// </summary>
    public class NPCMove : MonoBehaviour
    {
        [Tooltip("移动速度")]
        [SerializeField]
        private float speed;
        private Vector3 targetPosition;
        
        //移动到指定地点
        public void MoveToward(Vector3 position)
        {
            this.targetPosition = position;
            StartCoroutine(Move());
        }
        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }
        private IEnumerator Move()
        {
            float acceptableDelta = 0.01f;
            Vector3 startPosition = this.transform.position;
            float t = 0;
            while (Vector3.Distance(this.transform.position,targetPosition)>acceptableDelta)
            {
                this.transform.LookAt(targetPosition);
                t += Time.deltaTime * speed;
                this.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        }
    }
}
