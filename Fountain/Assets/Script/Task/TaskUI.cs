using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 任务UI控制器
/// 负责单个任务UI的显示、更新和动画效果
/// </summary>
public class TaskUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("任务编号文本（如#1、#2）")]
    [SerializeField] private TextMeshProUGUI taskNumberText;

    [Tooltip("任务名称文本")]
    [SerializeField] private TextMeshProUGUI taskNameText;

    [Tooltip("任务进度文本（如3/5）")]
    [SerializeField] private TextMeshProUGUI taskProgressText;

    [Tooltip("任务描述文本")]
    [SerializeField] private TextMeshProUGUI taskDescriptionText;

    [Tooltip("进度条滑块")]
    [SerializeField] private Slider progressSlider;

    [Tooltip("CanvasGroup组件，用于控制整体透明度")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("背景图片")]
    [SerializeField] private Image backgroundImage;

    [Header("Animation Settings")]
    [Tooltip("淡入淡出动画持续时间")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("完成时闪烁动画的每次闪烁持续时间")]
    [SerializeField] private float completeFlashDuration = 0.1f;

    [Header("Debug")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool debugMode = true;

    private TaskData currentTask;      // 当前显示的任务数据
    private string taskNumber;          // 当前显示的任务编号
    private bool isVisible = false;     // UI是否可见

    /// <summary>
    /// 初始化，查找UI组件并设置初始状态
    /// </summary>
    private void Awake()
    {
        // 如果没有指定CanvasGroup，尝试获取
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // 查找所有UI元素
        FindUIElements();

        // 设置初始状态为透明
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            if (debugMode) Debug.Log($"{gameObject.name} 初始alpha = 0");
        }
    }

    /// <summary>
    /// 查找并获取所有UI组件的引用
    /// 通过路径查找，确保即使在Inspector中未拖拽也能正常工作
    /// </summary>
    private void FindUIElements()
    {
        // 查找内容容器
        Transform contentContainer = transform.Find("ContentContainer");
        if (contentContainer != null)
        {
            // 查找标题行
            Transform titleRow = contentContainer.Find("TitleRow");
            if (titleRow != null)
            {
                // 查找任务编号文本
                if (taskNumberText == null)
                {
                    Transform trans = titleRow.Find("TaskNumber");
                    if (trans != null)
                        taskNumberText = trans.GetComponent<TextMeshProUGUI>();
                }

                // 查找任务名称文本
                if (taskNameText == null)
                {
                    Transform trans = titleRow.Find("TaskName");
                    if (trans != null)
                        taskNameText = trans.GetComponent<TextMeshProUGUI>();
                }

                // 查找任务进度文本
                if (taskProgressText == null)
                {
                    Transform trans = titleRow.Find("TaskProgress");
                    if (trans != null)
                        taskProgressText = trans.GetComponent<TextMeshProUGUI>();
                }
            }

            // 查找任务描述文本
            if (taskDescriptionText == null)
            {
                Transform trans = contentContainer.Find("TaskDescription");
                if (trans != null)
                    taskDescriptionText = trans.GetComponent<TextMeshProUGUI>();
            }

            // 查找进度条
            if (progressSlider == null)
            {
                Transform trans = contentContainer.Find("ProgressBar");
                if (trans != null)
                    progressSlider = trans.GetComponent<Slider>();
            }
        }

        // 查找背景图片
        if (backgroundImage == null)
        {
            Transform trans = transform.Find("Background");
            if (trans != null)
                backgroundImage = trans.GetComponent<Image>();
        }
    }

    /// <summary>
    /// 初始化任务UI
    /// </summary>
    /// <param name="task">任务数据</param>
    /// <param name="taskNumber">任务编号</param>
    public void Initialize(TaskData task, string taskNumber = "")
    {
        if (debugMode) Debug.Log($"TaskUI.Initialize: {GetTaskName(task)}, #{taskNumber}");

        currentTask = task;
        this.taskNumber = taskNumber;
        UpdateDisplay(task);

        // 立即播放出现动画
        Show();
    }

    /// <summary>
    /// 安全获取任务名称（避免空引用）
    /// </summary>
    /// <param name="task">任务数据</param>
    /// <returns>任务名称或"null"</returns>
    private string GetTaskName(TaskData task)
    {
        if (task != null)
            return task.taskName;
        return "null";
    }

    /// <summary>
    /// 更新UI显示内容
    /// </summary>
    /// <param name="task">最新的任务数据</param>
    public void UpdateDisplay(TaskData task)
    {
        if (debugMode)
        {
            string progress = "unknown";
            if (task != null)
                progress = task.GetProgressText();
            Debug.Log($"TaskUI.UpdateDisplay: {GetTaskName(task)}, {progress}");
        }

        // 更新任务编号
        if (taskNumberText != null)
        {
            if (!string.IsNullOrEmpty(this.taskNumber))
                taskNumberText.text = "#" + this.taskNumber;
            else
                taskNumberText.text = "";
        }

        // 更新任务名称
        if (taskNameText != null && task != null)
            taskNameText.text = task.taskName;

        // 更新进度文本
        if (taskProgressText != null && task != null)
            taskProgressText.text = task.GetProgressText();

        // 更新任务描述
        if (taskDescriptionText != null && task != null)
            taskDescriptionText.text = task.description;

        // 更新进度条
        if (progressSlider != null && task != null)
            progressSlider.value = task.GetProgressPercentage();
    }

    /// <summary>
    /// 显示任务（淡入动画）
    /// </summary>
    public void Show()
    {
        if (debugMode) Debug.Log("Show() called");
        if (isVisible) return; // 已经可见则直接返回

        StopAllCoroutines(); // 停止所有正在进行的动画
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 隐藏任务（淡出动画）
    /// </summary>
    public void Hide()
    {
        if (debugMode) Debug.Log("Hide() called");
        if (!isVisible) return; // 已经隐藏则直接返回

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 任务完成效果（闪烁动画）
    /// </summary>
    public void Complete()
    {
        if (debugMode) Debug.Log("Complete() called");
        StopAllCoroutines();
        StartCoroutine(PlayCompleteAnimation());
    }

    /// <summary>
    /// 强制显示（调试用）
    /// </summary>
    public void ForceShow()
    {
        if (debugMode) Debug.Log("ForceShow called");
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            isVisible = true;
        }
    }

    /// <summary>
    /// 淡入动画协程
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (debugMode) Debug.Log("FadeIn coroutine started");

        float elapsed = 0f;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        // 逐渐增加透明度
        while (elapsed < fadeDuration && canvasGroup != null)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        // 确保最终透明度为1
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        isVisible = true;

        if (debugMode) Debug.Log("FadeIn complete, alpha=1");
    }

    /// <summary>
    /// 淡出动画协程
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (debugMode) Debug.Log("FadeOut coroutine started");

        float elapsed = 0f;

        // 逐渐减少透明度
        while (elapsed < fadeDuration && canvasGroup != null)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        // 确保最终透明度为0
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        isVisible = false;

        if (debugMode) Debug.Log("FadeOut complete");
    }

    /// <summary>
    /// 任务完成动画协程（绿色闪烁效果）
    /// </summary>
    private IEnumerator PlayCompleteAnimation()
    {
        if (debugMode) Debug.Log("PlayCompleteAnimation started");

        // 确保可见
        if (!isVisible)
        {
            yield return FadeIn();
        }

        // 完成闪烁效果（绿色闪烁3次）
        if (taskNameText != null && backgroundImage != null)
        {
            // 保存原始颜色
            Color originalTextColor = taskNameText.color;
            Color originalNumberColor = Color.white;
            if (taskNumberText != null)
                originalNumberColor = taskNumberText.color;

            Color originalBgColor = backgroundImage.color;

            // 闪烁3次
            for (int i = 0; i < 3; i++)
            {
                // 变为绿色
                if (taskNumberText != null)
                    taskNumberText.color = Color.green;

                if (taskNameText != null)
                    taskNameText.color = Color.green;

                if (backgroundImage != null)
                    backgroundImage.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);

                yield return new WaitForSeconds(completeFlashDuration);

                // 恢复原色
                if (taskNumberText != null)
                    taskNumberText.color = originalNumberColor;

                if (taskNameText != null)
                    taskNameText.color = originalTextColor;

                if (backgroundImage != null)
                    backgroundImage.color = originalBgColor;

                yield return new WaitForSeconds(completeFlashDuration);
            }
        }

        if (debugMode) Debug.Log("PlayCompleteAnimation finished");
    }
}