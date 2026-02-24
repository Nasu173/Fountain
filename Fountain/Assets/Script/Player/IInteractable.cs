using System;

namespace Foutain.Player
{
    /// <summary>
    /// 可交互物体的接口,所有可交互物体必须实现这个接口
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 与玩家交互
        /// </summary>
        /// <param name="player"></param>
        public void InteractWith(PlayerInteractor player);
        /// <summary>
        /// 被玩家选中时调用该函数
        /// </summary>
        public void Select();//不考虑为所有交互物体提供高亮显示的基础实现,各个脚本自己实现
        /// <summary>
        ///  玩家取消选中时调用该函数
        /// </summary>
        public void Deselect();
    }
}
