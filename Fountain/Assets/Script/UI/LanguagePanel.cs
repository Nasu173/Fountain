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

        private void SetLanguage(int option)
        {
            //请务必保证UI里的语言顺序和LocalizationManager里的枚举顺序是一致的!!
            //以及文字显示也是一致的!!!
            LocalizationManager.Instance.SetLocale
                ((LocalizationManager.LocaleID)option);
        }

    }
}
