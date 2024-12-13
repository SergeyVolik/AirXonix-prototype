using UnityEngine;
using Zenject;

[System.Serializable]
public class PlayerFactorySetting : IPlayerFactorySettings
{
    public GameObject playerPrefab;
    public GameObject PlayerPrefab => playerPrefab;
}

[System.Serializable]
public class LevelFactorySetting : ILevelFactorySetting
{
    public GameObject levelPrefab;

    public GameObject LevelPrefab => levelPrefab;
}

public class GameSetup : MonoInstaller
{
    public GameManager gameManager;

    public PlayerFactorySetting playerFactorySetting;
    public LevelFactorySetting levelFactorySetting;

    public override void InstallBindings()
    {
        Container.BindInstance<IPlayerFactorySettings>(playerFactorySetting);
        Container.BindInstance<ILevelFactorySetting>(levelFactorySetting);

        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<ILevelFactory>().To<LevelFactory>().AsSingle();

        Container.BindInstance(gameManager);
    }
}
