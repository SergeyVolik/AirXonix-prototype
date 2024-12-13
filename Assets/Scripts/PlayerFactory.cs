using UnityEngine;
using Zenject;

public interface IPlayerFactorySettings
{
    public GameObject PlayerPrefab { get; }
}

public interface IPlayerFactory
{
    public GameObject SpawnPlayer(Vector3 position);
}

public class PlayerFactory : IPlayerFactory
{

    private DiContainer m_container;
    private IPlayerFactorySettings m_setting;

    public PlayerFactory(DiContainer container, IPlayerFactorySettings setting)
    {
        m_container = container;
        m_setting = setting;
    }

    public GameObject SpawnPlayer(Vector3 position)
    {
        return m_container.InstantiatePrefab(m_setting.PlayerPrefab, position.normalized, Quaternion.identity, null);
    }
}