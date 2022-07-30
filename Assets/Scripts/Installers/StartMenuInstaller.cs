using Zenject;

public class StartMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(false).WhenInjectedInto<GameManager>();
        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();

        Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuManager>().AsSingle();
    }
}
