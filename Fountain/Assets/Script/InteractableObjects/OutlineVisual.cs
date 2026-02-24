using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Common
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
            this.outlineVisual.enabled = visible;
        }
    }
}
