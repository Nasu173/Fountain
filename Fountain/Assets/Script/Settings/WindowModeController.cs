using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fountain.Localization;

/// <summary>
/// 窗口模式控制器
/// 负责控制游戏窗口模式（全屏/窗口/无边框）的切换和设置保存
/// </summary>
public class WindowModeController : MonoBehaviour
{
    [Tooltip("窗口模式选择下拉菜单")]
    [SerializeField] private TMP_Dropdown windowModeDropdown; // 使用TMP_Dropdown
                                                              // 如果使用普通Dropdown: [SerializeField] private Dropdown windowModeDropdown;

    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;

    [Tooltip("是否将窗口模式设置保存到PlayerPrefs")]
    [SerializeField] private bool saveSettings = true;

    /// <summary>
    /// 初始化组件，设置下拉选项并加载保存的设置
    /// </summary>
    private void Start()
    {
        if (windowModeDropdown == null) return;

        // 设置下拉选项（使用本地化）
        SetupDropdownOptions();

        // 从PlayerPrefs加载保存的窗口模式
        LoadWindowMode();

        // 添加下拉菜单值改变监听器
        windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
    }

    /// <summary>
    /// 设置下拉菜单的选项
    /// 使用本地化系统设置文本
    /// </summary>
    private void SetupDropdownOptions()
    {
        // 使用本地化系统设置下拉菜单文本
        dropdownLocalize.SetOptionText();

        /*
        // 以下代码被注释，因为使用了本地化系统
        // 手动设置选项的示例：
        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Fullscreen (Exclusive)",  // 独占全屏
            "Windowed",                 // 窗口模式
            "Fullscreen (Borderless)"  // 无边框全屏
        });
         */
    }

    /// <summary>
    /// 当窗口模式选项改变时调用
    /// 根据选择的索引应用对应的窗口模式
    /// </summary>
    /// <param name="index">选中的选项索引（0:全屏, 1:窗口, 2:无边框全屏）</param>
    private void OnWindowModeChanged(int index)
    {
        // 根据索引设置对应的全屏模式
        switch (index)
        {
            case 0: // 全屏模式（独占）
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // 窗口模式
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2: // 无边框全屏模式
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }

        // 设置全屏状态（窗口模式时为false）
        Screen.fullScreen = (index != 1); // 窗口模式时设为false

        Debug.Log($"窗口模式: {windowModeDropdown.options[index].text}");

        // 保存设置到PlayerPrefs
        if (saveSettings)
        {
            PlayerPrefs.SetInt("WindowMode", index);
            PlayerPrefs.Save(); // 立即保存
        }
    }

    /// <summary>
    /// 加载保存的窗口模式设置
    /// 如果存在保存的设置则应用，否则使用当前模式
    /// </summary>
    private void LoadWindowMode()
    {
        // 检查是否有保存的设置
        if (saveSettings && PlayerPrefs.HasKey("WindowMode"))
        {
            int savedMode = PlayerPrefs.GetInt("WindowMode");
            windowModeDropdown.value = savedMode; // 设置下拉菜单值
            OnWindowModeChanged(savedMode); // 立即应用保存的模式
        }
        else
        {
            // 如果没有保存的设置，根据当前屏幕模式设置下拉菜单
            if (Screen.fullScreen)
            {
                // 全屏状态下，判断是全屏还是无边框
                windowModeDropdown.value = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? 0 : 2;
            }
            else
            {
                // 窗口模式
                windowModeDropdown.value = 1;
            }
        }
    }

    /// <summary>
    /// 对象销毁时移除事件监听，防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        if (windowModeDropdown != null)
        {
            // 移除监听器，避免在对象销毁后仍被调用
            windowModeDropdown.onValueChanged.RemoveListener(OnWindowModeChanged);
        }
    }
}