using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Fountain.Common;
using Fountain.Localization;
using UnityEngine.UI;

namespace Fountain.UI
{
    /// <summary>
    /// 语言选择UI
    /// </summary>
    public class LanguagePanel : MonoBehaviour
    {
        //控制显隐
        private Button showButton;
        private Button returnButton;
        private Transform setLanguagePanel;

        private TextMeshProUGUI languageName;

        private void Awake()
        {
            showButton = this.transform.FindChildByName(nameof(showButton)).
                GetComponent<Button>();
            returnButton = this.transform.FindChildByName(nameof(returnButton)).
                GetComponent<Button>();
            setLanguagePanel = this.transform.FindChildByName
                (nameof(setLanguagePanel));
            Hide();
            showButton.onClick.AddListener(Show);
            showButton.onClick.AddListener(UpdateLanguageName);
            returnButton.onClick.AddListener(Hide);

            languageName = this.transform.FindChildByName(nameof(languageName)).
                GetComponent<TextMeshProUGUI>();

            InitChangeLanguageButton();
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<LocaleChangeEvent>(UpdateLanguageName);
        }
        private void OnDisable()
        {
            GameEventBus.Subscribe<LocaleChangeEvent>(UpdateLanguageName);
        }

        private void Hide()
        {
            setLanguagePanel.gameObject.SetActive(false);
        }
        private void Show()
        {
            setLanguagePanel.gameObject.SetActive(true);
        }
        private void InitChangeLanguageButton()
        {
            //这里要求按钮名字命名:"locale"+Button,比如"zhButton"
            //方便注册事件
            Button zhButton = this.transform.
                FindChildByName(LocalizationManager.LocaleID.zh.ToString() + "Button").
                GetComponent<Button>();
            zhButton.onClick.AddListener(() =>
            {
                LocalizationManager.Instance.SetLocale
                (LocalizationManager.LocaleID.zh);
            });

            Button enButton = this.transform.
                FindChildByName(LocalizationManager.LocaleID.en.ToString() + "Button").
                GetComponent<Button>();
            enButton.onClick.AddListener(() =>
            {
                LocalizationManager.Instance.SetLocale
                (LocalizationManager.LocaleID.en);
            });

        }
        /// <summary>
        /// 更新当前选中的语言的提示
        /// </summary>
        /// <param name="locale"></param>
        private void UpdateLanguageName(LocalizationManager.LocaleID locale)
        {
            languageName.text = LocalizationManager.Instance.GetLocaleName(locale);        
        }
        private void UpdateLanguageName()
        {
            UpdateLanguageName(LocalizationManager.Instance.GetLocale());
        }
        //用于注册事件的
        private void UpdateLanguageName(LocaleChangeEvent e)
        {
            UpdateLanguageName(e.locale);
        }
    }
}
