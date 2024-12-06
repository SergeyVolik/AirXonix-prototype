using UnityEngine;

public class Level : MonoBehaviour
{
    public Texture2D levelTexture;
    public GameObject levelPartPrefab;
    public Transform gridRoot;
    private LevelGrid m_LevelGrid;
    public GameObject ballPrefab;

    private void Awake()
    {
        m_LevelGrid = new LevelGrid(gridRoot, levelPartPrefab, levelTexture);
    }
}

public class LevelGridCell
{
    public Transform Transform { get; private set; }

    public LevelGridCell(GameObject cellObject)
    {
        this.Transform = cellObject.transform;
    }
}

public class LevelGrid
{
    private Texture2D levelTexture;
    private Transform gridRoot;
    private GameObject levelPartPrefab;
    private LevelGridCell[,] gridItems;

    const float CELL_WIDTH = 0.2f;
    const float CELL_HEIGHT = 0.2f;


    public LevelGrid(Transform gridRoot, GameObject levelPartPrefab, Texture2D levelTexture)
    {
        this.levelTexture = levelTexture;
        this.gridRoot = gridRoot;
        this.levelPartPrefab = levelPartPrefab;

        gridItems = new LevelGridCell[levelTexture.width, levelTexture.height];

        for (int i = 0; i < levelTexture.width; i++)
        {
            for (int j = 0; j < levelTexture.height; j++)
            {
                var pixel = levelTexture.GetPixel(i, j);
                var cellObject = new GameObject($"Cell_{i}_{j}");
                cellObject.transform.parent = gridRoot;
                cellObject.transform.position = new Vector3(i* CELL_WIDTH, 0, j * CELL_HEIGHT);
                LevelGridCell levelGridCell = new LevelGridCell(cellObject);
                gridItems[i, j] = levelGridCell;

                if (pixel == Color.black)
                {
                    SpawnGroundOnCell(levelGridCell);
                }     
            }
        }
    }

    public void SpawnGroundOnCell(LevelGridCell cell)
    {
        GameObject.Instantiate(levelPartPrefab, cell.Transform);
    }
}