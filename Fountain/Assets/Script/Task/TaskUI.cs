using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI taskNumberText;
    [SerializeField] private TextMeshProUGUI taskNameText;
    [SerializeField] private TextMeshProUGUI taskProgressText;
    [SerializeField] private TextMeshProUGUI taskDescriptionText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float completeFlashDuration = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private TaskData currentTask;
    private string taskNumber;
    private bool isVisible = false;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        FindUIElements();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            if (debugMode) Debug.Log($"{gameObject.name} 初始alpha = 0");
        }
    }

    private void FindUIElements()
    {
        Transform contentContainer = transform.Find("ContentContainer");
        if (contentContainer != null)
        {
            Transform titleRow = contentContainer.Find("TitleRow");
            if (titleRow != null)
            {
                if (taskNumberText == null)
                {
                    Transform trans = titleRow.Find("TaskNumber");
                    if (trans != null)
                        taskNumberText = trans.GetComponent<TextMeshProUGUI>();
                }

                if (taskNameText == null)
                {
                    Transform trans = titleRow.Find("TaskName");
                    if (trans != null)
                        taskNameText = trans.GetComponent<TextMeshProUGUI>();
                }

                if (taskProgressText == null)
                {
                    Transform trans = titleRow.Find("TaskProgress");
                    if (trans != null)
                        taskProgressText = trans.GetComponent<TextMeshProUGUI>();
                }
            }

            if (taskDescriptionText == null)
            {
                Transform trans = contentContainer.Find("TaskDescription");
                if (trans != null)
                    taskDescriptionText = trans.GetComponent<TextMeshProUGUI>();
            }

            if (progressSlider == null)
            {
                Transform trans = contentContainer.Find("ProgressBar");
                if (trans != null)
                    progressSlider = trans.GetComponent<Slider>();
            }
        }

        if (backgroundImage == null)
        {
            Transform trans = transform.Find("Background");
            if (trans != null)
                backgroundImage = trans.GetComponent<Image>();
        }
    }

    public void Initialize(TaskData task, string taskNumber = "")
    {
        if (debugMode) Debug.Log($"TaskUI.Initialize: {GetTaskName(task)}, #{taskNumber}");

        currentTask = task;
        this.taskNumber = taskNumber;
        UpdateDisplay(task);

        // 立即播放出现动画
        Show();
    }

    private string GetTaskName(TaskData task)
    {
        if (task != null)
            return task.taskName;
        return "null";
    }

    public void UpdateDisplay(TaskData task)
    {
        if (debugMode)
        {
            string progress = "unknown";
            if (task != null)
                progress = task.GetProgressText();
            Debug.Log($"TaskUI.UpdateDisplay: {GetTaskName(task)}, {progress}");
        }

        if (taskNumberText != null)
        {
            if (!string.IsNullOrEmpty(this.taskNumber))
                taskNumberText.text = "#" + this.taskNumber;
            else
                taskNumberText.text = "";
        }

        if (taskNameText != null && task != null)
            taskNameText.text = task.taskName;

        if (taskProgressText != null && task != null)
            taskProgressText.text = task.GetProgressText();

        if (taskDescriptionText != null && task != null)
            taskDescriptionText.text = task.description;

        if (progressSlider != null && task != null)
            progressSlider.value = task.GetProgressPercentage();
    }

    // 显示任务（淡入）
    public void Show()
    {
        if (debugMode) Debug.Log("Show() called");
        if (isVisible) return;

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    // 隐藏任务（淡出）
    public void Hide()
    {
        if (debugMode) Debug.Log("Hide() called");
        if (!isVisible) return;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    // 任务完成效果
    public void Complete()
    {
        if (debugMode) Debug.Log("Complete() called");
        StopAllCoroutines();
        StartCoroutine(PlayCompleteAnimation());
    }

    // 强制显示（调试用）
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

    private IEnumerator FadeIn()
    {
        if (debugMode) Debug.Log("FadeIn coroutine started");

        float elapsed = 0f;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        while (elapsed < fadeDuration && canvasGroup != null)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        isVisible = true;

        if (debugMode) Debug.Log("FadeIn complete, alpha=1");
    }

    private IEnumerator FadeOut()
    {
        if (debugMode) Debug.Log("FadeOut coroutine started");

        float elapsed = 0f;

        while (elapsed < fadeDuration && canvasGroup != null)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        isVisible = false;

        if (debugMode) Debug.Log("FadeOut complete");
    }

    private IEnumerator PlayCompleteAnimation()
    {
        if (debugMode) Debug.Log("PlayCompleteAnimation started");

        // 确保可见
        if (!isVisible)
        {
            yield return FadeIn();
        }

        // 完成闪烁效果
        if (taskNameText != null && backgroundImage != null)
        {
            Color originalTextColor = taskNameText.color;
            Color originalNumberColor = Color.white;
            if (taskNumberText != null)
                originalNumberColor = taskNumberText.color;

            Color originalBgColor = backgroundImage.color;

            for (int i = 0; i < 3; i++)
            {
                if (taskNumberText != null)
                    taskNumberText.color = Color.green;

                if (taskNameText != null)
                    taskNameText.color = Color.green;

                if (backgroundImage != null)
                    backgroundImage.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);

                yield return new WaitForSeconds(completeFlashDuration);

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