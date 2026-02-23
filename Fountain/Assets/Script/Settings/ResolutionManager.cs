using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_Dropdown resolutionDropdown; // 关联的Dropdown组件

    [Header("可选设置")]
    public bool showRefreshRate = false; // 是否显示刷新率
    public bool saveSettings = true; // 是否保存设置

    // 存储唯一分辨率的列表
    private readonly List<Resolution> uniqueResolutions = new();

    // 保存用的键名
    private const string RESOLUTION_WIDTH_KEY = "ResolutionWidth";
    private const string RESOLUTION_HEIGHT_KEY = "ResolutionHeight";

    void Start()
    {
        // 检查是否关联了Dropdown
        if (resolutionDropdown == null)
        {
            Debug.LogError("请在Inspector中将ResolutionDropdown拖拽到脚本上！");
            return;
        }

        // 初始化和填充分辨率列表
        InitializeResolutions();

        // 设置当前分辨率在Dropdown中的显示
        SetCurrentResolutionInDropdown();

        // 添加事件监听 - 当Dropdown选项改变时立即应用分辨率
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        Debug.Log("分辨率设置初始化完成 - 切换选项即可直接应用");
    }

    /// <summary>
    /// 获取并过滤唯一分辨率
    /// </summary>
    void InitializeResolutions()
    {
        // 获取所有可用分辨率
        Resolution[] allResolutions = Screen.resolutions;

        // 清空之前的列表
        uniqueResolutions.Clear();

        // 用于去重的HashSet
        HashSet<string> resolutionSet = new();

        // 过滤分辨率，去除相同尺寸不同刷新率的重复项
        for (int i = 0; i < allResolutions.Length; i++)
        {
            Resolution res = allResolutions[i];

            // 根据是否显示刷新率来创建唯一键
            string resolutionKey;
            if (showRefreshRate)
            {
                resolutionKey = res.width + "x" + res.height + "@" + res.refreshRateRatio + "Hz";
            }
            else
            {
                resolutionKey = res.width + "x" + res.height;
            }

            // 如果这个分辨率还没有添加过，就添加它
            if (!resolutionSet.Contains(resolutionKey))
            {
                resolutionSet.Add(resolutionKey);
                uniqueResolutions.Add(res);
            }
        }

        Debug.Log("找到 " + uniqueResolutions.Count + " 个唯一分辨率");

        // 填充Dropdown选项
        PopulateDropdown();
    }

    /// <summary>
    /// 填充Dropdown的选项
    /// </summary>
    void PopulateDropdown()
    {
        // 清除现有选项
        resolutionDropdown.ClearOptions();

        // 创建选项列表
        List<string> options = new();

        // 为每个分辨率创建显示文本
        foreach (Resolution res in uniqueResolutions)
        {
            string optionText;

            if (showRefreshRate)
            {
                // 显示分辨率 + 刷新率
                optionText = res.width + " x " + res.height + "  " + res.refreshRateRatio + "Hz";
            }
            else
            {
                // 只显示分辨率
                optionText = res.width + " x " + res.height;
            }

            options.Add(optionText);
        }

        // 添加选项到Dropdown
        resolutionDropdown.AddOptions(options);

        Debug.Log("已添加 " + options.Count + " 个分辨率选项");
    }

    /// <summary>
    /// 设置当前分辨率在Dropdown中的索引
    /// </summary>
    void SetCurrentResolutionInDropdown()
    {
        // 获取当前分辨率
        Resolution currentRes = Screen.currentResolution;

        // 查找当前分辨率在列表中的索引
        int currentIndex = 0;
        bool found = false;

        for (int i = 0; i < uniqueResolutions.Count; i++)
        {
            Resolution res = uniqueResolutions[i];

            // 比较宽度和高度（忽略刷新率）
            if (res.width == currentRes.width && res.height == currentRes.height)
            {
                currentIndex = i;
                found = true;
                break;
            }
        }

        if (found)
        {
            // 设置Dropdown显示当前分辨率
            resolutionDropdown.value = currentIndex;
            resolutionDropdown.RefreshShownValue();

            Debug.Log("当前分辨率已设置为Dropdown索引: " + currentIndex);
        }
        else
        {
            Debug.LogWarning("当前分辨率不在列表中: " + currentRes.width + "x" + currentRes.height);
        }
    }

    /// <summary>
    /// 当Dropdown选项改变时调用 - 直接应用分辨率
    /// </summary>
    public void OnResolutionChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= uniqueResolutions.Count)
        {
            Debug.LogError("无效的分辨率索引: " + index);
            return;
        }

        // 获取选中的分辨率
        Resolution selectedResolution = uniqueResolutions[index];

        // 应用分辨率
        ApplyResolution(selectedResolution);
    }

    /// <summary>
    /// 应用指定的分辨率
    /// </summary>
    void ApplyResolution(Resolution resolution)
    {
        // 保持当前的全屏模式
        bool isFullscreen = Screen.fullScreen;

        // 设置新分辨率
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);

        Debug.Log("分辨率已切换为: " + resolution.width + " x " + resolution.height);

        // 如果需要保存设置
        if (saveSettings)
        {
            SaveResolutionSettings(resolution);
        }
    }

    /// <summary>
    /// 保存分辨率设置到PlayerPrefs
    /// </summary>
    void SaveResolutionSettings(Resolution resolution)
    {
        PlayerPrefs.SetInt(RESOLUTION_WIDTH_KEY, resolution.width);
        PlayerPrefs.SetInt(RESOLUTION_HEIGHT_KEY, resolution.height);
        PlayerPrefs.Save();

        Debug.Log("分辨率设置已保存");
    }

    /// <summary>
    /// 从保存的设置中加载分辨率（可选）
    /// </summary>
    public void LoadSavedResolution()
    {
        if (saveSettings && PlayerPrefs.HasKey(RESOLUTION_WIDTH_KEY))
        {
            int savedWidth = PlayerPrefs.GetInt(RESOLUTION_WIDTH_KEY);
            int savedHeight = PlayerPrefs.GetInt(RESOLUTION_HEIGHT_KEY);

            // 查找保存的分辨率在列表中的索引
            for (int i = 0; i < uniqueResolutions.Count; i++)
            {
                Resolution res = uniqueResolutions[i];
                if (res.width == savedWidth && res.height == savedHeight)
                {
                    // 设置Dropdown值
                    resolutionDropdown.value = i;
                    resolutionDropdown.RefreshShownValue();

                    // 应用分辨率
                    Screen.SetResolution(savedWidth, savedHeight, Screen.fullScreen);

                    Debug.Log("已加载保存的分辨率: " + savedWidth + "x" + savedHeight);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 获取当前分辨率信息
    /// </summary>
    public string GetCurrentResolutionInfo()
    {
        Resolution current = Screen.currentResolution;
        return "当前分辨率: " + current.width + " x " + current.height;
    }
}
