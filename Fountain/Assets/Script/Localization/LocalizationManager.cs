using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Foutain.Localization
{
    /// <summary>
    /// 本地化管理器,提供更改语言的方法,做成单例方便访问
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        /// <summary>
        /// 确保ProjectSettings里的顺序和这个是一样的
        /// </summary>
        public enum LocaleID
        {
            zh=0,//中文
            en//英文
        }
        public static LocalizationManager Instance { get; private set; }
        private LocaleID currentLocale;
        private void Awake()
        {
            Instance = this;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }
        /*测试代码,记得删除
        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard.aKey.wasPressedThisFrame)
            {
                SetLocale(LocaleID.zh);
            }
            else if (keyboard.bKey.wasPressedThisFrame)
            {
                SetLocale(LocaleID.en);
            }
        }
         */
        public void SetLocale(LocaleID id)
        {
            //修改本地化设置并发布事件
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[(int)id];
        }
        public LocaleID GetLocale()
        {
            return currentLocale;
        }
            
        private void OnLocaleChanged(Locale locale)
        {
            string localeCode = null;
            //获得字符串代码,理应匹配成功,都不做检查
            Match match= Regex.Match(locale.LocaleName, @"^.+\((.+)\)$");
            localeCode = match.Groups[1].Value;
            LocaleID localeID = Enum.Parse<LocaleID>(localeCode);
            this.currentLocale = localeID;

            GameEventBus.Publish<LocaleChangeEvent>
                (new LocaleChangeEvent { locale = this.currentLocale });
        }

    }
}
