using System;
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

[System.Serializable]
public class PlayerLifesSetting
{
    public int starLifes = 3;
}

public class PlayerLifes
{
    public PlayerLifes(PlayerLifesSetting setting)
    {
        m_setting = setting;
        CurrentLifes = m_setting.starLifes;
    }

    private PlayerLifesSetting m_setting;
    private int m_CurrentLifes;
    public int CurrentLifes
    {
        get
        {
            return m_CurrentLifes;
        }
        set
        {
            m_CurrentLifes = value;
            onLifesChanged?.Invoke();
        }
    }

    public event Action onLifesChanged;

    internal void Reset()
    {
        CurrentLifes = m_setting.starLifes;
    }
}


public class GameSetup : MonoInstaller
{
    public GameManager gameManager;

    public PlayerFactorySetting playerFactorySetting;
    public LevelFactorySetting levelFactorySetting;
    public PlayerLifesSetting playerLifes;
    public override void InstallBindings()
    {
        Container.BindInstance<IPlayerFactorySettings>(playerFactorySetting);
        Container.BindInstance<ILevelFactorySetting>(levelFactorySetting);
        Container.BindInstance<PlayerLifesSetting>(playerLifes);
        Container.Bind<PlayerLifes>().AsSingle();


        Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        Container.Bind<ILevelFactory>().To<LevelFactory>().AsSingle();

        Container.BindInstance(gameManager);
    }
}
