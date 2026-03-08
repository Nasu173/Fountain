using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Dialogue
{
    /// <summary>
    /// 为对话演出提供数据的接口,它的子类获取数据供DialoguePerform的子类使用,
    /// 实现类自行做强制类型转换
    /// </summary>
    public interface IPerformDataProvider 
    {
        /// <summary>
        /// 返回的数字说明这份数据是用于哪个DialogueNode的演出
        /// </summary>
        /// <returns></returns>
        public int GetTargetIndex();
    }
}
