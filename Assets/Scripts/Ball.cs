using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Vector3 velocity;
    public float speed;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        var vel = Random.onUnitSphere;
        velocity = new Vector3(vel.x, 0, vel.z).normalized;
    }

    float m_PrevCollisionTime;
    private void OnCollisionEnter(Collision collision)
    {
        if (m_PrevCollisionTime + 0.05f > Time.time)
        {
            return;
        }

        m_PrevCollisionTime = Time.time;

        if (collision.collider.GetComponent<BallObstacle>())
        {
            velocity = Vector3.Reflect(velocity, collision.contacts[0].normal);
        }    
    }

    private void Update()
    {
        m_Rigidbody.velocity = velocity * speed;
    }
}
