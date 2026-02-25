using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VHSGammaSlider : MonoBehaviour
{
    [Header("VHS Retro Feature引用")]
    [SerializeField] private VHSRetroFeature vhsRetroFeature;

    [Header("UI组件")]
    [SerializeField] private Slider gammaSlider;
    [SerializeField] private TMP_Text valueDisplayText; // 可选

    [Header("Gamma范围")]
    [SerializeField] private float minGamma = 0.8f;
    [SerializeField] private float maxGamma = 1.2f;
    [SerializeField] private float defaultGamma = 1.05f;

    [Header("保存设置")]
    [SerializeField] private bool saveSettings = true;

    private void Start()
    {
        // 自动查找VHSRetroFeature
        if (vhsRetroFeature == null)
        {
            vhsRetroFeature = FindObjectOfType<VHSRetroFeature>();
            if (vhsRetroFeature == null)
            {
                Debug.LogError("VHSGammaSlider: 未找到VHSRetroFeature组件！");
                return;
            }
        }

        // 初始化Slider
        SetupSlider();

        // 加载保存的值
        LoadGamma();

        // 添加监听
        gammaSlider.onValueChanged.AddListener(OnGammaChanged);
    }

    private void SetupSlider()
    {
        gammaSlider.minValue = minGamma;
        gammaSlider.maxValue = maxGamma;
    }

    private void OnGammaChanged(float value)
    {
        // 设置Gamma值
        vhsRetroFeature.gamma = value;

        // 更新显示文本
        if (valueDisplayText != null)
        {
            valueDisplayText.text = $"Gamma: {value:F2}";
        }

        // 保存设置
        if (saveSettings)
        {
            PlayerPrefs.SetFloat("VHSGamma", value);
            PlayerPrefs.Save();
        }
    }

    private void LoadGamma()
    {
        float value = defaultGamma;

        if (saveSettings && PlayerPrefs.HasKey("VHSGamma"))
        {
            value = PlayerPrefs.GetFloat("VHSGamma");
        }

        gammaSlider.value = value;
        vhsRetroFeature.gamma = value;

        if (valueDisplayText != null)
        {
            valueDisplayText.text = $"Gamma: {value:F2}";
        }
    }

    private void OnDestroy()
    {
        if (gammaSlider != null)
        {
            gammaSlider.onValueChanged.RemoveListener(OnGammaChanged);
        }
    }
}
