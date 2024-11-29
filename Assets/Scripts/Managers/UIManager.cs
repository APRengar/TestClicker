using UnityEngine;
using TMPro;
using Zenject;
using UniRx;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _currencyText;
    [SerializeField] private TMP_Text _energyText;

    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Start()
    {
        _gameManager.Currency.Subscribe(value => _currencyText.text = $"Currency: {value}").AddTo(this);
        _gameManager.Energy.Subscribe(value => _energyText.text = $"Energy: {value}").AddTo(this);
    }
}