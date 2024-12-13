using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterController : MonoBehaviour
{
    public float speed = 1f;
    private Transform m_Transform;
    private Level m_Level;
    private Vector2 m_SnakeDirection;
    private SnakeTail m_SnakeTail;
    public SnakeTail SnakeTail => m_SnakeTail;

    public GameObject tailPrefab;
    public event Action onSnakeSelfCollision;

    private GameManager m_gameManager;

    [Inject]
    void Construct(GameManager gameManager)
    {
        m_gameManager = gameManager;
    }

    public void BindLevel(Level level)
    {
        m_Level = level;
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

        var cell = m_Level.Grid.GetClosestCell(m_Transform.position);

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

        m_SnakeTail.Update();
    }

    private void ClearSnake()
    {
        if (m_SnakeDirection == Vector2.zero)
            return;

        m_SnakeDirection = Vector2.zero;

        for (int i = 0; i < m_SnakeTail.TailCells.Count; i++)
        {
            if (m_SnakeTail.TailInstance[i] != null)
                m_Level.Grid.SpawnGroundOnCell(m_SnakeTail.TailCells[i]);
        }
       
        m_Level.TryFillArea(m_SnakeTail.TailCells.ToArray());
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
            onSnakeSelfCollision?.Invoke();
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
    private List<GameObject> m_SnakeTailInstances = new List<GameObject>();

    public List<LevelGridCell> TailCells => m_SnakeTail;
    public List<GameObject> TailInstance => m_SnakeTailInstances;


    private GameObject m_snakeTailPrefab;
    private bool tailDestroyProcessStarted;
    private int startTailIndex;
    private int destoryPrevIndex;
    private int destroyNextIndex;

    public event Action onSnakeHeadDestroyed;
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
        var instance = GameObject.Instantiate(m_snakeTailPrefab, cell.Transform);
        m_SnakeTail.Add(cell);
        m_SnakeTailInstances.Add(instance);
        var index = m_SnakeTail.Count-1;
        instance.GetComponent<TailPart>().onCollidedWithBall += ()=> {
            StartSnakeDestroyProcess(index);
        };
       
        cell.IsTailPart = true;
    }

    private void StartSnakeDestroyProcess(int startTailIndex)
    {
        if (tailDestroyProcessStarted)
            return;

        tailDestroyProcessStarted = true;
        this.startTailIndex = startTailIndex;
        this.destoryPrevIndex = startTailIndex - 1;
        this.destroyNextIndex = startTailIndex + 1;

        GameObject.Destroy(TailInstance[startTailIndex]);
    }

    public bool IsHead(LevelGridCell cell)
    {
        return m_SnakeTail.IndexOf(cell) == m_SnakeTail.Count - 1;
    }

    public void Clear()
    {
        for (int i = 0; i < m_SnakeTail.Count; i++)
        {
            m_SnakeTail[i].IsTailPart = false;
            GameObject.Destroy(m_SnakeTailInstances[i]);
        }
        destoryTick = 0;
        tailDestroyProcessStarted = false;
        m_SnakeTailInstances.Clear();
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

    float destoryTick;

    public void Update()
    {
        const float destoryTickDuraction = 0.03f;

        if (tailDestroyProcessStarted)
        {
            destoryTick += Time.deltaTime;

            if (destoryTick > destoryTickDuraction)
            {
                destoryTick = 0;
                bool canDestoryPrev = destoryPrevIndex >= 0;
                bool canDestoryNext = destroyNextIndex <= m_SnakeTailInstances.Count - 1;

                if (canDestoryPrev)
                {
                    GameObject.Destroy(m_SnakeTailInstances[destoryPrevIndex]);
                    destoryPrevIndex--;
                }

                if (canDestoryNext)
                {
                    if (destroyNextIndex == m_SnakeTailInstances.Count - 1)
                    {
                        onSnakeHeadDestroyed?.Invoke();
                    }
                    GameObject.Destroy(m_SnakeTailInstances[destroyNextIndex]);
                    destroyNextIndex++;
                   
                }

                tailDestroyProcessStarted = canDestoryPrev || canDestoryNext;
            }
        }
    }
}