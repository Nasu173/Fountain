using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Fountain.Localization;

/// <summary>
/// 帧率管理器
/// 负责控制游戏的目标帧率、显示当前FPS以及保存用户设置
/// </summary>
public class FrameRateManager : MonoBehaviour
{
    [Header("UI组件")]
    [Tooltip("帧率选择下拉菜单")]
    public TMP_Dropdown frameRateDropdown; // 帧率选择下拉菜单

    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;

    [Tooltip("显示'帧数'标题的文本")]
    public TMP_Text fpsLableText; // 显示"帧数"那个标题的文本

    [Tooltip("显示当前FPS的文本组件")]
    public TMP_Text fpsDisplayText; // 显示FPS的文本组件

    [Tooltip("暂停时显示的文本")]
    public TMP_Text fpsPauseText; // 暂停时显示的文本

    [Header("帧率选项")]
    [Tooltip("可选的帧率列表（0表示不限制）")]
    public List<int> frameRateOptions = new() { 30, 60, 90, 120, 144, 0 }; // 0表示不限制

    [Header("显示设置")]
    [Tooltip("是否显示当前FPS")]
    public bool showCurrentFPS = true; // 是否显示当前FPS

    [Header("保存设置")]
    [Tooltip("是否保存设置到PlayerPrefs")]
    public bool saveSettings = true; // 是否保存设置

    // 变量
    private float deltaTime = 0.0f;          // 用于计算FPS的帧时间
    private const string FRAME_RATE_KEY = "TargetFrameRate"; // PlayerPrefs存储键名
    private float fps = 0f;                   // 当前计算的FPS值
    private float updateTimer = 0f;            // 更新计时器
    private const float UPDATE_INTERVAL = 0.5f; // FPS显示更新间隔（秒）

    /// <summary>
    /// 初始化组件，加载设置并添加事件监听
    /// </summary>
    void Start()
    {
        // 检查下拉菜单是否已赋值
        if (frameRateDropdown == null)
        {
            Debug.LogError("请在Inspector中将FrameRateDropdown拖拽到脚本上！");
            return;
        }

        // 初始化Dropdown选项
        InitializeDropdown();

        // 从PlayerPrefs加载保存的设置
        LoadSettings();

        // 添加事件监听 - 切换选项立即应用帧率
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
    }

    /// <summary>
    /// 每帧更新，计算并显示当前FPS
    /// </summary>
    void Update()
    {
        // 使用unscaledDeltaTime而不是deltaTime，避免受Time.timeScale影响
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // 按指定间隔更新显示
        updateTimer += Time.unscaledDeltaTime;
        if (updateTimer >= UPDATE_INTERVAL)
        {
            updateTimer = 0f;

            // 安全检查：确保deltaTime不为0
            if (deltaTime > 0f)
            {
                fps = 1f / deltaTime; // 计算FPS = 1/帧时间
            }
            else
            {
                fps = 0f;
            }

            // 更新显示
            UpdateDisplay();
        }
    }

    /// <summary>
    /// 更新FPS显示文本和颜色
    /// </summary>
    void UpdateDisplay()
    {
        if (fpsDisplayText == null) return;

        // 检查游戏是否暂停（Time.timeScale == 0）
        bool isPaused = Time.timeScale == 0f;
        Color displayColor = Color.white;

        if (isPaused)
        {
            // 游戏暂停时，显示暂停文本
            displayColor = Color.yellow;
            fpsPauseText.gameObject.SetActive(true);  // 显示暂停文本
            fpsDisplayText.gameObject.SetActive(false); // 隐藏FPS数值文本
        }
        else
        {
            // 游戏运行时，显示FPS数值
            float displayFPS = Mathf.Clamp(fps, 0, 999); // 限制显示范围

            // 根据FPS值进行颜色编码
            if (displayFPS >= 55)
                displayColor = Color.green;  // 流畅
            else if (displayFPS >= 30)
                displayColor = Color.yellow; // 可接受
            else
                displayColor = Color.red;    // 卡顿

            fpsDisplayText.text = $"{displayFPS:F1}"; // 显示一位小数
            fpsPauseText.gameObject.SetActive(false);  // 隐藏暂停文本
            fpsDisplayText.gameObject.SetActive(true); // 显示FPS数值文本
        }

        // 统一设置所有相关文本的颜色
        fpsLableText.color = displayColor;
        fpsPauseText.color = displayColor;
        fpsDisplayText.color = displayColor;
    }

    /// <summary>
    /// 初始化Dropdown选项
    /// </summary>
    void InitializeDropdown()
    {
        /*
        // 以下代码被注释，因为使用了本地化系统
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
         */

        // 使用本地化系统设置下拉菜单文本
        dropdownLocalize.SetOptionText();
    }

    /// <summary>
    /// 当帧率选项改变时调用
    /// </summary>
    /// <param name="index">选中的选项索引</param>
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
    /// <param name="targetFPS">目标帧率（0表示不限制）</param>
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

        // 保存设置到PlayerPrefs
        if (saveSettings)
        {
            PlayerPrefs.SetInt(FRAME_RATE_KEY, targetFPS);
            PlayerPrefs.Save(); // 立即保存
        }
    }

    /// <summary>
    /// 加载保存的设置
    /// 从PlayerPrefs读取之前保存的帧率设置并应用到UI
    /// </summary>
    void LoadSettings()
    {
        // 检查是否有保存的设置
        if (saveSettings && PlayerPrefs.HasKey(FRAME_RATE_KEY))
        {
            int savedFPS = PlayerPrefs.GetInt(FRAME_RATE_KEY);

            // 查找保存的帧率在选项列表中的索引
            int savedIndex = frameRateOptions.IndexOf(savedFPS);

            if (savedIndex >= 0)
            {
                // 设置Dropdown值为保存的索引
                frameRateDropdown.value = savedIndex;
                frameRateDropdown.RefreshShownValue(); // 刷新显示

                // 应用保存的帧率
                SetFrameRate(savedFPS);

                Debug.Log("已加载保存的帧率设置: " + (savedFPS == 0 ? "不限" : savedFPS + " FPS"));
            }
        }
        else
        {
            // 如果没有保存的设置，默认设置为60FPS
            int defaultIndex = frameRateOptions.IndexOf(60);
            if (defaultIndex >= 0)
            {
                frameRateDropdown.value = defaultIndex;
                SetFrameRate(60); // 默认60FPS
            }
        }
    }

    /// <summary>
    /// 获取当前帧率设置描述
    /// </summary>
    /// <returns>当前帧率设置的文本描述</returns>
    public string GetCurrentFrameRateDescription()
    {
        int currentFPS = Application.targetFrameRate;
        if (currentFPS == -1)
            return "No limit";
        else
            return currentFPS + " FPS";
    }
}