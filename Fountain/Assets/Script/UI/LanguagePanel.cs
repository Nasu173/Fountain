using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Fountain.Common;
using Fountain.Localization;

namespace Fountain.UI
{
    /// <summary>
    /// 语言选择UI
    /// </summary>
    public class LanguagePanel : MonoBehaviour
    {
        private TMP_Dropdown languageDropdown;
        //保证真实的选项和文本的一致
        private LocalizeDropdown localizeDropdown;
        private void Awake()
        {
            languageDropdown = this.transform.FindChildByName(nameof(languageDropdown)).
             GetComponent<TMP_Dropdown>();
            localizeDropdown = languageDropdown.GetComponent<LocalizeDropdown>();
            localizeDropdown.SetOptionText();
            languageDropdown.onValueChanged.AddListener(SetLanguage);
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<LocaleChangeEvent>(RefreshDropdown);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<LocaleChangeEvent>(RefreshDropdown);
        }


        private void SetLanguage(int option)
        {
            if (option==(int)LocalizationManager.Instance.GetLocale())
            {
                return;
            }
            //请务必保证UI里的语言顺序和LocalizationManager里的枚举顺序是一致的!!
            //以及文字显示也是一致的!!!
            LocalizationManager.Instance.SetLocale
                ((LocalizationManager.LocaleID)option);
        }
        private void RefreshDropdown(LocaleChangeEvent e)
        {
            if ((int)e.locale==languageDropdown.value)
            {
                return;
            }
            languageDropdown.value = (int)e.locale;
            languageDropdown.RefreshShownValue();
        }
    }
}
