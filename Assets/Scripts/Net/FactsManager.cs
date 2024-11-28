using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FactsManager : MonoBehaviour
{
    [SerializeField] private Transform factsListHolder; // Сюда будет выводиться список пород
    [SerializeField] private GameObject loader; // Индикатор загрузки
    private string apiUrl = "https://api.thedogapi.com/v1/breeds"; // URL для получения списка пород собак

    [SerializeField] private GameObject breedPrefab; // Попап для отображения имени породы
    [SerializeField] private GameObject popupPrefab; // Попап для отображения деталей породы

    [SerializeField] private Button updateWeather;
    [SerializeField] private Button updateAPIInfo;
    
    [Header("Read Only")]
    [SerializeField] private string currentBreedId;

    private CancellationTokenSource cancellationTokenSource;

    [System.Serializable]
    public class Breed
    {
        public string id;
        public string name;
        // Другие поля, если они нужны (для текущей задачи нет) для отображения на кнопках в листе.
    }

    private void Start()
    {
        loader.SetActive(false);  // Скрываем индикатор загрузки при старте
        updateAPIInfo.onClick.AddListener(FetchBreeds);
        updateWeather.onClick.AddListener(cancellationToken);
    }

    private async void FetchBreeds()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        loader.SetActive(true);  // Показываем индикатор загрузки
        updateAPIInfo.interactable = false;
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MyWeatherApp/1.0");

            try
            {
                var response = await client.GetStringAsync(apiUrl); // Получаем данные
                // Debug.Log($"API Response: {response}"); // Логируем ответ, чтобы проверить

                // Десериализуем массив объектов
                var breeds = JsonUtility.FromJson<BreedArrayWrapper>($"{{\"breeds\":{response}}}");

                // Обновляем UI с полученными данными
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
                loader.SetActive(false); // Скрываем индикатор загрузки
                updateAPIInfo.interactable = true;
            }
        }
    }

    private void cancellationToken()
    {
        cancellationTokenSource?.Cancel();
        updateAPIInfo.interactable = true;
        loader.SetActive(false);
    }

    // Создаём обёртку для массива, чтобы использовать JsonUtility
    [System.Serializable]
    public class BreedArrayWrapper
    {
        public Breed[] breeds;
    }

    private void UpdateBreedListUI(Breed[] breeds)
    {
        foreach (Transform child in factsListHolder)
        {
            Destroy(child.gameObject); // Удаляем старые элементы
        }

        // foreach (var breed in breeds) // Проходим по массиву пород
        // {
        //     // Создаём кнопку или интерактивный элемент для каждой породы
        //     var breedButton = Instantiate(breedPrefab);
        //     breedButton.transform.SetParent(factsListHolder);
        //     breedButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        //     // breedButton.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
        //     // Назначаем обработчик клика для кнопки
        //     breedButton.GetComponent<BreedHolder>().breedButton.onClick.AddListener(() => OnBreedClicked(breed));  // При клике вызывает OnBreedClicked

        //     breedButton.GetComponent<BreedHolder>().breedText.text = breed.name;
        //     breedButton.GetComponent<BreedHolder>().breedNumber.text = breed[i];
        // }

        for (int i = 0; i < breeds.Length; i++)
        {
            // Debug.Log(i);
            var breed = breeds[i];

            // Создаём кнопку или интерактивный элемент для каждой породы
            var breedButton = Instantiate(breedPrefab, factsListHolder);

            // Настраиваем размер кнопки
            breedButton.GetComponent<RectTransform>().localScale = Vector3.one;

            // Назначаем обработчик клика для кнопки
            var breedHolder = breedButton.GetComponent<BreedHolder>();
            breedHolder.breedText.text = breed.name;       // Название породы
            breedHolder.breedNumber.text = (i + 1).ToString(); // Номер породы в списке
            breedHolder.breedButton.onClick.AddListener(() => OnBreedClicked(breed));
            // Debug.Log(breed.id);
        }
    }

    // private Queue<Task> requestQueue = new Queue<Task>();
    // public void OnBreedClicked(Breed breed)
    // {
    //     // Показываем индикатор загрузки для факта
    //     loader.SetActive(true);

    //     // Ставим запрос на получение данных о выбранной породе
    //     var factRequest = FetchBreedDetails(breed.id);
    //     QueueRequest(factRequest);
    // }

    // private void QueueRequest(Task request)
    // {
    //     requestQueue.Enqueue(request);
    //     if (requestQueue.Count == 1)
    //     {
    //         ProcessNextRequest();  // Запускаем первый запрос, если очередь пуста
    //     }
    // }
    // private async void ProcessNextRequest()
    // {
    //     if (requestQueue.Count > 0)
    //     {
    //         var currentRequest = requestQueue.Dequeue();
    //         await currentRequest;  // Ожидаем завершения текущего запроса

    //         if (requestQueue.Count > 0)
    //         {
    //             ProcessNextRequest();  // Запускаем следующий запрос из очереди
    //         }
    //     }
    // }
    private Queue<Func<Task>> requestQueue = new Queue<Func<Task>>();
    // private bool isProcessingRequest = false;
    private bool isRequestInProgress = false;

    public async void OnBreedClicked(Breed breed)
    {
        // // Показываем индикатор загрузки
        // loader.SetActive(true);
        // currentBreedId = breed.id;

        // // Очищаем очередь перед добавлением нового запроса
        // requestQueue.Clear();

        // isProcessingRequest = true;
        // // Создаём запрос с правильным ID
        // Func<Task> factRequest = async () =>
        // {
        //     if (isProcessingRequest) return;
        //     if (currentBreedId == breed.id) // Только если ID совпадает
        //     {
        //         await FetchBreedDetails(breed.id);
        //         isProcessingRequest = false; // Сбрасываем флаг после завершения запроса
        //     }
        // };
        // QueueRequest(factRequest);

        // Если запрос уже выполняется, игнорируем новый клик
        if (isRequestInProgress) return;

        // Помечаем, что запрос начался
        isRequestInProgress = true;

        // Показываем индикатор загрузки
        loader.SetActive(true);
        currentBreedId = breed.id;

        // Создаем запрос
        Func<Task> factRequest = async () =>
        {
            if (currentBreedId == breed.id) // Выполняем запрос только если ID совпадает
            {
                await FetchBreedDetails(breed.id);
            }

            // Завершаем запрос
            isRequestInProgress = false;
        };

        // Запускаем запрос
        await factRequest();
    }


    // private void QueueRequest(Func<Task> request)
    // {
    //     requestQueue.Enqueue(request);
    //     if (requestQueue.Count == 1)
    //     {
    //         ProcessNextRequest(); // Запускаем первый запрос, если очередь пуста
    //     }
    // }

    // private async void ProcessNextRequest()
    // {
    //     if (requestQueue.Count > 0)
    //     {
    //         var currentRequest = requestQueue.Dequeue();
    //         await currentRequest(); // Выполняем текущий запрос

    //         if (requestQueue.Count > 0)
    //         {
    //             ProcessNextRequest(); // Запускаем следующий запрос
    //         }
    //     }
    // }
    private async Task FetchBreedDetails(string breedId)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

            try
            {
                var response = await client.GetStringAsync($"https://api.thedogapi.com/v1/breeds/{breedId}");  // Запрос по ID породы
                var breedDetails = JsonUtility.FromJson<BreedDetailsResponse>(response);
                ShowBreedDetailsPopup(breedDetails);  // Отображаем информацию о породе
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request failed: {e.Message}");
            }
            finally
            {
                loader.SetActive(false); // Скрываем индикатор загрузки после завершения
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
        var popupObject = Instantiate(popupPrefab);
        popupObject.GetComponent<PopupObject>().titleText.text = breedDetails.name;
        popupObject.GetComponent<PopupObject>().descriptionText.text = fullDescription;

        loader.SetActive(false); // Скрываем индикатор загрузки
    }

    // private async Task FetchBreedDetails(string breedId)
    // {
    //     using (HttpClient client = new HttpClient())
    //     {
    //         client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

    //         try
    //         {
    //             var response = await client.GetStringAsync($"https://api.thedogapi.com/v1/breeds/{breedId}");  // Запрос по ID породы
    //             var breedDetails = JsonUtility.FromJson<BreedDetailsResponse>(response);
    //             ShowBreedDetailsPopup(breedDetails);  // Отображаем информацию о породе
    //         }
    //         catch (HttpRequestException e)
    //         {
    //             Debug.LogError($"Request failed: {e.Message}");
    //         }
    //     }
    // }



    // private void ShowBreedDetailsPopup(BreedDetailsResponse breedDetails)
    // {
    //     var descriptionParts = new List<string>();

    //     if (!string.IsNullOrWhiteSpace(breedDetails.bred_for))
    //         descriptionParts.Add($"Bred for: {breedDetails.bred_for}");

    //     if (!string.IsNullOrWhiteSpace(breedDetails.breed_group))
    //         descriptionParts.Add($"Breed group: {breedDetails.breed_group}");

    //     if (!string.IsNullOrWhiteSpace(breedDetails.life_span))
    //         descriptionParts.Add($"Life span: {breedDetails.life_span}");

    //     if (!string.IsNullOrWhiteSpace(breedDetails.temperament))
    //         descriptionParts.Add($"Temperament: {breedDetails.temperament}");

    //     if (!string.IsNullOrWhiteSpace(breedDetails.origin))
    //         descriptionParts.Add($"Origin: {breedDetails.origin}");

    //     if (!string.IsNullOrWhiteSpace(breedDetails.description))
    //         descriptionParts.Add($"Description: {breedDetails.description}");

    //     // Сохраняем итоговое описание
    //     var fullDescription = string.Join("\n", descriptionParts);
        
    //    // Показ попапа
    //     Instantiate(popupPrefab);

    //     // Обновляем UI
    //     var popupObject = popupPrefab.GetComponent<PopupObject>();
    //     popupObject.titleText.text = breedDetails.name;
    //     popupObject.descriptionText.text = fullDescription;

    //     // Логирование
    //     // Debug.Log($"Generated Description: {fullDescription}");
    //     loader.SetActive(false);
    // }
}
