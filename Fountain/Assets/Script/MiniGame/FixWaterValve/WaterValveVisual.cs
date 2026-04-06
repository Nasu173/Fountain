using Fountain.Common;
using Fountain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.MiniGame.RepairWaterValve
{
    /// <summary>
    /// 显示水阀的视觉效果:描边和进度条
    /// </summary>
    public class WaterValveVisual : MonoBehaviour
    {
        private OutlineVisual outline;
        private ProgressBar bar;
        private WaterValveController valve;
        [Tooltip("阀杆旋转速度")]
        [SerializeField]
        private float rotateSpeed;
        private Transform stem;//阀杆
        private FadeEffect barFade;
        private bool barShown;//是否显示了进度条
        private void Start()
        {
            bar = this.transform.FindChildByName(nameof(bar)).GetComponent<ProgressBar>();
            barFade = bar.GetComponent<FadeEffect>();
            outline = this.transform.GetComponent<OutlineVisual>();
            valve = this.GetComponent<WaterValveController>();
            valve.ProgressIncrease += () =>
            {
                float amount = valve.GetRepairedPercentage();
                bar.SetFillAmount(amount);
                RotateValve(1);
                if (amount!=0&&!barShown)
                {
                    ShowBar();
                }
            };
            valve.ProgressDecrease += () =>
            {
                float amount = valve.GetRepairedPercentage();
                bar.SetFillAmount(amount);
                RotateValve(-1);
                if (amount<=0)
                {
                    ForceHideBar();
                }
            };
            valve.RepairComplete += ForceHideBar;
            HideBar();
            stem = this.transform.FindChildByName(nameof(stem)).GetChild(0);
        }

        /// <summary>
        /// 显示进度条
        /// </summary>
        public void ShowBar()
        {
            if (!barShown)
            {
                barFade.FadeIn();
                barShown = true;
            }
            //bar.gameObject.SetActive(true);
        }
        /// <summary>
        /// 隐藏进度条
        /// </summary>
        public void HideBar()
        {
            float progress = valve.GetRepairedPercentage();
            //如果没有进度,不用动
            if (progress==0||progress==1)
            {
                return;
            }
            barShown = false;
            barFade.FadeOut();
        }
        private void ForceHideBar()
        {
            barShown = false;
            barFade.FadeOut();
        }
        
        /// <summary>
        /// 隐藏描边 
        /// </summary>
        public void ShowOutline()
        {
            outline.SetOutline(true);
        }
        /// <summary>
        /// 显示描边
        /// </summary>
        public void HideOutline()
        {
            outline.SetOutline(false);
        }

        /// <summary>
        /// 旋转阀门
        /// </summary>
        /// <param name="direction">方向+-1</param>
        private void RotateValve(int direction)
        {
            //stem.Rotate(this.transform.forward,
            //direction*Time.deltaTime * rotateSpeed, Space.World);
            Vector3 rot = stem.localEulerAngles;
            rot.z += direction * Time.deltaTime * rotateSpeed;
            stem.localEulerAngles = rot;
        }
        private void FadeOutBar()
        {
            barFade.FadeOut();
        }
    }
}
