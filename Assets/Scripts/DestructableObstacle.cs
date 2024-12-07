using UnityEngine;

public interface IDestructableObstacle
{
    void Destroy();
}

public class DestructableObstacle : MonoBehaviour, IDestructableObstacle, IBallObstacle
{
    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}