using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    public Texture2D levelTexture;
    public GameObject levelPartPrefab;
    public Transform gridRoot;
    public LevelGrid m_LevelGrid;
    public GameObject ballPrefab;
    public bool generateGrid;

    private void Update()
    {
        if (generateGrid)
        {
            generateGrid = false;
            m_LevelGrid.Dispose();
            m_LevelGrid = new LevelGrid(gridRoot, levelPartPrefab, levelTexture);
        }
    }
}

[System.Serializable]
public class LevelGridCell : IDisposable
{
    [field: SerializeField]
    public Transform Transform { get; private set; }
    public LevelGridCell()
    {
    }
    public LevelGridCell(GameObject cellObject)
    {
        this.Transform = cellObject.transform;
    }

    public void Dispose()
    {
        if (Transform)
            GameObject.DestroyImmediate(Transform.gameObject);
    }
}

[System.Serializable]
public class LevelGrid : IDisposable
{
    public Transform gridRoot;
    public GameObject levelPartPrefab;
    public LevelGridCell[] gridItems;

    const float CELL_WIDTH = 0.2f;
    const float CELL_HEIGHT = 0.2f;

    [HideInInspector]
    public int width;

    public LevelGrid() { }

    public LevelGrid(Transform gridRoot, GameObject levelPartPrefab, Texture2D levelTexture)
    {
        this.gridRoot = gridRoot;
        this.levelPartPrefab = levelPartPrefab;

        gridItems = new LevelGridCell[levelTexture.width * levelTexture.height];
        width = levelTexture.width;
        for (int x = 0; x < levelTexture.width; x++)
        {
            for (int y = 0; y < levelTexture.height; y++)
            {
                var pixel = levelTexture.GetPixel(x, y);
                var cellObject = new GameObject($"Cell_{x}_{y}");
                cellObject.transform.parent = gridRoot;
                cellObject.transform.position = new Vector3(x * CELL_WIDTH, 0, y * CELL_HEIGHT);
                LevelGridCell levelGridCell = new LevelGridCell(cellObject);
                gridItems[x * levelTexture.width + y] = levelGridCell;

                if (pixel == Color.black)
                {
                    SpawnGroundOnCell(levelGridCell);
                }
            }
        }
    }

    public LevelGridCell GetCell(int x, int y)
    {
        return gridItems[x * width + y];
    }

    public void SpawnGroundOnCell(LevelGridCell cell)
    {
#if UNITY_EDITOR
      
        GameObject instance = PrefabUtility.InstantiatePrefab(levelPartPrefab) as GameObject;
        instance.transform.parent = cell.Transform;
        instance.transform.localPosition = Vector3.zero;
#else
        GameObject.Instantiate(levelPartPrefab, cell.Transform);
#endif
    }

    public void Dispose()
    {
        foreach (var item in gridItems)
        {
            item.Dispose();
        }
    }
}