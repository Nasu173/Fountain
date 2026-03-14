using System;
using System.Collections;
using System.Collections.Generic;
using Fountain.Localization;
using Fountain.Player;
using TMPro;
using UnityEngine;

/// <summary>
/// VHS复古特效管理器
/// 负责控制VHS滤镜的开关、UI交互和设置保存
/// </summary>
public class VHSRetroFeatureManager : MonoBehaviour
{
    [Header("UI组件")]
    [Tooltip("VHS滤镜选择下拉菜单")]
    public TMP_Dropdown VHSRetroFeatureDropdown; // VHS滤镜选择下拉菜单

    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;

    [Tooltip("显示是否启用VHS滤镜的文本组件")]
    public TMP_Text VHSRetroFeatureDisplayText; // 显示是否启用VHS滤镜的文本组件

    [Tooltip("VHS复古特效组件引用")]
    [SerializeField] private VHSRetroFeature VHS;

    [Tooltip("VHS滤镜选项列表（1代表启用，0代表禁用）")]
    public List<int> VHSRetroFeatureOptions = new() { 0, 1 };//1代表启用VHS滤镜，0代表禁用

    [Header("保存设置")]
    [Tooltip("是否保存设置到PlayerPrefs")]
    public bool saveSettings = true; // 是否保存设置

    // PlayerPrefs存储键名
    private const string VHS_RETRO_FEATURE_KEY = "VHSRetroFeature";

    /// <summary>
    /// 初始化组件，加载设置并添加事件监听
    /// </summary>
    private void Start()
    {
        // 检查下拉菜单是否已赋值
        if (VHSRetroFeatureDropdown == null)
        {
            Debug.LogError("请在Inspector中将VHSRetroFeatureDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown选项
        InitializeDropdown();

        // 从PlayerPrefs加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项立即应用VHS滤镜设置
        VHSRetroFeatureDropdown.onValueChanged.AddListener(OnVHSRetroFeatureChanged);
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
        /*
        // 以下代码被注释，因为使用了本地化系统
        // 清除现有选项
        VHSRetroFeatureDropdown.ClearOptions();

        // 创建选项列表
        List<string> options = new();

        foreach (int cameraShake in VHSRetroFeatureOptions)
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
        VHSRetroFeatureDropdown.AddOptions(options);

        Debug.Log("VHS滤镜选项初始化完成，共 " + options.Count + " 个选项");
         */

        // 使用本地化系统设置下拉菜单文本
        dropdownLocalize.SetOptionText();
    }

    /// <summary>
    /// 当VHS滤镜选项改变时调用
    /// </summary>
    /// <param name="index">选中的选项索引</param>
    public void OnVHSRetroFeatureChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= VHSRetroFeatureOptions.Count)
        {
            Debug.LogError("无效的VHS滤镜索引: " + index);
            return;
        }

        // 获取选中的VHS滤镜值（0或1）
        int targetVHSRetroFeature = VHSRetroFeatureOptions[index];

        // 应用VHS滤镜设置
        SetVHSRetroFeature(targetVHSRetroFeature);
    }

    /// <summary>
    /// 设置VHS滤镜状态
    /// </summary>
    /// <param name="targetVHSRetroFeature">目标值（1启用，0禁用）</param>
    private void SetVHSRetroFeature(int targetVHSRetroFeature)
    {
        // 根据选择启用或禁用VHS滤镜
        if (targetVHSRetroFeature == 1)
        {
            VHS.enabled = true;  // 启用VHS滤镜
        }
        else
        {
            VHS.enabled = false; // 禁用VHS滤镜
        }

        // 保存设置到PlayerPrefs
        if (saveSettings)
        {
            PlayerPrefs.SetInt(VHS_RETRO_FEATURE_KEY, targetVHSRetroFeature);
            PlayerPrefs.Save(); // 立即保存
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// 从PlayerPrefs读取之前保存的VHS滤镜设置并应用到UI
    /// </summary>
    void LoadSettings()
    {
        // 检查是否有保存的设置
        if (saveSettings && PlayerPrefs.HasKey(VHS_RETRO_FEATURE_KEY))
        {
            int savedVHSRetroFeature = PlayerPrefs.GetInt(VHS_RETRO_FEATURE_KEY);

            // 查找保存的设置在选项列表中的索引
            int savedIndex = VHSRetroFeatureOptions.IndexOf(savedVHSRetroFeature);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值为保存的索引
                VHSRetroFeatureDropdown.value = savedIndex;
                VHSRetroFeatureDropdown.RefreshShownValue(); // 刷新显示

                // 应用保存的设置
                SetVHSRetroFeature(savedVHSRetroFeature);

                Debug.Log("已加载保存的VHS滤镜设置: " + (savedVHSRetroFeature == 0 ? "禁用" : "启用"));
            }
        }
        else
        {
            // 如果没有保存的设置，默认设置为启用(1)
            int defaultIndex = VHSRetroFeatureOptions.IndexOf(1);
            if (defaultIndex >= 0)
            {
                VHSRetroFeatureDropdown.value = defaultIndex;
                SetVHSRetroFeature(1); // 默认启用
            }
        }
    }
}