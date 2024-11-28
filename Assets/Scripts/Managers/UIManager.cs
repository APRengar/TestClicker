using UnityEngine;
using TMPro;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text energyText;


    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Start()
    {
        _gameManager.Currency.Subscribe(value => currencyText.text = $"Currency: {value}").AddTo(this);
        _gameManager.Energy.Subscribe(value => energyText.text = $"Energy: {value}").AddTo(this);

    }
}