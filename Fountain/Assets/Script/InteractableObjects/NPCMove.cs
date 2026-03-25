using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// npc移动的脚本,似乎不用什么移动方式,就单纯的平移即可,否则要重构一下PlayerMove
    /// </summary>
    public class NPCMove : MonoBehaviour
    {
        [Tooltip("移动总时间")]
        [SerializeField]
        private float duration;
        private Vector3 targetPosition;
        
        //移动到指定地点
        public void MoveToward(Vector3 position)
        {
            this.targetPosition = position;
            StartCoroutine(Move());
        }
        public void SetDuration(float duration)
        {
            this.duration = duration;
        }
        private IEnumerator Move()
        {
            float elapsed = 0;
            Vector3 startPosition = this.transform.position;
            while (elapsed<duration)
            {
                float t = elapsed / duration;
                this.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            /*
            float acceptableDelta = 0.01f;
            Vector3 startPosition = this.transform.position;
            float t = 0;
            while (Vector3.Distance(this.transform.position,targetPosition)>acceptableDelta)
            {
                this.transform.LookAt(targetPosition);
                t += Time.deltaTime * duration;
                this.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
             
             */
        }
    }
}
