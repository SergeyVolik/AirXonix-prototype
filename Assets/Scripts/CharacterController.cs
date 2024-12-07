using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Cinemachine.DocumentationSortingAttribute;

public class CharacterController : MonoBehaviour
{
 
    public float speed = 1f;
    private Transform m_Transform;
    private Level m_Level;
    private Vector2 m_SnakeDirection;
    private SnakeTail m_SnakeTail;
    public GameObject tailPrefab;
    public GameObject tailtoDestoryPrefab;

    private GameManager m_gameManager;

    [Inject]
    void Construct(GameManager gameManager)
    {
        m_gameManager = gameManager;
    }

    private void Awake()
    {
        m_SnakeTail = new SnakeTail(tailPrefab);
        m_Transform = transform;

        m_Level = GetComponentInParent<Level>();
    }

    public Vector2 moveInput;

    private void Update()
    {
        var input = moveInput;

        var cell = m_Level.m_LevelGrid.GetClosestCell(m_Transform.position);

        input = UpdateInput(input);

        if (cell.HasGround)
        {
            ClearSnake();
            SimpleMovement(input);
        }
        else
        {
            SnakeMovement(input, cell);
        }
    }

    private void ClearSnake()
    {
        if (m_SnakeDirection == Vector2.zero)
            return;

        m_SnakeDirection = Vector2.zero;

        foreach (var item in m_SnakeTail.Tail)
        {
            item.ClearCell();
            m_Level.m_LevelGrid.SpawnGroundOnCell(item);
        }

       
        var tailPart = m_SnakeTail.GetHead();
        var tailEnd = m_SnakeTail.GetTailEnd();
        var tailMiddle = m_SnakeTail.GetTailEnd();

        m_Level.TryFillArea(tailPart, tailMiddle, tailEnd);

        m_SnakeTail.Clear();
    }

    private static Vector2 UpdateInput(Vector2 input)
    {
        float xMovement = Mathf.Sign(input.x);
        float zMovement = Mathf.Sign(input.y);

        if (input.x != 0)
        {
            input.y = 0;
            input.x = xMovement;
        }
        else if (input.y != 0)
        {
            input.y = zMovement;
            input.x = 0;
        }

        return input;
    }

    private void SimpleMovement(Vector2 moveInpute)
    {
        m_Transform.position += new Vector3(moveInpute.x, 0, moveInpute.y) * speed * Time.deltaTime;
    }

    private void SnakeMovement(Vector2 moveInpute, LevelGridCell cell)
    {
        if (m_SnakeDirection == Vector2.zero || m_SnakeDirection != moveInpute * -1 && moveInpute != Vector2.zero)
        {
            m_SnakeDirection = moveInpute;
        }

        if (!m_SnakeTail.IsTailPart(cell))
        {
            m_SnakeTail.AddTail(cell);
        }
        else if (!m_SnakeTail.IsHead(cell))
        {
            m_gameManager.RestartLevel();
        }

        m_Transform.position += new Vector3(m_SnakeDirection.x, 0, m_SnakeDirection.y) * speed * Time.deltaTime;
    }
}

public class SnakeTail
{
    public static Vector2 headUp = new Vector2(0, 1);
    public static Vector2 headDown = new Vector2(0, -1);
    public static Vector2 headLeft = new Vector2(-1, 0);
    public static Vector2 headRight = new Vector2(1, 0);

    private List<LevelGridCell> m_SnakeTail = new List<LevelGridCell>();

    public List<LevelGridCell> Tail => m_SnakeTail;

    private GameObject m_snakeTailPrefab;

    public SnakeTail(GameObject snakeTailPrefab)
    {
        m_snakeTailPrefab = snakeTailPrefab;
    }

    public bool IsTailPart(LevelGridCell cell)
    {
        return m_SnakeTail.Contains(cell);
    }

    internal void AddTail(LevelGridCell cell)
    {
        GameObject.Instantiate(m_snakeTailPrefab, cell.Transform);
        m_SnakeTail.Add(cell);
        cell.IsTailPart = true;
    }

    public bool IsHead(LevelGridCell cell)
    {
        return m_SnakeTail.IndexOf(cell) == m_SnakeTail.Count - 1;
    }

    public void Clear()
    {
        foreach (var item in m_SnakeTail)
        {
            item.IsTailPart = false;
        }

        m_SnakeTail.Clear();
    }

    internal LevelGridCell GetHead()
    {
        return m_SnakeTail[m_SnakeTail.Count - 1];
    }

    internal LevelGridCell GetTailEnd()
    {
        return m_SnakeTail[0];
    }

    internal LevelGridCell GetTailMiddle()
    {
        return m_SnakeTail[(int)(m_SnakeTail.Count/2)];
    }
}