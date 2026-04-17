using UnityEngine;
using Fountain.Player;
using Fountain.Common;
using System;

namespace Fountain.MiniGame.RepairWaterValve
{
    /// <summary>
    /// 水阀交互逻辑控制类
    /// </summary>
    public class WaterValveController : MonoBehaviour, IInteractable
    {
        [Header("修理设置")]
        [Tooltip("完成修理所需的总时间（秒）")]
        [SerializeField] 
        private float repairTimeRequired = 3f;
        
        [Tooltip("停止交互时，进度下降的速度")]
        [SerializeField]
        private float decreaseRate = 0.5f;

        /// <summary>
        /// 当修理完成时触发的事件
        /// </summary>
        public event Action RepairComplete;
        /// <summary>
        /// 进行修理时触发该事件 
        /// </summary>
        public event Action ProgressIncrease;
        /// <summary>
        /// 修理进度降低时触发该事件 
        /// </summary>
        public event Action ProgressDecrease;

        [Tooltip("水阀完成的任务id")]
        public string taskID;


        /// <summary>
        /// 小规模的视觉效果就用不着太多事件,顶多用上面这两个
        /// </summary>
        private WaterValveVisual visual;
        /// <summary>
        /// 当前的修理进度（0 到 1）
        /// </summary>
        private float repairProgress;
        /// <summary>
        /// 阀门是否已经被修好
        /// </summary>
        private bool repaired;
        /// <summary>
        /// 是否在维修中
        /// </summary>
        private bool repairing;
        private const int DELAY_MAX = 2;
        /// <summary>
        /// 降低进度这一行为的延迟,防止这一帧同时出现又增加又降低的情况
        /// </summary>
        private int decreaseDelay;//2只是随便给的一个大于1的数


        [Header("音效资源")]
        public AudioClip fixSFX;
        public AudioTrack track;
        /// <summary>
        /// 是否在播放维修音效
        /// </summary>
        private bool playingSound;

        private bool canInteract;
        public bool CanInteract 
        { get { return canInteract; } set { canInteract = value; } } 
        private void Start()
        {
            visual = this.GetComponent<WaterValveVisual>();
            repairProgress = 0;
            repaired = false;
            repairing = false;
            decreaseDelay = DELAY_MAX;
            playingSound = false;
        }
        private void Update()
        {
            decreaseDelay = Math.Clamp(decreaseDelay-1, 0, DELAY_MAX);
            if (decreaseDelay<=0)
            {
                repairing = false;
            }
        }
        private void LateUpdate()
        {
            //Debug.LogFormat("FixUpdate :progress:{0},repairing:{1}", repairProgress, repairing);
            if (!repairing&&!repaired)
            {
                if (playingSound)
                {
                    GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
                    { Track = this.track });
                    this.playingSound = false;
                }
                DecreaseProgress();
            }
        }

        public void InteractWith(PlayerInteractor player)
        {
            if (repaired) return;
            if (!playingSound&&!repaired)
            {
                GameEventBus.Publish<PlaySoundEvent>(new PlaySoundEvent()
                { Clip = this.fixSFX, Track = this.track, IsLoop = true });
                this.playingSound = true;
            }
            IncreaseProgress();
        }

        //这两个视觉效果就不用事件了
        public void Select()
        {
            visual.ShowOutline();
            if (repairProgress!=0)
            {
                visual.ShowBar();
            }
        }
        public void Deselect()
        {
            visual.HideOutline();
            visual.HideBar();
        }

        /// <summary>
        /// 水阀是否修好
        /// </summary>
        /// <returns></returns>
        public bool IsRepaired()
        {
            return repaired;
        }
        /// <summary>
        /// 获取进度 
        /// </summary>
        /// <returns></returns>
        public float GetRepairedPercentage()
        {
            return repairProgress;
        }

        private void DecreaseProgress()
        {
            if (repairProgress<=0)
            {
                return;
            }

            repairProgress -= decreaseRate * Time.deltaTime;
            repairProgress = Mathf.Clamp01(repairProgress);
            ProgressDecrease?.Invoke();
        }
        private void IncreaseProgress()
        {
            repairing = true;
            decreaseDelay = DELAY_MAX;

            repairProgress += Time.deltaTime / repairTimeRequired;
            repairProgress = Mathf.Clamp01(repairProgress);
            ProgressIncrease?.Invoke();

            if (repairProgress >= 1f)
            {
                repaired = true;
                repairProgress = 1f;
                RepairComplete?.Invoke();
                GameEventBus.Publish<ValveFixedEvent>(null);
                GameEventBus.Publish(new TaskProgressEvent
                {
                    TaskId = taskID,
                    Amount = 1
                });
                this.canInteract = false;
                GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
                { Track = this.track });
                this.playingSound = false;
            }
        }
    }
}
