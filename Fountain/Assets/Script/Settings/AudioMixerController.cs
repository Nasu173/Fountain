using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// 音频混合器控制器
/// 负责管理游戏音量的设置、保存和加载，以及UI交互
/// </summary>
public class AudioMixerController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [Tooltip("音频混合器，用于控制不同音效组")]
    public AudioMixer audioMixer;  // 音频混合器

    [Header("UI组件")]
    [Tooltip("主音量滑块")]
    public Slider masterSlider;

    [Tooltip("音乐音量滑块")]
    public Slider musicSlider;

    [Tooltip("音效音量滑块")]
    public Slider sfxSlider;

    [Tooltip("主音量百分比显示文本")]
    public TMP_Text masterText;

    [Tooltip("音乐音量百分比显示文本")]
    public TMP_Text musicText;

    [Tooltip("音效音量百分比显示文本")]
    public TMP_Text sfxText;

    [Header("音量参数名称")]
    [Tooltip("AudioMixer中主音量的参数名")]
    public string masterVolumeParam = "MasterVolume";

    [Tooltip("AudioMixer中音乐音量的参数名")]
    public string musicVolumeParam = "BGMVolume";

    [Tooltip("AudioMixer中音效音量的参数名")]
    public string sfxVolumeParam = "SFXVolume";

    [Header("音量范围")]
    [Tooltip("最小音量（分贝）")]
    public float minVolumeDB = -80f;  // 最小音量（分贝）

    [Tooltip("最大音量（分贝）")]
    public float maxVolumeDB = 20f;   // 最大音量（分贝）

    /// <summary>
    /// 初始化组件，设置Slider并加载保存的音量
    /// </summary>
    void Start()
    {
        // 初始化Slider的范围和默认值
        InitializeSliders();

        // 先注册监听器，确保 LoadAllVolumes 赋值时能触发回调应用到 AudioMixer
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // 从PlayerPrefs加载保存的音量设置
        LoadAllVolumes();
    }

    /// <summary>
    /// 初始化Slider（将线性0-1转换为对数分贝）
    /// </summary>
    void InitializeSliders()
    {
        // 设置Slider范围，最小值设为0.0001f避免Mathf.Log10(0)出错
        if (masterSlider != null)
        {
            masterSlider.minValue = 0.0001f; // 避免log(0)
            masterSlider.maxValue = 1f;
        }

        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;
        }
    }

    /// <summary>
    /// 设置主音量
    /// </summary>
    /// <param name="sliderValue">滑块值（0-1之间的线性值）</param>
    public void SetMasterVolume(float sliderValue)
    {
        // 将线性值转换为分贝（人耳感知的音量是对数的）
        float volumeDB = Mathf.Log10(sliderValue) * 20;

        // 限制在指定范围内
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        // 设置AudioMixer参数
        if (audioMixer != null)
            audioMixer.SetFloat(masterVolumeParam, volumeDB);

        // 更新显示文本
        if (masterText != null)
        {
            masterText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        // 保存设置到PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        PlayerPrefs.Save();

        Debug.Log($"Master Volume: {sliderValue:P0} -> {volumeDB:F1}dB");
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="sliderValue">滑块值（0-1之间的线性值）</param>
    public void SetMusicVolume(float sliderValue)
    {
        // 线性转分贝
        float volumeDB = Mathf.Log10(sliderValue) * 20;
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        // 设置AudioMixer
        if (audioMixer != null)
            audioMixer.SetFloat(musicVolumeParam, volumeDB);

        // 更新显示文本
        if (musicText != null)
        {
            musicText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        // 保存设置
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="sliderValue">滑块值（0-1之间的线性值）</param>
    public void SetSFXVolume(float sliderValue)
    {
        // 线性转分贝
        float volumeDB = Mathf.Log10(sliderValue) * 20;
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        // 设置AudioMixer
        if (audioMixer != null)
            audioMixer.SetFloat(sfxVolumeParam, volumeDB);

        // 更新显示文本
        if (sfxText != null)
        {
            sfxText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        // 保存设置
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载所有保存的音量
    /// 从PlayerPrefs读取之前保存的音量值并应用到UI和AudioMixer
    /// </summary>
    void LoadAllVolumes()
    {
        // 加载主音量
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedMaster = PlayerPrefs.GetFloat("MasterVolume");
            if (masterSlider != null)
                masterSlider.value = savedMaster;  // 触发Slider事件
            else
                SetMasterVolume(savedMaster);       // 直接设置
        }
        else
        {
            // 默认音量80%
            if (masterSlider != null)
                masterSlider.value = 0.8f;
        }

        // 加载音乐音量
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume");
            if (musicSlider != null)
                musicSlider.value = savedMusic;
            else
                SetMusicVolume(savedMusic);
        }
        else
        {
            if (musicSlider != null)
                musicSlider.value = 0.8f;
        }

        // 加载音效音量
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume");
            if (sfxSlider != null)
                sfxSlider.value = savedSFX;
            else
                SetSFXVolume(savedSFX);
        }
        else
        {
            if (sfxSlider != null)
                sfxSlider.value = 0.8f;
        }
    }

    /// <summary>
    /// 获取当前音量（分贝）
    /// </summary>
    /// <param name="parameterName">AudioMixer参数名</param>
    /// <returns>当前音量分贝值，如果获取失败返回最小音量</returns>
    public float GetCurrentVolumeDB(string parameterName)
    {
        if (audioMixer != null && audioMixer.GetFloat(parameterName, out float value))
        {
            return value;
        }
        return minVolumeDB;
    }

    /// <summary>
    /// 分贝转线性值
    /// </summary>
    /// <param name="db">分贝值</param>
    /// <returns>线性值（0-1）</returns>
    float DBToLinear(float db)
    {
        return Mathf.Pow(10.0f, db / 20.0f);
    }

    /// <summary>
    /// 线性值转分贝
    /// </summary>
    /// <param name="linear">线性值（0-1）</param>
    /// <returns>分贝值</returns>
    float LinearToDB(float linear)
    {
        return Mathf.Log10(linear) * 20;
    }
}