using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioMixerController : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;  // 音频混合器

    [Header("UI组件")]
    public Slider musicSlider;
    public Slider masterSlider;
    public Slider sfxSlider;
    public TMP_Text masterText;
    public TMP_Text musicText;
    public TMP_Text sfxText;

    [Header("音量参数名称")]
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "BGMVolume";
    public string sfxVolumeParam = "SFXVolume";

    [Header("音量范围")]
    public float minVolumeDB = -80f;  // 最小音量（分贝）
    public float maxVolumeDB = 20f;   // 最大音量（分贝）

    void Start()
    {
        // 初始化Slider
        InitializeSliders();

        // 加载保存的设置
        LoadAllVolumes();

        // 添加监听
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    /// <summary>
    /// 初始化Slider（将线性0-1转换为对数分贝）
    /// </summary>
    void InitializeSliders()
    {
        // 设置Slider范围
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
    public void SetMasterVolume(float sliderValue)
    {
        // 将线性值转换为分贝
        float volumeDB = Mathf.Log10(sliderValue) * 20;

        // 限制在范围内
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        // 设置AudioMixer参数
        audioMixer.SetFloat(masterVolumeParam, volumeDB);

        // 更新显示文本
        if (masterText != null)
        {
            masterText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        // 保存设置
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        PlayerPrefs.Save();

        Debug.Log($"Master Volume: {sliderValue:P0} -> {volumeDB:F1}dB");
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public void SetMusicVolume(float sliderValue)
    {
        float volumeDB = Mathf.Log10(sliderValue) * 20;
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        audioMixer.SetFloat(musicVolumeParam, volumeDB);

        if (musicText != null)
        {
            musicText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float sliderValue)
    {
        float volumeDB = Mathf.Log10(sliderValue) * 20;
        volumeDB = Mathf.Clamp(volumeDB, minVolumeDB, maxVolumeDB);

        audioMixer.SetFloat(sfxVolumeParam, volumeDB);

        if (sfxText != null)
        {
            sfxText.text = $"{Mathf.RoundToInt(sliderValue * 100)}%";
        }

        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载所有保存的音量
    /// </summary>
    void LoadAllVolumes()
    {
        // 加载主音量
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedMaster = PlayerPrefs.GetFloat("MasterVolume");
            if (masterSlider != null)
                masterSlider.value = savedMaster;
            else
                SetMasterVolume(savedMaster);
        }
        else
        {
            if (masterSlider != null)
                masterSlider.value = 0.8f; // 默认80%
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
    public float GetCurrentVolumeDB(string parameterName)
    {
        if (audioMixer.GetFloat(parameterName, out float value))
        {
            return value;
        }
        return minVolumeDB;
    }

    /// <summary>
    /// 分贝转线性值
    /// </summary>
    float DBToLinear(float db)
    {
        return Mathf.Pow(10.0f, db / 20.0f);
    }

    /// <summary>
    /// 线性值转分贝
    /// </summary>
    float LinearToDB(float linear)
    {
        return Mathf.Log10(linear) * 20;
    }
}
