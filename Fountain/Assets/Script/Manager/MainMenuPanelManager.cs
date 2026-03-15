using Foutain.UI;
using UnityEngine;

public class MainMenuPanelManager : MonoBehaviour
{
    [SerializeField] private MainMenuPanel mainMenuPanel;

    private void OnEnable()
    {
        GameEventBus.Subscribe<SettingBackEvent>(OnSettingBack);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<SettingBackEvent>(OnSettingBack);
    }

    private void OnSettingBack(SettingBackEvent e)
    {
        mainMenuPanel.gameObject.SetActive(true);
    }
}
