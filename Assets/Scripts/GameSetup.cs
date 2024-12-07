using Zenject;

public class GameSetup : MonoInstaller
{
    public LevelFactory levelFactory;
    public GameManager gameManager;

    public override void InstallBindings()
    {
        Container.BindInstance(levelFactory);
        Container.BindInstance(gameManager);
    }
}
