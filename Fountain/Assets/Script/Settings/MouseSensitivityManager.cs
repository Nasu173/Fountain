using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Foutain.Player
{
    /// <summary>
    /// 鼠标灵敏度管理脚本
    /// 独立控制PlayerSight的鼠标灵敏度，支持UI Slider调节
    /// </summary>
    public class MouseSensitivityManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text valueDisplayText;

        [Header("UI组件")]
        [SerializeField]
        private Slider sensitivitySlider;

        [Header("灵敏度范围")]
        [SerializeField]
        private float minSensitivity = 0.5f;
        [SerializeField]
        private float maxSensitivity = 3f;
        [SerializeField]
        private float defaultSensitivity = 1f;

        [Header("保存设置")]
        [SerializeField]
        private bool saveSettings = true;

        private GameInputManager gameInputManager;

        private void Start()
        {
            // 获取GameInputManager
            gameInputManager = GameInputManager.Instance;
            if (gameInputManager == null)
            {
                gameInputManager = FindObjectOfType<GameInputManager>();
            }

            if (gameInputManager == null)
            {
                Debug.LogError("MouseSensitivitySlider: 未找到GameInputManager！");
                return;
            }

            // 初始化Slider
            SetupSlider();

            // 加载保存的灵敏度
            LoadSensitivity();

            // 添加监听
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        private void SetupSlider()
        {
            if (sensitivitySlider == null) return;

            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;
            sensitivitySlider.wholeNumbers = false;
        }

        private void OnSensitivityChanged(float value)
        {
            GameInputManager.Instance.sensitivity = value;

            Debug.Log($"灵敏度设置为: {value}");

            if (saveSettings)
            {
                PlayerPrefs.SetFloat("MouseSensitivity", value);
                PlayerPrefs.Save();
            }

            if (valueDisplayText != null)
            {
                valueDisplayText.text = $"{value:F1}";
            }
        }

        private void LoadSensitivity()
        {
            float value = defaultSensitivity;

            if (saveSettings && PlayerPrefs.HasKey("MouseSensitivity"))
            {
                value = PlayerPrefs.GetFloat("MouseSensitivity");
            }

            sensitivitySlider.value = value;
            GameInputManager.Instance.sensitivity = value; // 同步到静态变量

            if (valueDisplayText != null)
            {
                valueDisplayText.text = $"{value:F1}";
            }
        }

        private void OnDestroy()
        {
            if (sensitivitySlider != null)
            {
                sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            }
        }
    }
}