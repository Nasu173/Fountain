using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        [SerializeField]
        private Vector3 targetRotation;
        //npc到达指定地点调用
        public event Action Arrived;
        private float elapsed;
        private bool doneRotation;
        public string walkName;
        public Animator anim;
        //移动到指定地点
        private void Start()
        {
            this.enabled = false;
        }
        private void Update()
        {

            if (elapsed>duration)
            {
                if (!doneRotation)
                {
                    StartCoroutine(Move());
                    anim.SetBool(walkName, true);
                }
                doneRotation = true;
                return;
            }
            elapsed += Time.deltaTime;
            this.transform.rotation= Quaternion.Lerp
                (this.transform.rotation, Quaternion.Euler(targetRotation), elapsed / duration);

            
        }
        public void MoveToward(Vector3 position)
        {
            this.enabled = true;
            doneRotation = false;
            elapsed = 0;
            this.targetPosition = position;
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
            Arrived?.Invoke();
            this.anim.SetBool(walkName, false);

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
