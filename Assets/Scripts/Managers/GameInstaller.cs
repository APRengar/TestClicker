using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameSetings gameSetings;
    [SerializeField] private ClickButton buttonHandler;
    
    public override void InstallBindings()
    {
        Container.Bind<GameSetings>().FromInstance(gameSetings).AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        Container.Bind<UIManager>().AsSingle();
        Container.Bind<AudioManager>().FromComponentInHierarchy().AsSingle();

        Container.QueueForInject(buttonHandler);
    }
}