using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class VirusEnemy : MonoBehaviour
{
    public float speed;
    private Level m_Level;

    private Vector3 velocity;
    private Transform m_Transform;
    public LayerMask groundLayer;
    public void Construct(Level level)
    {
        m_Level = level;
    }

    private void Awake()
    {
        var sign1 = Mathf.Sign(Random.Range(-1f, 1));
        var sign2 = Mathf.Sign(Random.Range(-1f, 1));

        velocity = new Vector3(1 * sign1, 0, 1 * sign2).normalized;
        m_Transform = transform;
    }

    private void Update()
    {
        var currentPos = m_Transform.position;
        Vector3 nextPos = currentPos + Time.deltaTime * velocity;

        if (!Physics.Raycast(nextPos, Vector3.down, 100, groundLayer))
        {
            var cell = m_Level.Grid.GetClosestCell(currentPos);

            var normal = Vector3.zero;
            if (!cell.IsValidDown() || !m_Level.Grid.GetCell(cell.downNeighbourIndex).HasGround)
            {
                normal.z = 1;
            }
            else if (!cell.IsValidTop() || !m_Level.Grid.GetCell(cell.topNeighbourIndex).HasGround)
            {
                normal.z = -1;
            }
            else if (!cell.IsValidLeft() || !m_Level.Grid.GetCell(cell.leftNeighbourIndex).HasGround)
            {
                normal.x = -1;
            }
            else if (!cell.IsValidRight() || !m_Level.Grid.GetCell(cell.rightNeighbourIndex).HasGround)
            {
                normal.x = 1;
            }

            velocity = Vector3.Reflect(velocity, normal).normalized;
        }

        m_Transform.position = nextPos;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out var character))
        {
            character.ForceDeath();
        }
    }
}
