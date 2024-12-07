using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Countdown
{
    public float duration;
    public float currentTime;

    public event Action onFinished;
    public event Action onTimeChanged;

    public bool paused;
    public bool finished;

    public Countdown(float duration)
    {
        this.duration = duration;
    }

    public void Pause()
    {
        paused = true;
    }
    public void Continue()
    {
        paused = false;
    }

    public void Restart()
    {
        finished = false;
        paused = false;
        currentTime = 0;
    }

    public void Update(float deltaTime)
    {
        if (paused || finished)
            return;

        currentTime += deltaTime;
     
        if (currentTime > duration)
        {
            currentTime = duration;
            finished = true;
            onFinished?.Invoke();
        }
        onTimeChanged?.Invoke();
    }
}

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public Texture2D levelTexture;
    public GameObject levelPartPrefab;
    public Transform gridRoot;
    public LevelGrid m_LevelGrid;
    public GameObject ballPrefab;

    public bool generateGrid;
    public bool destructableObstacles;

    public float levelDuration = 60;
    public event Action<float> onLevelProgressChanged;
    public event Action onLevelFinished;
    private bool m_LevelFinished = false;
    [System.NonSerialized]
    public Ball[] balls;
    private Countdown m_Countdown;
    public Countdown Countdown => m_Countdown;
    List<LevelGridCell> m_VisitedCells = new List<LevelGridCell>();
    List<LevelGridCell> m_BallsCells = new List<LevelGridCell>();

    public int totalProgressTiles;
    public int progressTilesToFinish;
    public int currentTilesProgress;

    private void Awake()
    {
        balls = GetComponentsInChildren<Ball>();
        m_Countdown = new Countdown(levelDuration);
        m_Countdown.Pause();
        StartTimer();
        totalProgressTiles = 0;
        foreach (var item in m_LevelGrid.gridItems)
        {
            if (!item.HasGround)
                totalProgressTiles++;
        }

        progressTilesToFinish = (int)(totalProgressTiles * 0.9f);
        m_LevelGrid.onGroundCreated += M_LevelGrid_onGroundCreated;
        m_LevelGrid.onGroundDestroyed += M_LevelGrid_onGroundDestroyed;
    }

    private void M_LevelGrid_onGroundDestroyed()
    {
        if (m_LevelFinished)
            return;

        currentTilesProgress--;
        onLevelProgressChanged?.Invoke(GetProgress01());
    }

    public float GetProgress01()
    {
        return Mathf.Clamp01((float)currentTilesProgress / progressTilesToFinish);
    }

    public float GetProgressPercent()
    {
        return GetProgress01()*100;
    }

    private void M_LevelGrid_onGroundCreated()
    {
        if (m_LevelFinished)
            return;

        currentTilesProgress++;
        var percent = GetProgress01();
        onLevelProgressChanged?.Invoke(percent);

        if (percent == 1)
        {
            m_LevelFinished = true;
            onLevelFinished?.Invoke();
        }
    }

    private void CollectAllAreaCells(LevelGridCell notGroundCell, out bool areaHasBall)
    {
        m_BallsCells.Clear();
        foreach (var item in balls)
        {
            m_BallsCells.Add(m_LevelGrid.GetClosestCell(item.transform.position));
        }

        areaHasBall = false;
        VisitAreaCell(notGroundCell, ref areaHasBall);
    }

    public void StartTimer()
    {
        m_Countdown.Restart();
    }

    private void VisitAreaCell(LevelGridCell cell, ref bool areaHasBall)
    {
        if (areaHasBall)
            return;

        foreach (var item in m_BallsCells)
        {
            if (cell == item)
            {
                areaHasBall = true;
                return;
            }
        }

        if (cell.HasGround)
            return;

        if (cell.IsAlreadyVisited)
            return;

        cell.IsAlreadyVisited = true;
        m_VisitedCells.Add(cell);

        foreach (var item in m_LevelGrid.IterNeighbours(cell))
        {
            VisitAreaCell(item, ref areaHasBall);
        }
    }

    IEnumerator TryFillArea_CO(params LevelGridCell[] fillAreaCells)
    {
        foreach (var fillCell in fillAreaCells)
        {
            foreach (var cell in m_LevelGrid.IterNeighbours(fillCell))
            {
                if (!cell.HasGround)
                {
                    var notGroundCell = cell;
                    CollectAllAreaCells(notGroundCell, out bool hasBall);

                    if (!hasBall)
                    {
                        foreach (var item2 in m_VisitedCells)
                        {
                            item2.IsAlreadyVisited = false;
                            m_LevelGrid.SpawnGroundOnCell(item2, 0.6f);
                        }
                    }
                    else
                    {
                        foreach (var item2 in m_VisitedCells)
                        {
                            item2.IsAlreadyVisited = false;
                        }
                    }

                    m_VisitedCells.Clear();
                }
            }

            yield return null;
        }
    }

    public void TryFillArea(params LevelGridCell[] fillAreaCells)
    {
        StartCoroutine(TryFillArea_CO(fillAreaCells));

       
    }

    private void Update()
    {      
#if UNITY_EDITOR
        if (generateGrid)
        {
            generateGrid = false;
            m_LevelGrid.Dispose();
            m_LevelGrid = new LevelGrid(gridRoot, levelPartPrefab, levelTexture, destructableObstacles);
        }
#endif
        if(m_Countdown != null)
            m_Countdown.Update(Time.deltaTime);
    }
}

