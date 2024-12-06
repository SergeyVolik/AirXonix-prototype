using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerControlls playerControlls;
    public float speed = 1f;
    private Transform m_Transform;

    private void Awake()
    {
        playerControlls = new PlayerControlls();
        playerControlls.Enable();
        m_Transform = transform;
    }

    private void Update()
    {
        var movement = playerControlls.Input.Movement.ReadValue<Vector2>();

        m_Transform.position += new Vector3(movement.x, 0, movement.y) * speed * Time.deltaTime;

    }
}
