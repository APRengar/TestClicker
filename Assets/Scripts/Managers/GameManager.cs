using UnityEngine;
using UniRx;
using Zenject;
using Cysharp.Threading.Tasks;

public class GameManager : IInitializable
{
    public IReadOnlyReactiveProperty<int> Currency => _currency;
    public IReadOnlyReactiveProperty<int> Energy => _energy;

    private readonly GameSetings _setings;
    private readonly AudioManager _audioManager;

    private readonly ReactiveProperty<int> _currency = new ReactiveProperty<int>(0);
    private readonly ReactiveProperty<int> _energy = new ReactiveProperty<int>(0);

    [Inject]
    public GameManager(GameSetings settings, AudioManager audioManager)
    {
        _setings = settings;
        _audioManager = audioManager;
    }

    public void Initialize()
    {
        // Debug.Log("GameManager инициализирован.");
        _energy.Value = _setings.maxEnergy;
        StartAutoClick().Forget();
        StartEnergyRecharge().Forget();
    }

    public void PerformClick(Vector2 clickPosition)
    {
        if (_energy.Value < _setings.energyPerClick) 
            return;
        _energy.Value -= _setings.energyPerClick;
        _currency.Value += _setings.currencyPerClick;

        Object.Instantiate(_setings.currencyPrefab, clickPosition, Quaternion.identity);
        Object.Instantiate(_setings.clickVFX, clickPosition, Quaternion.identity);

        _audioManager.PlayClickSound();
    }

    private async UniTaskVoid StartAutoClick()
    {
        while (true)
        {
            await UniTask.Delay((int)(_setings.autoClickInterval * 1000));
            if (_energy.Value >= _setings.autoClickCost)
            {
                _energy.Value -= _setings.autoClickCost;
                _currency.Value += _setings.currencyPerClick;
            }
        }
    }

    private async UniTaskVoid StartEnergyRecharge()
    {
        while (true)
        {
            await UniTask.Delay((int)(_setings.energyRechargeInterval * 1000));
            _energy.Value = Mathf.Min(_energy.Value + _setings.energyRechargeAmount, _setings.maxEnergy);
        }
    }
}