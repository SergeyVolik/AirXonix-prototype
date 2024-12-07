using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IBallObstacle {
    bool IsDestructable { get; set; }
    void DestroyObstacle();
}

public class BallObstacle : MonoBehaviour, IBallObstacle
{
    public bool destructable;

    [HideInInspector]
    public LevelGridCell girdCell;
    public event Action onDestroyed;
    public bool IsDestructable { get => destructable; set => destructable = value; }

    public void DestroyObstacle()
    {
        onDestroyed?.Invoke();
        girdCell.HasGround = false;
        GameObject.Destroy(gameObject);
    }
}
