using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fountain.Localization
{
    /// <summary>
    /// 本地化管理器(单例),提供更改语言的方法,其实感觉做成静态类也行
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
            if (Instance!=null)//要跨场景存在,如果做成普通的c#类也行
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            //等待本地化插件初始化完成(插件是异步初始化的)
            if (LocalizationSettings.InitializationOperation.IsDone)
            {
                //Debug.LogWarning("初始化完了吗?");
                LoadLocale();
            }
            else
            {
                //Debug.LogWarning("没那么快初始化");
                LocalizationSettings.InitializationOperation.Completed 
                    += LoadLocale;
            }

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            //Debug.LogWarning("所有的Locale"+LocalizationSettings.AvailableLocales.Locales.Count);

        }
        public void SetLocale(LocaleID id)
        {
            //修改本地化设置并发布事件
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[(int)id];
        }
        /// <summary>
        /// 获取当前的语言
        /// </summary>
        /// <returns></returns>
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




        //-------------下面这些做数据持久化的代码最好不写到这个类里--------------------------
        private const string LanguagePrefKey = "SelectedLanguage";
        /// <summary>
        /// 初始化语言。如果是首次进入游戏，根据系统地区选择默认语言；否则读取本地保存的设定。
        /// </summary>
        private void LoadLocale()
        {
            int savedLocale = 0;
            // 检查是否已经保存过语言设置（判断是否首次进入游戏）
            if (PlayerPrefs.HasKey(LanguagePrefKey))
            {
                savedLocale = PlayerPrefs.GetInt(LanguagePrefKey);
                SetLocale((LocaleID)savedLocale);
                //Debug.LogWarning("加载了存储的语言设置," + currentLocale.ToString());
                return;
            }
            // 第一次进入游戏,根据系统语言选则
            if (Application.systemLanguage == SystemLanguage.Chinese ||
                Application.systemLanguage == SystemLanguage.ChineseSimplified ||
                Application.systemLanguage == SystemLanguage.ChineseTraditional)
            {
                SetLocale(LocaleID.zh);
            }
            else
            {
                // 非中文地区默认英文
                SetLocale(LocaleID.en);
            }
            //Debug.LogWarning("没有存储的语言设置,只用默认语言");
        }
        private void LoadLocale(AsyncOperationHandle<LocalizationSettings> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                LoadLocale();
            }
            else
            {
                Debug.LogError("本地化插件初始化错误!");
            }
        }
        private void SaveLocale()
        {
            PlayerPrefs.SetInt(LanguagePrefKey,(int)currentLocale);
            PlayerPrefs.Save();
        }
        private void OnApplicationQuit()
        {
            SaveLocale(); 
        }

    }
}
