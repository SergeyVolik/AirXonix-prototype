using UnityEngine;
using Zenject;

public class Ball : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Vector3 velocity;
    public float speed;

    [Inject]
    void Construct(GameManager gm)
    {
        m_GM = gm;
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        var vel = Random.onUnitSphere;
        velocity = new Vector3(vel.x, 0, vel.z).normalized;
    }

    float m_PrevCollisionTime;
    private GameManager m_GM;

    private void OnCollisionEnter(Collision collision)
    {
        if (m_PrevCollisionTime + 0.05f > Time.time)
        {
            return;
        }

        m_PrevCollisionTime = Time.time;
        var ballObstacle = collision.collider.GetComponent<IBallObstacle>();
        if (ballObstacle != null)
        {
            velocity = Vector3.Reflect(velocity, collision.contacts[0].normal);

            if (ballObstacle.IsDestructable)
            {
                ballObstacle.DestroyObstacle();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>())
        {
            m_GM.RestartLevel();
        }
    }

    private void Update()
    {
        m_Rigidbody.velocity = velocity * speed;
    }
}
