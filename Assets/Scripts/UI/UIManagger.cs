using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class UIManagger : MonoBehaviour
{
    [SerializeField] private TMP_Text _currencyCounter;
    [SerializeField] private TMP_Text _energyCounter;
    [SerializeField] private Slider _energySlider;

    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;

        // Подписка на обновление валюты
        _gameManager.Currency.Subscribe(value =>
        {
            _currencyCounter.text = $"{value}";
        }).AddTo(this);

        // Подписка на обновление энергии
        _gameManager.Energy.Subscribe(value =>
        {
            _energyCounter.text = $"{value}";
        }).AddTo(this);

        _gameManager.Energy.Subscribe(value => _energySlider.value = value).AddTo(this);
    }
}