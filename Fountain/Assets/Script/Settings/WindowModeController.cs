using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Foutain.Localization;

public class WindowModeController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown windowModeDropdown; // 使用TMP_Dropdown
                                                              // 如果使用普通Dropdown: [SerializeField] private Dropdown windowModeDropdown;
    [Tooltip("本地化dropdown的脚本")]
    [SerializeField] private LocalizeDropdown dropdownLocalize;
    [SerializeField] private bool saveSettings = true;

    private void Start()
    {
        if (windowModeDropdown == null) return;

        // 设置下拉选项
        SetupDropdownOptions();

        // 加载保存的设置
        LoadWindowMode();

        // 添加监听
        windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
    }

    private void SetupDropdownOptions()
    {
        dropdownLocalize.SetOptionText();
        /*
         
        windowModeDropdown.ClearOptions();
        windowModeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Fullscreen (Exclusive)",  // 独占全屏
            "Windowed",                 // 窗口模式
            "Fullscreen (Borderless)"  // 无边框全屏
        });
         */
    }

    private void OnWindowModeChanged(int index)
    {
        switch (index)
        {
            case 0: // 全屏模式
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1: // 窗口模式
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2: // 全屏窗口模式
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }

        Screen.fullScreen = (index != 1); // 窗口模式时设为false

        Debug.Log($"窗口模式: {windowModeDropdown.options[index].text}");

        if (saveSettings)
        {
            PlayerPrefs.SetInt("WindowMode", index);
            PlayerPrefs.Save();
        }
    }

    private void LoadWindowMode()
    {
        if (saveSettings && PlayerPrefs.HasKey("WindowMode"))
        {
            int savedMode = PlayerPrefs.GetInt("WindowMode");
            windowModeDropdown.value = savedMode;
            OnWindowModeChanged(savedMode); // 立即应用
        }
        else
        {
            // 设置为当前模式
            if (Screen.fullScreen)
            {
                windowModeDropdown.value = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? 0 : 2;
            }
            else
            {
                windowModeDropdown.value = 1;
            }
        }
    }

    private void OnDestroy()
    {
        if (windowModeDropdown != null)
        {
            windowModeDropdown.onValueChanged.RemoveListener(OnWindowModeChanged);
        }
    }
}