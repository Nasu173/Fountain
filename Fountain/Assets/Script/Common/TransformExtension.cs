using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Common
{
    /// <summary>
    /// Transform扩展类,提供一些有用的方法
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// 未知层级查找后代物体的Transform组件,不要频繁调用
        /// </summary>
        /// <param name="fatherTF"></param>
        /// <param name="childName">子物体的名称,必须完全一致</param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform fatherTF,  string childName)
        {
            //递归查找
            Transform childTF=fatherTF.Find(childName);
            if(childTF ==null)
            {
                for (int i = 0; i < fatherTF.childCount; i++)
                {
                   childTF= FindChildByName(fatherTF.GetChild(i), childName);
                    if (childTF != null)
                    {
                        break;
                    }
                }
            }
            return childTF;
        }

        /*废弃的方法
         
        /// <summary>
        /// 注视方向旋转渐变
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="direction"></param>
        /// <param name="rotateSpeed"></param>
        public static void LookAtDirectionLerp(this Transform currentTF,Vector3 direction,float rotateSpeed)
        {
            if (direction==Vector3.zero )
            {
                return;
            }
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            if (Quaternion.Angle(lookRotation ,currentTF .rotation )<0.5F)
            {
                currentTF.rotation = lookRotation;
                return;
            }
            currentTF.rotation = Quaternion.Lerp
                (currentTF.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
        /// <summary>
        /// 注视位置旋转渐变
        /// </summary>
        /// <param name="currentTF"></param>
        /// <param name="position"></param>
        /// <param name="rotateSpeed"></param>
        public static void LookAtPositionLerp(this Transform currentTF,Vector3 position,float rotateSpeed)
        {
            currentTF.LookAtDirectionLerp(position - currentTF.position, rotateSpeed);
        }

         */
    }
}