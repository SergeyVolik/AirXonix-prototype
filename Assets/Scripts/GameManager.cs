using System;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private ILevelFactory m_LevelFactory;
    private Level m_LevelInstance;

    public event Action<Level> onLevelCreated;

    [Inject]
    void Construct(ILevelFactory levelFactory)
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
        m_LevelInstance.onPlayerSpawned += M_LevelInstance_onPlayerSpawned;
    }

    private void M_LevelInstance_onPlayerSpawned(GameObject obj)
    {
        var charController = obj.GetComponent<CharacterController>();
        charController.SnakeTail.onSnakeHeadDestroyed += RestartLevel;
        charController.onSnakeSelfCollision += RestartLevel;
        charController.onDeath += CharController_onDeath;
    }

    private void CharController_onDeath()
    {
        RestartLevel();
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
