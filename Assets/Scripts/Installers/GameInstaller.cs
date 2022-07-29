using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Lava>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();

        Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<HeightTracker>().AsSingle();
    }
}
