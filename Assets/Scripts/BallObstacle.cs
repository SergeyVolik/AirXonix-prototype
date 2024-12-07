using System;
using UnityEngine;

public interface IBallObstacle { }

public class BallObstacle : MonoBehaviour, IBallObstacle
{
    public bool destructable;
    public LevelGridCell girdCell;
    internal void DestoryObstacle()
    {
        girdCell.HasGround = false;

        GameObject.Destroy(gameObject);
    }
}
