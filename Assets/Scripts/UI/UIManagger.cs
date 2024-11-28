using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class UIManagger : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyCounter;
    [SerializeField] private TMP_Text energyCounter;
    [SerializeField] Slider energySlider;

    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;

        // Подписка на обновление валюты
        _gameManager.Currency.Subscribe(value =>
        {
            currencyCounter.text = $"{value}";
        }).AddTo(this);

        // Подписка на обновление энергии
        _gameManager.Energy.Subscribe(value =>
        {
            energyCounter.text = $"{value}";
        }).AddTo(this);

        _gameManager.Energy.Subscribe(value => energySlider.value = value).AddTo(this);
    }
}