using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Fountain.Player
{
    /// <summary>
    /// 鼠标灵敏度管理脚本
    /// 独立控制PlayerSight的鼠标灵敏度，支持UI Slider调节
    /// </summary>
    public class MouseSensitivityManager : MonoBehaviour
    {
        [Tooltip("用于显示当前灵敏度数值的文本组件")]
        [SerializeField]
        private TMP_Text valueDisplayText;

        [Header("UI组件")]
        [Tooltip("控制鼠标灵敏度的滑动条")]
        [SerializeField]
        private Slider sensitivitySlider;

        [Header("灵敏度范围")]
        [Tooltip("最小灵敏度值")]
        [SerializeField]
        private float minSensitivity = 0.5f;

        [Tooltip("最大灵敏度值")]
        [SerializeField]
        private float maxSensitivity = 3f;

        [Tooltip("默认灵敏度值")]
        [SerializeField]
        private float defaultSensitivity = 1f;

        [Header("保存设置")]
        [Tooltip("是否将灵敏度设置保存到PlayerPrefs")]
        [SerializeField]
        private bool saveSettings = true;

        private GameInputManager gameInputManager; // 游戏输入管理器引用

        /// <summary>
        /// 初始化组件，设置Slider并加载保存的灵敏度
        /// </summary>
        private void Start()
        {
            // 获取GameInputManager实例
            gameInputManager = GameInputManager.Instance;
            if (gameInputManager == null)
            {
                // 如果单例实例为空，尝试在场景中查找
                gameInputManager = FindObjectOfType<GameInputManager>();
            }

            if (gameInputManager == null)
            {
                Debug.LogError("MouseSensitivitySlider: 未找到GameInputManager！");
                return;
            }

            // 初始化Slider的范围和属性
            SetupSlider();

            // 从PlayerPrefs加载保存的灵敏度设置
            LoadSensitivity();

            // 添加Slider值改变监听器
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        /// <summary>
        /// 初始化Slider的设置
        /// </summary>
        private void SetupSlider()
        {
            if (sensitivitySlider == null) return;

            // 设置Slider的最小值和最大值
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;

            // 允许小数（不是整数滑块）
            sensitivitySlider.wholeNumbers = false;
        }

        /// <summary>
        /// 当Slider值改变时调用
        /// 更新游戏输入管理器的灵敏度并保存设置
        /// </summary>
        /// <param name="value">新的灵敏度值</param>
        private void OnSensitivityChanged(float value)
        {
            // 更新GameInputManager中的灵敏度值
            GameInputManager.Instance.sensitivity = value;

            Debug.Log($"灵敏度设置为: {value}");

            // 保存设置到PlayerPrefs
            if (saveSettings)
            {
                PlayerPrefs.SetFloat("MouseSensitivity", value);
                PlayerPrefs.Save(); // 立即保存
            }

            // 更新显示文本
            if (valueDisplayText != null)
            {
                valueDisplayText.text = $"{value:F1}"; // 显示一位小数
            }
        }

        /// <summary>
        /// 加载保存的灵敏度设置
        /// 如果存在保存的设置则使用，否则使用默认值
        /// </summary>
        private void LoadSensitivity()
        {
            float value = defaultSensitivity; // 先使用默认值

            // 如果存在保存的设置，则使用保存的值
            if (saveSettings && PlayerPrefs.HasKey("MouseSensitivity"))
            {
                value = PlayerPrefs.GetFloat("MouseSensitivity");
            }

            // 设置Slider的值（会自动触发OnSensitivityChanged事件）
            sensitivitySlider.value = value;

            // 直接同步到静态变量（防止事件未触发的情况）
            GameInputManager.Instance.sensitivity = value;

            // 更新显示文本
            if (valueDisplayText != null)
            {
                valueDisplayText.text = $"{value:F1}";
            }
        }

        /// <summary>
        /// 对象销毁时移除事件监听，防止内存泄漏
        /// </summary>
        private void OnDestroy()
        {
            if (sensitivitySlider != null)
            {
                // 移除监听器，避免在对象销毁后仍被调用
                sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            }
        }
    }
}