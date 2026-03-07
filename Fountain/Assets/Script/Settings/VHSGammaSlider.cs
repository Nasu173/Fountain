using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// VHS效果伽马值滑动条控制器
/// 负责通过Slider控制VHSRetroFeature的伽马值参数
/// </summary>
public class VHSGammaSlider : MonoBehaviour
{
    [Header("VHS Retro Feature引用")]
    [Tooltip("VHS复古特效组件，如果为空会自动查找")]
    [SerializeField] private VHSRetroFeature vhsRetroFeature;

    [Header("UI组件")]
    [Tooltip("控制伽马值的滑动条")]
    [SerializeField] private Slider gammaSlider;

    [Tooltip("显示当前伽马值的文本组件（可选）")]
    [SerializeField] private TMP_Text valueDisplayText; // 可选

    [Header("Gamma范围")]
    [Tooltip("最小伽马值")]
    [SerializeField] private float minGamma = 0.8f;

    [Tooltip("最大伽马值")]
    [SerializeField] private float maxGamma = 1.2f;

    [Tooltip("默认伽马值")]
    [SerializeField] private float defaultGamma = 1.05f;

    [Header("保存设置")]
    [Tooltip("是否将伽马值设置保存到PlayerPrefs")]
    [SerializeField] private bool saveSettings = true;

    /// <summary>
    /// 初始化组件，查找VHS特效并设置Slider
    /// </summary>
    private void Start()
    {
        // 自动查找VHSRetroFeature（如果未指定）
        if (vhsRetroFeature == null)
        {
            vhsRetroFeature = FindObjectOfType<VHSRetroFeature>();
            if (vhsRetroFeature == null)
            {
                Debug.LogError("VHSGammaSlider: 未找到VHSRetroFeature组件！");
                return;
            }
        }

        // 初始化Slider的范围
        SetupSlider();

        // 从PlayerPrefs加载保存的伽马值
        LoadGamma();

        // 添加Slider值改变监听器
        gammaSlider.onValueChanged.AddListener(OnGammaChanged);
    }

    /// <summary>
    /// 初始化Slider的设置
    /// </summary>
    private void SetupSlider()
    {
        // 设置Slider的最小值和最大值
        gammaSlider.minValue = minGamma;
        gammaSlider.maxValue = maxGamma;
    }

    /// <summary>
    /// 当Slider值改变时调用
    /// 更新VHS特效的伽马值并保存设置
    /// </summary>
    /// <param name="value">新的伽马值</param>
    private void OnGammaChanged(float value)
    {
        // 设置VHS特效的伽马值
        vhsRetroFeature.gamma = value;

        // 更新显示文本（如果有）
        if (valueDisplayText != null)
        {
            valueDisplayText.text = $"{value:F2}"; // 显示两位小数
        }

        // 保存设置到PlayerPrefs
        if (saveSettings)
        {
            PlayerPrefs.SetFloat("VHSGamma", value);
            PlayerPrefs.Save(); // 立即保存
        }
    }

    /// <summary>
    /// 加载保存的伽马值设置
    /// 如果存在保存的设置则使用，否则使用默认值
    /// </summary>
    private void LoadGamma()
    {
        float value = defaultGamma; // 先使用默认值

        // 如果存在保存的设置，则使用保存的值
        if (saveSettings && PlayerPrefs.HasKey("VHSGamma"))
        {
            value = PlayerPrefs.GetFloat("VHSGamma");
        }

        // 设置Slider的值（会自动触发OnGammaChanged事件）
        gammaSlider.value = value;

        // 直接设置VHS特效的伽马值（防止事件未触发的情况）
        vhsRetroFeature.gamma = value;

        // 更新显示文本
        if (valueDisplayText != null)
        {
            valueDisplayText.text = $"{value:F2}";
        }
    }

    /// <summary>
    /// 对象销毁时移除事件监听，防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        if (gammaSlider != null)
        {
            // 移除监听器，避免在对象销毁后仍被调用
            gammaSlider.onValueChanged.RemoveListener(OnGammaChanged);
        }
    }
}