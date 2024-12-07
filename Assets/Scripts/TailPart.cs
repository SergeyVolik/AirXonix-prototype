using System;
using UnityEngine;

public class TailPart : MonoBehaviour, IBallObstacle
{
    [field: SerializeField]
    public bool IsDestructable { get; set; } = true;

    public event Action onCollidedWithBall;

    public void DestroyObstacle()
    {
        onCollidedWithBall?.Invoke();
    }
}
