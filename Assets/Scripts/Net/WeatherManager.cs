using System;
using System.Collections;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private RequestQueue _requestQueue;
    [SerializeField] private GameObject _weatherUI; // UI для отображения погоды
    [SerializeField] private Image _weatherIcon; // Иконка погоды
    [SerializeField] private TextMeshProUGUI _weatherText; // Текст погоды

    [SerializeField] private TMP_InputField emailInputField; // InputField для ввода email
    [SerializeField] private TextMeshProUGUI feedbackText; // Поле для вывода сообщений

    private readonly string _apiUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
    private bool _isUserOnClickerScreen = false; // Устанавливается, когда пользователь на экране кликера

    [Serializable]
    public class WeatherResponse
    {
        public Properties properties;

        [Serializable]
        public class Properties
        {
            public Period[] periods;
        }

        [Serializable]
        public class Period
        {
            public int temperature;
            public string icon;
        }
    }

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        _weatherUI.SetActive(false);
        emailInputField.onValueChanged.AddListener(OnEmailChanged);
    }

    private async Task FetchWeatherData()
    {
        if (!IsEmailValid(emailInputField.text))
        {
            Debug.LogError("Некорректный email. Запрос не будет выполнен.");
            feedbackText.text = "Введите корректный email для выполнения запроса!";
            feedbackText.color = Color.red;
            return;
        }
        using (HttpClient client = new HttpClient())
        {
            // client.DefaultRequestHeaders.Add("User-Agent", "MyWeatherApp/1.0 (example@mail.ru)");
            client.DefaultRequestHeaders.Add("User-Agent", $"MyWeatherApp/1.0 ({emailInputField.text})");
            
            try
            {
                var response = await client.GetStringAsync(_apiUrl);
                var weatherData = JsonUtility.FromJson<WeatherResponse>(response);
                UpdateWeatherUI(weatherData);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request failed: {e.Message}");
            }
        }
    }
    
    private void OnEmailChanged(string email)
    {
        if (IsEmailValid(email))
        {
            feedbackText.text = "Email корректен"; // Сообщение об успешной проверке
            feedbackText.color = Color.green;
        }
        else
        {
            feedbackText.text = "Некорректный email!"; // Сообщение об ошибке
            feedbackText.color = Color.red;
        }
    }

    private bool IsEmailValid(string email)
    {
        // Регулярное выражение для проверки структуры email
        string pattern = @"^[a-zA-Z0-9._%+-]+@(?:gmail|mail|inbox|yahoo|outlook)+\.(?:ru|com|org|ge|net)$";
        return Regex.IsMatch(email, pattern);
    }

    private void UpdateWeatherUI(WeatherResponse data)
    {
        // Для простоты берем первый прогноз
        var todayForecast = data.properties.periods[0];
        _weatherText.text = $"Сегодня - {todayForecast.temperature}F";
        StartCoroutine(UpdateWeatherIcon(todayForecast.icon));
    }

    private IEnumerator UpdateWeatherIcon(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                _weatherIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                _weatherUI.SetActive(true);
            }
        }
    }

    public void ToggleUserOnClickerScreen(bool isOnClickerScreen)
    {
        _isUserOnClickerScreen = isOnClickerScreen;
        if (isOnClickerScreen)
        {
            StartWeatherRequests();
        }
        else
        {
            CancelWeatherRequests();
        }
    }

    private void StartWeatherRequests()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        InvokeRepeating(nameof(EnqueueWeatherRequest), 0, 5f);
    }

    private void CancelWeatherRequests()
    {
        _cancellationTokenSource?.Cancel();
        _requestQueue.ClearQueue();
        CancelInvoke(nameof(EnqueueWeatherRequest));
        _weatherUI.SetActive(false);
    }

    private void EnqueueWeatherRequest()
    {
        if (!_isUserOnClickerScreen) return;
        _requestQueue.EnqueueRequest(() => FetchWeatherData());
    }
}