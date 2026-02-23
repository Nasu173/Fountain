using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FrameRateManager : MonoBehaviour
{
    [Header("UI组件")]
    public TMP_Dropdown frameRateDropdown; // 帧率选择下拉菜单
    public TMP_Text fpsDisplayText; // 显示FPS的文本组件

    [Header("帧率选项")]
    public List<int> frameRateOptions = new() { 30, 60, 90, 120, 144, 0 }; // 0表示不限制

    [Header("显示设置")]
    public bool showCurrentFPS = true; // 是否显示当前FPS

    [Header("保存设置")]
    public bool saveSettings = true; // 是否保存设置

    // 变量
    private float deltaTime = 0.0f;
    private const string FRAME_RATE_KEY = "TargetFrameRate";
    private float fps = 0f;
    private float updateTimer = 0f;
    private const float UPDATE_INTERVAL = 0.5f;

    void Start()
    {
        // 检查是否关联了Dropdown
        if (frameRateDropdown == null)
        {
            Debug.LogError("请在Inspector中将FrameRateDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown
        InitializeDropdown();

        // 加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项立即应用帧率
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
    }

    void Update()
    {
        // 核心修复：使用unscaledDeltaTime而不是deltaTime
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        updateTimer += Time.unscaledDeltaTime;
        if (updateTimer >= UPDATE_INTERVAL)
        {
            updateTimer = 0f;

            // 安全检查：确保deltaTime不为0
            if (deltaTime > 0f)
            {
                fps = 1f / deltaTime;
            }
            else
            {
                fps = 0f;
            }

            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        if (fpsDisplayText == null) return;

        bool isPaused = Time.timeScale == 0f;

        if (isPaused)
        {
            fpsDisplayText.text = "FPS: Paused";
            fpsDisplayText.color = Color.yellow;
        }
        else
        {
            // 限制显示范围
            float displayFPS = Mathf.Clamp(fps, 0, 999);

            // 颜色编码
            if (displayFPS >= 55)
                fpsDisplayText.color = Color.green;
            else if (displayFPS >= 30)
                fpsDisplayText.color = Color.yellow;
            else
                fpsDisplayText.color = Color.red;

            fpsDisplayText.text = $"FPS: {displayFPS:F1}";
        }
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
        // 清除现有选项
        frameRateDropdown.ClearOptions();

        // 创建选项列表
        List<string> options = new();

        foreach (int fps in frameRateOptions)
        {
            string optionText;
            if (fps == 0)
            {
                optionText = "No limit";
            }
            else if (fps == 30)
            {
                optionText = "30 FPS";
            }
            else if (fps == 60)
            {
                optionText = "60 FPS";
            }
            else if (fps == 90)
            {
                optionText = "90 FPS";
            }
            else if (fps == 120)
            {
                optionText = "120 FPS";
            }
            else if (fps == 144)
            {
                optionText = "144 FPS";
            }
            else
            {
                optionText = fps + " FPS";
            }

            options.Add(optionText);
        }

        // 添加选项到Dropdown
        frameRateDropdown.AddOptions(options);

        Debug.Log("帧率选项初始化完成，共 " + options.Count + " 个选项");
    }

    /// <summary>
    /// 当帧率选项改变时调用
    /// </summary>
    public void OnFrameRateChanged(int index)
    {
        // 确保索引有效
        if (index < 0 || index >= frameRateOptions.Count)
        {
            Debug.LogError("无效的帧率索引: " + index);
            return;
        }

        // 获取选中的帧率
        int targetFPS = frameRateOptions[index];

        // 应用帧率
        SetFrameRate(targetFPS);
    }

    /// <summary>
    /// 设置目标帧率
    /// </summary>
    void SetFrameRate(int targetFPS)
    {
        // 关闭垂直同步（因为VSync会覆盖targetFrameRate设置）
        QualitySettings.vSyncCount = 0;

        if (targetFPS == 0)
        {
            // 0表示不限制帧率
            Application.targetFrameRate = -1;
            Debug.Log("帧率限制已关闭");
        }
        else
        {
            // 设置目标帧率
            Application.targetFrameRate = targetFPS;
            Debug.Log("帧率限制为: " + targetFPS + " FPS");
        }

        // 保存设置
        if (saveSettings)
        {
            PlayerPrefs.SetInt(FRAME_RATE_KEY, targetFPS);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// </summary>
    void LoadSettings()
    {
        if (saveSettings && PlayerPrefs.HasKey(FRAME_RATE_KEY))
        {
            int savedFPS = PlayerPrefs.GetInt(FRAME_RATE_KEY);

            // 查找保存的帧率在选项中的索引
            int savedIndex = frameRateOptions.IndexOf(savedFPS);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值
                frameRateDropdown.value = savedIndex;
                frameRateDropdown.RefreshShownValue();

                // 应用保存的帧率
                SetFrameRate(savedFPS);

                Debug.Log("已加载保存的帧率设置: " + (savedFPS == 0 ? "不限" : savedFPS + " FPS"));
            }
        }
        else
        {
            // 如果没有保存的设置，设置为60FPS
            int defaultIndex = frameRateOptions.IndexOf(60);
            if (defaultIndex >= 0)
            {
                frameRateDropdown.value = defaultIndex;
                SetFrameRate(60);
            }
        }
    }

    /// <summary>
    /// 获取当前帧率设置描述
    /// </summary>
    public string GetCurrentFrameRateDescription()
    {
        int currentFPS = Application.targetFrameRate;
        if (currentFPS == -1)
            return "No limit";
        else
            return currentFPS + " FPS";
    }
}