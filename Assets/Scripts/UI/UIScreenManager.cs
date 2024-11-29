using UnityEngine;
using UnityEngine.UI;

public class UIScreenManager : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup _dimmerCanvasGroup;
    [SerializeField] private CanvasGroup _forecastCanvasGroup;
    [SerializeField] private CanvasGroup _factsCanvasGroup;

    [SerializeField] private WeatherManager _weatherManager;

    [Header("Buttons")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 0.5f; 

    private bool _isForecastActive = true;

    private void Start()
    {
        StartingVisualsHandle();
        ButtonsHandle();
    }

    private void ButtonsHandle()
    {
        if (_leftButton != null)
            _leftButton.onClick.AddListener(OnLeftButtonClicked);

        if (_rightButton != null)
            _rightButton.onClick.AddListener(OnRightButtonClicked);

        _leftButton.onClick.AddListener(() => OnTabChanged("Forecast"));
        _rightButton.onClick.AddListener(() => OnTabChanged("Facts"));
    }

    private void StartingVisualsHandle()
    {
        _dimmerCanvasGroup.alpha = 1;
        _dimmerCanvasGroup.blocksRaycasts = true;
        _forecastCanvasGroup.alpha = 1;
        _forecastCanvasGroup.blocksRaycasts = true;
        _factsCanvasGroup.alpha = 0;
        _factsCanvasGroup.blocksRaycasts = false;
        FadeOutDimmer();
    }

    public void OnLeftButtonClicked()
    {
        if (_isForecastActive) 
            return;

        ToggleScreen(_forecastCanvasGroup, _factsCanvasGroup);
        _isForecastActive = true;
    }

    public void OnRightButtonClicked()
    {
        if (!_isForecastActive) 
            return;

        ToggleScreen(_factsCanvasGroup, _forecastCanvasGroup);
        _isForecastActive = false;
    }

    public void OnTabChanged(string tabName)
    {
        if (tabName == "Forecast")
        {
            _forecastCanvasGroup.gameObject.SetActive(true);
            _factsCanvasGroup.gameObject.SetActive(false);
            _weatherManager.ToggleUserOnClickerScreen(true);
        }
        else if (tabName == "Facts")
        {
            _forecastCanvasGroup.gameObject.SetActive(false);
            _factsCanvasGroup.gameObject.SetActive(true);
            _weatherManager.ToggleUserOnClickerScreen(false);
        }
    }

    private void ToggleScreen(CanvasGroup targetToShow, CanvasGroup targetToHide)
    {
        StartCoroutine(SwitchScreensRoutine(targetToShow, targetToHide));
    }

    private System.Collections.IEnumerator SwitchScreensRoutine(CanvasGroup targetToShow, CanvasGroup targetToHide)
    {
        _dimmerCanvasGroup.alpha = 0;
        _dimmerCanvasGroup.gameObject.SetActive(true);
        yield return FadeCanvas(_dimmerCanvasGroup, 1);

        targetToHide.alpha = 0;
        targetToHide.blocksRaycasts = false;

        targetToShow.alpha = 1;
        targetToShow.blocksRaycasts = true;

        yield return FadeCanvas(_dimmerCanvasGroup, 0);
        _dimmerCanvasGroup.gameObject.SetActive(false);
    }

    private void FadeOutDimmer()
    {
        StartCoroutine(FadeCanvas(_dimmerCanvasGroup, 0, () =>
        {
            _dimmerCanvasGroup.gameObject.SetActive(false);
        }));
    }

    private System.Collections.IEnumerator FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, System.Action onComplete = null)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < _fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        if (_leftButton != null)
            _leftButton.onClick.RemoveListener(OnLeftButtonClicked);

        if (_rightButton != null)
            _rightButton.onClick.RemoveListener(OnRightButtonClicked);
    }
}