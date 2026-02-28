using System;
using System.Collections;
using System.Collections.Generic;
using Foutain.Localization;
using Foutain.Player;
using TMPro;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_Dropdown cameraShakeDropdown; // 镜头抖动选择下拉菜单
    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;
    public TMP_Text cameraShakeDisplayText; // 显示是否启用镜头抖动的文本组件

    [SerializeField] private PlayerSight sight;

    public List<int> cameraShakeOptions = new() { 0, 1 };//1代表启用视角抖动，0代表禁用

    [Header("保存设置")]
    public bool saveSettings = true; // 是否保存设置

    private const string CAMERA_SHAKE_KEY = "CameraShake";

    private void Start()
    {
        if (cameraShakeDropdown == null)
        {
            Debug.LogError("请在Inspector中将CameraShakeDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown
        InitializeDropdown();

        // 加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项立即应用帧率
        cameraShakeDropdown.onValueChanged.AddListener(OnCameraShakeChanged);
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
        /*
        // 清除现有选项
        cameraShakeDropdown.ClearOptions();

        // 创建选项列表
        List<string> options = new();

        foreach (int cameraShake in cameraShakeOptions)
        {
            string optionText;
            if (cameraShake == 1)
            {
                optionText = "On";
            }
            else
            {
                optionText = "Off";
            }

            options.Add(optionText);
        }

        // 添加选项到Dropdown
        cameraShakeDropdown.AddOptions(options);

        Debug.Log("帧率选项初始化完成，共 " + options.Count + " 个选项");
         
         */
        dropdownLocalize.SetOptionText();
    }

    /// <summary>
    /// 当帧率选项改变时调用
    /// </summary>
    public void OnCameraShakeChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= cameraShakeOptions.Count)
        {
            Debug.LogError("无效的帧率索引: " + index);
            return;
        }

        // 获取选中的帧率
        int targetCameraShake = cameraShakeOptions[index];

        // 应用帧率
        SetFrameRate(targetCameraShake);
    }

    private void SetFrameRate(int targetCameraShake)
    {
        if (targetCameraShake == 1)
        {
            sight.enableShake = true;
        }
        else
        {
            sight.enableShake = false;
        }

        // 保存设置
        if (saveSettings)
        {
            PlayerPrefs.SetInt(CAMERA_SHAKE_KEY, targetCameraShake);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// </summary>
    void LoadSettings()
    {
        if (saveSettings && PlayerPrefs.HasKey(CAMERA_SHAKE_KEY))
        {
            int savedCameraShake = PlayerPrefs.GetInt(CAMERA_SHAKE_KEY);

            // 查找保存的设置在选项中的索引
            int savedIndex = cameraShakeOptions.IndexOf(savedCameraShake);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值
                cameraShakeDropdown.value = savedIndex;
                cameraShakeDropdown.RefreshShownValue();

                // 应用保存的设置
                SetFrameRate(savedCameraShake);

                Debug.Log("已加载保存的镜头抖动设置: " + (savedCameraShake == 0 ? "禁用" : "启用"));
            }
        }
        else
        {
            // 如果没有保存的设置，设置为启用
            int defaultIndex = cameraShakeOptions.IndexOf(1);
            if (defaultIndex >= 0)
            {
                cameraShakeDropdown.value = defaultIndex;
                SetFrameRate(1);
            }
        }
    }
}
