using UnityEngine;
using UnityEngine.UI;

public class UIScreenManager : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup dimmerCanvasGroup;
    [SerializeField] private CanvasGroup forecastCanvasGroup;
    [SerializeField] private CanvasGroup factsCanvasGroup;

    [SerializeField] private WeatherManager weatherManager;

    [Header("Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // Время на анимацию

    private bool isForecastActive = true; // Указывает, какой экран активен

    private void Awake()
    {
        // Привязываем события к кнопкам
        if (leftButton != null)
            leftButton.onClick.AddListener(OnLeftButtonClicked);

        if (rightButton != null)
            rightButton.onClick.AddListener(OnRightButtonClicked);

        leftButton.onClick.AddListener(() => OnTabChanged("Forecast"));
        rightButton.onClick.AddListener(() => OnTabChanged("Facts"));

    }

    private void Start()
    {
        // Устанавливаем стартовые состояния
        dimmerCanvasGroup.alpha = 1; // Dimmer изначально видим
        dimmerCanvasGroup.blocksRaycasts = true;
        forecastCanvasGroup.alpha = 1; // Forecast активен
        forecastCanvasGroup.blocksRaycasts = true;
        factsCanvasGroup.alpha = 0; // Facts скрыт
        factsCanvasGroup.blocksRaycasts = false;

        // Запускаем Fade Out на Dimmer
        FadeOutDimmer();
    }

    public void OnLeftButtonClicked()
    {
        if (isForecastActive) return; // Если Forecast уже активен, ничего не делаем

        ToggleScreen(forecastCanvasGroup, factsCanvasGroup);
        isForecastActive = true;
    }

    public void OnRightButtonClicked()
    {
        if (!isForecastActive) return; // Если Facts уже активен, ничего не делаем

        ToggleScreen(factsCanvasGroup, forecastCanvasGroup);
        isForecastActive = false;
    }

    public void OnTabChanged(string tabName)
    {
        if (tabName == "Forecast")
        {
            forecastCanvasGroup.gameObject.SetActive(true);
            factsCanvasGroup.gameObject.SetActive(false);
            weatherManager.ToggleUserOnClickerScreen(true); // Пользователь на экране Forecast
        }
        else if (tabName == "Facts")
        {
            forecastCanvasGroup.gameObject.SetActive(false);
            factsCanvasGroup.gameObject.SetActive(true);
            weatherManager.ToggleUserOnClickerScreen(false); // Пользователь на экране Facts
        }
    }

    private void ToggleScreen(CanvasGroup targetToShow, CanvasGroup targetToHide)
    {
        StartCoroutine(SwitchScreensRoutine(targetToShow, targetToHide));
    }

    private System.Collections.IEnumerator SwitchScreensRoutine(CanvasGroup targetToShow, CanvasGroup targetToHide)
    {
        // Включаем Dimmer и делаем Fade In
        dimmerCanvasGroup.alpha = 0;
        dimmerCanvasGroup.gameObject.SetActive(true);
        yield return FadeCanvas(dimmerCanvasGroup, 1);

        // Переключаем видимые Canvas
        targetToHide.alpha = 0;
        targetToHide.blocksRaycasts = false;

        targetToShow.alpha = 1;
        targetToShow.blocksRaycasts = true;

        // Делаем Fade Out у Dimmer
        yield return FadeCanvas(dimmerCanvasGroup, 0);
        dimmerCanvasGroup.gameObject.SetActive(false);
    }

    private void FadeOutDimmer()
    {
        StartCoroutine(FadeCanvas(dimmerCanvasGroup, 0, () =>
        {
            dimmerCanvasGroup.gameObject.SetActive(false);
        }));
    }

    private System.Collections.IEnumerator FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, System.Action onComplete = null)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        // Убираем события при уничтожении объекта, чтобы избежать утечек памяти
        if (leftButton != null)
            leftButton.onClick.RemoveListener(OnLeftButtonClicked);

        if (rightButton != null)
            rightButton.onClick.RemoveListener(OnRightButtonClicked);
    }
}