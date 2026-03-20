using Fountain.InputManagement;
using UnityEngine;

public class RevivePanel : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus.Subscribe<MonsterCatchEvent>(Show);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<MonsterCatchEvent>(Show);
    }

    private void Show(MonsterCatchEvent e)
    {
        gameObject.SetActive(true);
        CursorManager.Instance?.SetPausePanelEnabled(true);
    }

    public void OnReviveClicked()
    {
        GameEventBus.Publish(new ReviveEvent());
        gameObject.SetActive(false);
        CursorManager.Instance?.SetPausePanelEnabled(false);
    }
}
