using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FactsManager : MonoBehaviour
{
    [SerializeField] private Transform _factsListHolder;
    [SerializeField] private GameObject _loader;
    private string apiUrl = "https://api.thedogapi.com/v1/breeds"; // URL для получения списка пород собак

    [SerializeField] private GameObject _breedPrefab; // Попап для отображения имени породы
    [SerializeField] private GameObject _popupPrefab; // Попап для отображения деталей породы

    [SerializeField] private Button _updateWeather;
    [SerializeField] private Button _updateAPIInfo;
    
    [Header("Read Only")]
    [SerializeField] private string _currentBreedId;

    private CancellationTokenSource _cancellationTokenSource;

    [System.Serializable]
    public class Breed
    {
        public string id;
        public string name;
        // Другие поля, если они нужны (для текущей задачи нет) для отображения на кнопках в листе.
    }

    private void Start()
    {
        _loader.SetActive(false);
        _updateAPIInfo.onClick.AddListener(FetchBreeds);
        _updateWeather.onClick.AddListener(cancellationToken);
    }

    private async void FetchBreeds()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        _loader.SetActive(true);
        _updateAPIInfo.interactable = false;
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MyWeatherApp/1.0");

            try
            {
                var response = await client.GetStringAsync(apiUrl);
                // Debug.Log($"API Response: {response}");

                var breeds = JsonUtility.FromJson<BreedArrayWrapper>($"{{\"breeds\":{response}}}");
                UpdateBreedListUI(breeds.breeds);
            }
            catch (TaskCanceledException)
            {
                Debug.LogWarning("FetchBreeds was canceled.");
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request failed: {e.Message}");
            }
            finally
            {
                _loader.SetActive(false);
                _updateAPIInfo.interactable = true;
            }
        }
    }

    private void cancellationToken()
    {
        _cancellationTokenSource?.Cancel();
        _updateAPIInfo.interactable = true;
        _loader.SetActive(false);
    }

    // Создаём обёртку для массива, чтобы использовать JsonUtility
    [System.Serializable]
    public class BreedArrayWrapper
    {
        public Breed[] breeds;
    }

    private void UpdateBreedListUI(Breed[] breeds)
    {
        foreach (Transform child in _factsListHolder)
        {
            Destroy(child.gameObject); // Удаляем старые элементы
        }

        for (int i = 0; i < breeds.Length; i++)
        {
            // Debug.Log(i);
            var breed = breeds[i];

            // Создаём кнопку или интерактивный элемент для каждой породы
            var breedButton = Instantiate(_breedPrefab, _factsListHolder);
            breedButton.GetComponent<RectTransform>().localScale = Vector3.one;

            // Назначаем обработчик клика для кнопки
            var breedHolder = breedButton.GetComponent<BreedHolder>();
            breedHolder.breedText.text = breed.name;
            breedHolder.breedNumber.text = (i + 1).ToString();
            breedHolder.breedButton.onClick.AddListener(() => OnBreedClicked(breed));
            // Debug.Log(breed.id);
        }
    }

    private Queue<Func<Task>> requestQueue = new Queue<Func<Task>>();

    private bool isRequestInProgress = false;

    public async void OnBreedClicked(Breed breed)
    {
        // Если запрос уже выполняется, игнорируем новый клик
        if (isRequestInProgress) 
            return;

        isRequestInProgress = true;

        _loader.SetActive(true);
        _currentBreedId = breed.id;

        // Создаем запрос
        Func<Task> factRequest = async () =>
        {
            if (_currentBreedId == breed.id)
            {
                await FetchBreedDetails(breed.id);
            }
            isRequestInProgress = false;
        };
        await factRequest();
    }

    private async Task FetchBreedDetails(string breedId)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

            try
            {
                var response = await client.GetStringAsync($"https://api.thedogapi.com/v1/breeds/{breedId}");  // Запрос по ID породы
                var breedDetails = JsonUtility.FromJson<BreedDetailsResponse>(response);
                ShowBreedDetailsPopup(breedDetails);  
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request failed: {e.Message}");
            }
            finally
            {
                _loader.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class BreedsResponse
    {
        public Breed[] breeds;
    }

    [System.Serializable]
    public class BreedDetailsResponse
    {
        public string name;
        public string description;
        // Другие поля, которые приходят в ответе от API
        public string countrycode;
        public Weight weight;
        public Height height;
        public string bred_for;
        public string breed_group;
        public string life_span;
        public string temperament;
        public string origin;
        public string reference_image_id;
    }
    [System.Serializable]
    public class Weight
    {
        public string imperial;
        public string metric;
    }

    [System.Serializable]
    public class Height
    {
        public string imperial;
        public string metric;
    }

    private void ShowBreedDetailsPopup(BreedDetailsResponse breedDetails)
    {
        var descriptionParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(breedDetails.bred_for))
            descriptionParts.Add($"Bred for: {breedDetails.bred_for}");

        if (!string.IsNullOrWhiteSpace(breedDetails.breed_group))
            descriptionParts.Add($"Breed group: {breedDetails.breed_group}");

        if (!string.IsNullOrWhiteSpace(breedDetails.life_span))
            descriptionParts.Add($"Life span: {breedDetails.life_span}");

        if (!string.IsNullOrWhiteSpace(breedDetails.temperament))
            descriptionParts.Add($"Temperament: {breedDetails.temperament}");

        if (!string.IsNullOrWhiteSpace(breedDetails.origin))
            descriptionParts.Add($"Origin: {breedDetails.origin}");

        if (!string.IsNullOrWhiteSpace(breedDetails.description))
            descriptionParts.Add($"Description: {breedDetails.description}");

        var fullDescription = string.Join("\n", descriptionParts);
        
        // Показ попапа
        var popupObject = Instantiate(_popupPrefab);
        popupObject.GetComponent<PopupObject>().titleText.text = breedDetails.name;
        popupObject.GetComponent<PopupObject>().descriptionText.text = fullDescription;

        _loader.SetActive(false); 
    }
}
