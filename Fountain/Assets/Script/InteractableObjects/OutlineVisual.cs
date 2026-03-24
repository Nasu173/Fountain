using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Common
{
    /// <summary>
    /// 描边效果
    /// </summary>
    public class OutlineVisual : MonoBehaviour
    {
        [Tooltip("实现描边的插件,手动拖上去")]
        [SerializeField]
        private Outline outlineVisual;
        /// <summary>
        /// 设置描边的显隐
        /// </summary>
        /// <param name="visible">显示或隐藏</param>
        public void SetOutline(bool visible)
        {
            //糊上去的代码,最好不出现这个判断
            if (outlineVisual==null)
            {
                return;
            }
            this.outlineVisual.enabled = visible;
        }
    }
}
