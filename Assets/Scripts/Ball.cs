using UnityEngine;
using Zenject;

public class Ball : MonoBehaviour, IBallObstacle
{
    private Rigidbody m_Rigidbody;
    private Vector3 velocity;
    public float speed;
    public bool wallDestructor;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        var sign1 = Mathf.Sign(Random.Range(-1f, 1));
        var sign2 = Mathf.Sign(Random.Range(-1f, 1));

        velocity = new Vector3(1* sign1, 0, 1* sign2).normalized;
    }

    float m_PrevCollisionTime;

    public bool IsDestructable { get => false; set { } }

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

            if (wallDestructor)
            {
                if (ballObstacle.IsDestructable)
                {
                    ballObstacle.DestroyObstacle();
                }
            }
        }

        var tailPart = collision.collider.GetComponent<TailPart>();
        if (tailPart)
        {
            tailPart.DestroyObstacle();
        }

        var character = collision.collider.GetComponent<CharacterController>();
        if (character)
        {
            character.ForceDeath();
        }
    }

    private void Update()
    {
        m_Rigidbody.velocity = velocity * speed;
    }

    public void DestroyObstacle()
    {
        throw new System.NotImplementedException();
    }
}
