using UnityEngine;
using Zenject;

public interface ILevelFactorySetting
{
    public GameObject LevelPrefab { get; }
}
public interface ILevelFactory
{
    public Level SpawnLevel();
}
public class LevelFactory : ILevelFactory
{
    private DiContainer m_container;
    private ILevelFactorySetting m_setting;

    public LevelFactory(DiContainer container, ILevelFactorySetting setting)
    {
        m_container = container;
        m_setting = setting;
    }

    public Level SpawnLevel()
    {
        return m_container.InstantiatePrefab(m_setting.LevelPrefab).GetComponent<Level>();
    }
}
