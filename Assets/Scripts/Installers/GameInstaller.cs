using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<BlockSpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Lava>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();

        Container.BindInstance(true).WhenInjectedInto<GameManager>();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();

        Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<HeightTracker>().AsSingle();
    }
}