[System.Serializable]
public class LevelGridCell : IDisposable
{
    [field: SerializeField]
    public Transform Transform { get; private set; }
    [field: SerializeField]
    public bool HasGround { get; set; }

    public bool IsTailPart { get; set; }

    public int leftNeighbourIndex = -1;
    public int rightNeighbourIndex = -1;
    public int topNeighbourIndex = -1;
    public int downNeighbourIndex = -1;

    public bool IsAlreadyVisited { get; set; }
    public LevelGridCell() { }

    public LevelGridCell(GameObject cellObject)
    {
        this.Transform = cellObject.transform;
    }

    public void Dispose()
    {
        if (Transform)
            GameObject.DestroyImmediate(Transform.gameObject);
    }

    internal void ClearCell()
    {
        foreach (Transform item in Transform)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}

[System.Serializable]
public class LevelGrid : IDisposable
{
    public Transform gridRoot;
    public GameObject levelPartPrefab;
    private bool destructableObstacles;
    public LevelGridCell[] gridItems;

    const float CELL_WIDTH = 0.2f;
    const float CELL_HEIGHT = 0.2f;

    [HideInInspector]
    public int width;
    private int height;

    public event Action onGroundCreated;
    public event Action onGroundDestroyed;

    public LevelGrid() { }
#if UNITY_EDITOR
    public LevelGrid(Transform gridRoot, GameObject levelPartPrefab, Texture2D levelTexture, bool destructableObstacles)
    {
        this.gridRoot = gridRoot;
        this.levelPartPrefab = levelPartPrefab;
        this.destructableObstacles = destructableObstacles;
        gridItems = new LevelGridCell[levelTexture.width * levelTexture.height];
        width = levelTexture.width;
        height = levelTexture.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pixel = levelTexture.GetPixel(x, y);
                var cellObject = new GameObject($"Cell_{x}_{y}");
                cellObject.transform.parent = gridRoot;
                cellObject.transform.position = new Vector3(x * CELL_WIDTH, 0, y * CELL_HEIGHT);
                LevelGridCell levelGridCell = new LevelGridCell(cellObject);
                gridItems[x * width + y] = levelGridCell;

                if (pixel == Color.black)
                {
                    SpawnGroundInEditor(levelGridCell);
                }
            }
        }

        ConnectCells();
    }
#endif

    private void ConnectCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = gridItems[x * width + y];
                int cellTop = -1;
                int cellDown = -1;
                int cellLeft = -1;
                int cellRight = -1;

                if (y != height - 1)
                {
                    cellTop = x * width + y + 1;
                }
                if (y != 0)
                {
                    cellDown = x * width + y - 1;
                }

                if (x != height - 1)
                {
                    cellRight = (x + 1) * width + y;
                }
                if (x != 0)
                {
                    cellLeft = (x - 1) * width + y;
                }

                cell.topNeighbourIndex = cellTop;
                cell.downNeighbourIndex = cellDown;
                cell.leftNeighbourIndex = cellLeft;
                cell.rightNeighbourIndex = cellRight;
            }
        }
    }

    public IEnumerable<LevelGridCell> IterNeighbours(LevelGridCell cell)
    {
        if (cell.leftNeighbourIndex != -1)
            yield return gridItems[cell.leftNeighbourIndex];
        if (cell.rightNeighbourIndex != -1)
            yield return gridItems[cell.rightNeighbourIndex];
        if (cell.topNeighbourIndex != -1)
            yield return gridItems[cell.topNeighbourIndex];
        if (cell.downNeighbourIndex != -1)
            yield return gridItems[cell.downNeighbourIndex];
    }

    public LevelGridCell GetCell(int x, int y)
    {
        return gridItems[x * width + y];
    }

    public GameObject SpawnGroundOnCell(LevelGridCell cell, float showTime)
    {
        var item = SpawnGroundOnCell(cell);

        item.transform.localPosition = new Vector3(0, -0.5f, 0);
        item.transform.DOLocalMoveY(0, showTime).SetEase(Ease.OutSine);

        return item;
    }

#if UNITY_EDITOR
    public GameObject SpawnGroundInEditor(LevelGridCell cell)
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(levelPartPrefab) as GameObject;
        instance.transform.parent = cell.Transform;
        instance.transform.localPosition = Vector3.zero;

        return instance;
    }
#endif
    public GameObject SpawnGroundOnCell(LevelGridCell cell)
    {
        onGroundCreated?.Invoke();
        GameObject instance = GameObject.Instantiate(levelPartPrefab, cell.Transform);
        cell.HasGround = true;
        var obstacle = instance.GetComponent<BallObstacle>();
        obstacle.onDestroyed += Obstacle_onDestroyed;
        obstacle.destructable = true;
        obstacle.girdCell = cell;
        return instance;
    }

    private void Obstacle_onDestroyed()
    {
        onGroundDestroyed?.Invoke();
    }

    public void Dispose()
    {
        foreach (var item in gridItems)
        {
            item.Dispose();
        }
    }

    internal LevelGridCell GetClosestCell(Vector3 position)
    {
        float minDistance = float.MaxValue;
        LevelGridCell closestCell = null;
        foreach (var item in gridItems)
        {
            var dist = Vector3.Distance(position, item.Transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestCell = item;
            }
        }

        return closestCell;
    }
}