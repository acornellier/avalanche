using Zenject;

public class StartMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(new MenuManager()).AsSingle();

        Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
    }
}
