using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSetings gameSetings;
    // [SerializeField] private GameObject currencyPopupPrefab;
    // [SerializeField] private GameObject currencyPopupPrefabposition;
    // [SerializeField] private GameObject _currencyPopupPrefab;
    [SerializeField] private ClickButton buttonHandler;
    // [SerializeField] private GameManager gameManager;
    
    public override void InstallBindings()
    {
        Container.Bind<GameSetings>().FromInstance(gameSetings).AsSingle();
        Container.Bind<UIManager>().AsSingle();
        Container.Bind<AudioManager>().FromComponentInHierarchy().AsSingle();

        // Регистрация GameManager
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();

        // Привязываем кнопку
        Container.QueueForInject(buttonHandler);

        // Настроим фабрику
        // Container.Bind<IFactory<CurrencyPopup>>().To<CurrencyPopupFactory>().AsTransient();

        // Привязываем пул
        // Container.Bind<CurrencyPopupPool>().AsTransient();

        // Привязываем префаб CurrencyPopup с фабрикой
        // Container.Bind<CurrencyPopup>()
        //     .FromComponentInNewPrefab(_currencyPopupPrefab)
        //     .AsTransient(); 
        // Container.BindMemoryPool<CurrencyPopup, CurrencyPopupPool>()
        //         .WithInitialSize(10)
        //         .FromComponentInNewPrefab(_currencyPopupPrefab)
        //         .UnderTransformGroup("CurrencyPopups");

        // // Привязка Сигналов
        // Container.DeclareSignal<TapButtonSignal>();

        // Container.BindSignal<TapButtonSignal>()
        //     .ToMethod(HandleTapButtonSignal);

    }

    // private void HandleTapButtonSignal(TapButtonSignal signal)
    // {
    //     // Handle the signal when it is fired
    //     Debug.Log("TapButtonSignal received");
    // }
}