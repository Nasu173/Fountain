using UnityEngine;
using UnityEngine.UI;

namespace Fountain.MiniGame.RepairWaterValve
{
    /// <summary>
    /// 水阀修理进度条的 UI 显示
    /// </summary>
    public class WaterValveUI : MonoBehaviour
    {
        [Header("组件引用")]
        [Tooltip("关联的水阀逻辑组件")]
        [SerializeField] private WaterValveController targetValve;
        
        [Tooltip("进度条")]
        [SerializeField] private Image progressBar;
        
        [Tooltip("需要隐藏/显示的 Canvas 组件")]
        [SerializeField] private Canvas uiCanvas;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            
            if (uiCanvas == null)
            {
                uiCanvas = GetComponent<Canvas>();
            }
        }

        private void Update()
        {
            if (targetValve == null || progressBar == null) return;

            float progress = targetValve.GetRepairedPercentage();
            bool isSelected = targetValve.IsRepaired();

            // 需求：只有当进度为0（被彻底撤销）或者没选中阀门时，才隐藏。
            bool shouldHide = (progress <= 0f) || !isSelected;
            
            // 采用控制 canvas.enabled 而非 GameObject.SetActive
            // 避免自己的 Update 循环被掐断
            if (uiCanvas != null && uiCanvas.enabled == shouldHide)
            {
                uiCanvas.enabled = !shouldHide;
            }

            if (!shouldHide)
            {
                progressBar.fillAmount = progress;

                // 面朝主相机
                if (mainCamera != null)
                {
                    transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
                }
            }
        }
    }
}
