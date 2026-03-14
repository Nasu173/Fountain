using System;
using System.Collections;
using System.Collections.Generic;
using Fountain.Localization;
using Fountain.Player;
using TMPro;
using UnityEngine;

/// <summary>
/// 镜头抖动管理器
/// 负责控制镜头抖动的开关、UI交互和设置保存
/// </summary>
public class CameraShakeManager : MonoBehaviour
{
    [Header("UI组件")]
    [Tooltip("镜头抖动选择下拉菜单")]
    public TMP_Dropdown cameraShakeDropdown; // 镜头抖动选择下拉菜单

    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;

    [Tooltip("显示是否启用镜头抖动的文本组件")]
    public TMP_Text cameraShakeDisplayText; // 显示是否启用镜头抖动的文本组件

    [Tooltip("玩家视线组件，用于控制镜头抖动")]
    [SerializeField] private PlayerSight sight;

    [Tooltip("镜头抖动选项列表（1代表启用，0代表禁用）")]
    public List<int> cameraShakeOptions = new() { 0, 1 };//1代表启用视角抖动，0代表禁用

    [Header("保存设置")]
    [Tooltip("是否保存设置到PlayerPrefs")]
    public bool saveSettings = true; // 是否保存设置

    // PlayerPrefs存储键名
    private const string CAMERA_SHAKE_KEY = "CameraShake";

    /// <summary>
    /// 初始化组件，加载设置并添加事件监听
    /// </summary>
    private void Start()
    {
        // 检查下拉菜单是否已赋值
        if (cameraShakeDropdown == null)
        {
            Debug.LogError("请在Inspector中将CameraShakeDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown选项
        InitializeDropdown();

        // 从PlayerPrefs加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项时立即应用镜头抖动设置
        cameraShakeDropdown.onValueChanged.AddListener(OnCameraShakeChanged);
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
        /*
        // 以下代码被注释，因为使用了本地化系统
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

        // 使用本地化系统设置下拉菜单文本
        dropdownLocalize.SetOptionText();
    }

    /// <summary>
    /// 当帧率选项改变时调用
    /// </summary>
    /// <param name="index">选中的选项索引</param>
    public void OnCameraShakeChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= cameraShakeOptions.Count)
        {
            Debug.LogError("无效的帧率索引: " + index);
            return;
        }

        // 获取选中的镜头抖动值（0或1）
        int targetCameraShake = cameraShakeOptions[index];

        // 应用镜头抖动设置
        SetFrameRate(targetCameraShake);
    }

    /// <summary>
    /// 设置镜头抖动状态
    /// </summary>
    /// <param name="targetCameraShake">目标值（1启用，0禁用）</param>
    private void SetFrameRate(int targetCameraShake)
    {
        // 根据选择启用或禁用镜头抖动
        if (targetCameraShake == 1)
        {
            sight.enableShake = true;  // 启用镜头抖动
        }
        else
        {
            sight.enableShake = false; // 禁用镜头抖动
        }

        // 保存设置到PlayerPrefs
        if (saveSettings)
        {
            PlayerPrefs.SetInt(CAMERA_SHAKE_KEY, targetCameraShake);
            PlayerPrefs.Save(); // 立即保存
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// 从PlayerPrefs读取之前保存的镜头抖动设置并应用到UI
    /// </summary>
    void LoadSettings()
    {
        // 检查是否有保存的设置
        if (saveSettings && PlayerPrefs.HasKey(CAMERA_SHAKE_KEY))
        {
            int savedCameraShake = PlayerPrefs.GetInt(CAMERA_SHAKE_KEY);

            // 查找保存的设置在选项列表中的索引
            int savedIndex = cameraShakeOptions.IndexOf(savedCameraShake);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值为保存的索引
                cameraShakeDropdown.value = savedIndex;
                cameraShakeDropdown.RefreshShownValue(); // 刷新显示

                // 应用保存的设置
                SetFrameRate(savedCameraShake);

                Debug.Log("已加载保存的镜头抖动设置: " + (savedCameraShake == 0 ? "禁用" : "启用"));
            }
        }
        else
        {
            // 如果没有保存的设置，默认设置为启用(1)
            int defaultIndex = cameraShakeOptions.IndexOf(1);
            if (defaultIndex >= 0)
            {
                cameraShakeDropdown.value = defaultIndex;
                SetFrameRate(1); // 默认启用
            }
        }
    }
}