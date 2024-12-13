using System;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private ILevelFactory m_LevelFactory;
    private PlayerLifes m_lifes;
    private Level m_LevelInstance;

    public event Action<Level> onLevelCreated;

    [Inject]
    void Construct(ILevelFactory levelFactory, PlayerLifes lifes)
    {
        m_LevelFactory = levelFactory;
        m_lifes = lifes;
    }

    private void Awake()
    {
        SpawnLevel();

        m_lifes.onLifesChanged += M_lifes_onLifesChanged;
    }

    private void M_lifes_onLifesChanged()
    {
        if (m_lifes.CurrentLifes == 0)
        {
            m_lifes.Reset();
            RestartLevel();
        }
    }

    private void SpawnLevel()
    {
        m_LevelInstance = m_LevelFactory.SpawnLevel();
        onLevelCreated?.Invoke(m_LevelInstance);    
    }

    public void RestartLevel()
    {
        GameObject.Destroy(m_LevelInstance.gameObject);
        SpawnLevel();
    }
}
