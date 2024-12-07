using DG.Tweening;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static UnityEditor.Progress;

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

    [System.NonSerialized]
    public Ball[] balls;

    private void Awake()
    {
        balls = GetComponentsInChildren<Ball>();
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

        GameObject instance = GameObject.Instantiate(levelPartPrefab, cell.Transform);
        cell.HasGround = true;
        var obstacle = instance.GetComponent<BallObstacle>();
        obstacle.destructable = true;
        obstacle.girdCell = cell;
        return instance;
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