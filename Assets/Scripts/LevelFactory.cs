using UnityEngine;
using Zenject;

public class LevelFactory : MonoBehaviour
{
    public GameObject levelPrefab;
    private DiContainer m_container;

    [Inject]
    void Construct(DiContainer container)
    {
        m_container = container;
    }

    public Level SpawnLevel()
    {
        return m_container.InstantiatePrefab(levelPrefab).GetComponent<Level>();
    }
}
