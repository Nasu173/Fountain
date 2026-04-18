using System;
using Fountain.InputManagement;
using Fountain.Player;
using Foutain.Scene;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private string _menuSceneAddress;
    [SerializeField] private string[] _scenesToKeep;
    [SerializeField] private AudioClip audioClip;

    //输入来源
    private PlayerSightInputProvider sightInput;
    private PauseInputProvider uiInput;
    //玩家相关脚本
    private PlayerMove playerMove;
    private PlayerSight playerSight;
    private PlayerInteractor playerInteractor;

    private bool isPaused = false;
    private bool isStarted = false;
    private bool pausePanelEnabled = false;
    private bool settingPanelEnabled = false;

    private void Start()
    {
        uiInput = GameInputManager.Instance.GetProvider<PauseInputProvider>();
        sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();

        playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
        playerInteractor = PlayerInstance.Instance.GetComponent<PlayerInteractor>();
        playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>();

        pausePanelEnabled = pausePanel != null && pausePanel.activeSelf;
        settingPanelEnabled = settingPanel != null && settingPanel.activeSelf;
        CursorManager.Instance?.SetPausePanelEnabled(pausePanelEnabled);
        CursorManager.Instance?.SetSettingPanelEnabled(settingPanelEnabled);
        isStarted = true;
    }

    private void OnEnable()
    {
        //订阅游戏暂停事件
        GameEventBus.Subscribe<GamePauseEvent>(OnPauseClicked);

        // 订阅事件
        GameEventBus.Subscribe<ContinueEvent>(GameContinue);
        GameEventBus.Subscribe<SettingEvent>(OpenSettingPanel);
        // 确保游戏开始时是运行状态
        if (!isPaused)
        {
            Time.timeScale = 1.0f;
            pausePanel.SetActive(false);
            pausePanelEnabled = false;
            CursorManager.Instance?.SetPausePanelEnabled(false);
        }
    }

    private void OnDisable()
    {
        // 取消订阅事件
        GameEventBus.Unsubscribe<GamePauseEvent>(OnPauseClicked);

        // 取消订阅事件
        GameEventBus.Unsubscribe<ContinueEvent>(GameContinue);
        GameEventBus.Unsubscribe<SettingEvent>(OpenSettingPanel);
    }

    private void Update()
    {
        if (uiInput != null && uiInput.GetPause())
        {
            GameEventBus.Publish(new GamePauseEvent());
        }
    }
    private void OnPauseClicked(GamePauseEvent gamePauseEvent)
    {
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        Pause();
    }

    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void Pause()
    {
        if (!isStarted)
        {
            return;
        }

        isPaused = !isPaused;

        if (isPaused)
        {
            // 暂停游戏
            Time.timeScale = 0f;
            pausePanel.SetActive(true);

            // 禁用玩家行为
            playerInteractor.Disable();
            playerMove.enabled = false;
            playerSight.enabled = false;
            pausePanelEnabled = true;
            CursorManager.Instance?.SetPausePanelEnabled(true);
        }
        else
        {
            // 继续游戏
            ResumeGame();
        }
    }

    /// <summary>
    /// 暂停面板中点击主菜单按钮后切换至主菜单场景
    /// </summary>
    public void OnMenuClicked()
    {
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.BGM });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.Fountain1 });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.Fountain2 });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.PlayerFootstep });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.MonsterFootstep });
        GameEventBus.Publish<PauseSoundEvent>(new PauseSoundEvent()
        { Track = AudioTrack.Other });

        Pause();

        pausePanel.SetActive(false);

        if (uiInput != null) uiInput.enabled = false;

        GameEventBus.Publish(new LoadSceneEvent
        {
            SceneAddress = _menuSceneAddress,
            Additive = true,
            UnloadAll = true,
            ScenesToKeep = _scenesToKeep
        });
    }

    /// <summary>
    /// 暂停面板中点击设置按钮后广播SettingEvent事件
    /// </summary>
    public void OnSettingClicked()
    {
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        var settingEvent = new SettingEvent();
        GameEventBus.Publish(settingEvent);

        pausePanel.SetActive(false);
        pausePanelEnabled = false;
        CursorManager.Instance?.SetPausePanelEnabled(false);

        if (uiInput != null) uiInput.enabled = false;
        //GameInputManager.Instance.DisablePausePanel();
    }

    /// <summary>
    /// 暂停面板中点击继续按钮后广播ContinueEvent事件
    /// </summary>
    public void OnContinueClicked()
    {
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        var continueEvent = new ContinueEvent();
        GameEventBus.Publish(continueEvent);
    }

    public void GameContinue(ContinueEvent continueEvent)
    {
        ResumeGame();
    }

    /// <summary>
    /// 退出暂停状态并继续游戏
    /// </summary>
    private void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;

        // 启用玩家行为
        playerInteractor.Enable();
        playerMove.enabled = true;
        playerSight.enabled = true;
        if (uiInput != null) uiInput.enabled = true;

        //隐藏鼠标
        pausePanelEnabled = false;
        CursorManager.Instance?.SetPausePanelEnabled(false);
    }

    private void OpenSettingPanel(SettingEvent settingEvent)
    {
        settingPanel.SetActive(true);
        settingPanelEnabled = true;
        CursorManager.Instance?.SetSettingPanelEnabled(true);
    }

    public void OnBackClicked()
    {
        GameEventBus.Publish(new PlaySoundEvent
        {
            Clip = audioClip,
            Track = AudioTrack.Other
        });

        if (isStarted)
        {
            if (uiInput != null) uiInput.enabled = true;
        }
        else
        {
            if (uiInput != null) uiInput.enabled = false;
        }

        settingPanel.SetActive(false);
        settingPanelEnabled = false;
        CursorManager.Instance?.SetSettingPanelEnabled(false);

        if (isPaused)
        {
            pausePanel.SetActive(true);
            pausePanelEnabled = true;
            CursorManager.Instance?.SetPausePanelEnabled(true);
        }

        GameEventBus.Publish(new SettingBackEvent());
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
