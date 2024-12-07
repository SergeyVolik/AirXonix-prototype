using System;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private LevelFactory m_LevelFactory;
    private Level m_LevelInstance;

    public event Action<Level> onLevelCreated;
    [Inject]
    void Construct(LevelFactory levelFactory)
    {
        m_LevelFactory = levelFactory;
    }

    private void Awake()
    {
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        m_LevelInstance = m_LevelFactory.SpawnLevel();
        onLevelCreated?.Invoke(m_LevelInstance);

        m_LevelInstance.Countdown.onFinished += Countdown_onFinished;
        m_LevelInstance.onLevelFinished += M_LevelInstance_onLevelFinished;
    }

    private void M_LevelInstance_onLevelFinished()
    {
        RestartLevel();
    }

    private void Countdown_onFinished()
    {
        RestartLevel();
    }

    public void RestartLevel()
    {
        GameObject.Destroy(m_LevelInstance.gameObject);
        SpawnLevel();
    }
}
