using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private LevelFactory m_LevelFactory;
    private Level m_LevelInstance;

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
    }

    public void RestartLevel()
    {
        GameObject.Destroy(m_LevelInstance.gameObject);
        SpawnLevel();
    }
}
