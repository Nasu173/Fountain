using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Localization
{
    public class LocalizationTest : MonoBehaviour
    {
        private void Start()
        {
            GameEventBus.Subscribe<LocaleChangeEvent>(PrintLocale);
        }
        private void PrintLocale(LocaleChangeEvent e)
        {
            Debug.Log(e.locale.ToString());
        }
        private void OnGUI()
        {
            if (GUILayout.Button("CN"))
            {
                LocalizationManager.Instance.SetLocale(LocalizationManager.LocaleID.zh);
            }
            if (GUILayout.Button("EN"))
            {
                LocalizationManager.Instance.SetLocale(LocalizationManager.LocaleID.en);
            }
        }
    }
}
