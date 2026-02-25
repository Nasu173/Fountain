using System;
using System.Collections;
using System.Collections.Generic;
using Foutain.Player;
using TMPro;
using UnityEngine;

public class VHSRetroFeatureManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_Dropdown VHSRetroFeatureDropdown; // VHS滤镜选择下拉菜单
    public TMP_Text VHSRetroFeatureDisplayText; // 显示是否启用VHS滤镜的文本组件

    [SerializeField] private VHSRetroFeature VHS;

    public List<int> VHSRetroFeatureOptions = new() { 0, 1 };//1代表启用VHS滤镜，0代表禁用

    [Header("保存设置")]
    public bool saveSettings = true; // 是否保存设置

    private const string VHS_RETRO_FEATURE_KEY = "VHSRetroFeature";

    private void Start()
    {
        if (VHSRetroFeatureDropdown == null)
        {
            Debug.LogError("请在Inspector中将VHSRetroFeatureDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown
        InitializeDropdown();

        // 加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项立即应用
        VHSRetroFeatureDropdown.onValueChanged.AddListener(OnVHSRetroFeatureChanged);
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
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
    }

    /// <summary>
    /// 当VHS滤镜选项改变时调用
    /// </summary>
    public void OnVHSRetroFeatureChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= VHSRetroFeatureOptions.Count)
        {
            Debug.LogError("无效的帧率索引: " + index);
            return;
        }

        // 获取选中的帧率
        int targetVHSRetroFeature = VHSRetroFeatureOptions[index];

        // 应用帧率
        SetVHSRetroFeature(targetVHSRetroFeature);
    }

    private void SetVHSRetroFeature(int targetVHSRetroFeature)
    {
        if (targetVHSRetroFeature == 1)
        {
            VHS.enabled = true;
        }
        else
        {
            VHS.enabled = false;
        }

        // 保存设置
        if (saveSettings)
        {
            PlayerPrefs.SetInt(VHS_RETRO_FEATURE_KEY, targetVHSRetroFeature);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// </summary>
    void LoadSettings()
    {
        if (saveSettings && PlayerPrefs.HasKey(VHS_RETRO_FEATURE_KEY))
        {
            int savedVHSRetroFeature = PlayerPrefs.GetInt(VHS_RETRO_FEATURE_KEY);

            // 查找保存的设置在选项中的索引
            int savedIndex = VHSRetroFeatureOptions.IndexOf(savedVHSRetroFeature);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值
                VHSRetroFeatureDropdown.value = savedIndex;
                VHSRetroFeatureDropdown.RefreshShownValue();

                // 应用保存的设置
                SetVHSRetroFeature(savedVHSRetroFeature);

                Debug.Log("已加载保存的VHS滤镜设置: " + (savedVHSRetroFeature == 0 ? "禁用" : "启用"));
            }
        }
        else
        {
            // 如果没有保存的设置，设置为启用
            int defaultIndex = VHSRetroFeatureOptions.IndexOf(1);
            if (defaultIndex >= 0)
            {
                VHSRetroFeatureDropdown.value = defaultIndex;
                SetVHSRetroFeature(1);
            }
        }
    }
}

