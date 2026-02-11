using Foutain.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingPanel;
    private PlayerInputActions inputActions;

    private bool isPaused = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // 只启用PausePanel相关输入
        inputActions.PausePanel.Enable();
        inputActions.PausePanel.Esc.started += OnEscClicked;

        // 订阅事件
        GameEventBus.Subscribe<ContinueEvent>(GameContinue);
        GameEventBus.Subscribe<SettingEvent>(OpenSettingPanel);

        // 确保游戏开始时是运行状态
        if (!isPaused)
        {
            Time.timeScale = 1.0f;
            pausePanel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        // 取消订阅事件
        inputActions.PausePanel.Esc.started -= OnEscClicked;
        inputActions.PausePanel.Disable();

        // 取消订阅事件
        GameEventBus.Unsubscribe<ContinueEvent>(GameContinue);
        GameEventBus.Unsubscribe<SettingEvent>(OpenSettingPanel);
    }

    private void OnEscClicked(InputAction.CallbackContext obj)
    {
        Pause();
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    private void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // 暂停游戏
            Time.timeScale = 0f;
            pausePanel.SetActive(true);

            // 禁用玩家输入
            inputActions.Player.Disable();

            //显示鼠标
            GameInputManager.Instance.ShowCursor();
        }
        else
        {
            // 继续游戏
            ResumeGame();
        }
    }

    /// <summary>
    /// 暂停面板中点击主菜单按钮后广播MenuEvent事件
    /// </summary>
    public void OnMenuClicked()
    {
        var menuEvent = new MenuEvent();
        GameEventBus.Publish(menuEvent);
    }

    /// <summary>
    /// 暂停面板中点击设置按钮后广播SettingEvent事件
    /// </summary>
    public void OnSettingClicked()
    {
        var settingEvent = new SettingEvent();
        GameEventBus.Publish(settingEvent);

        pausePanel.SetActive(false);
    }

    /// <summary>
    /// 暂停面板中点击继续按钮后广播ContinueEvent事件
    /// </summary>
    public void OnContinueClicked()
    {
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

        // 重新启用玩家输入
        inputActions.Player.Enable();

        //隐藏鼠标
        GameInputManager.Instance.HideCursor();
    }

    private void OpenSettingPanel(SettingEvent settingEvent)
    {
        settingPanel.SetActive(true);
    }

    public void OnBackClicked()
    {
        settingPanel.SetActive(false);

        if (isPaused)
        {
            pausePanel.SetActive(true);
        }
    }
}