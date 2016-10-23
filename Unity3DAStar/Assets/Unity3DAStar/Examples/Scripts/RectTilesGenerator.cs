using UnityEngine;

public class RectTilesGenerator : MonoBehaviour
{
    public MeshNode cellPrefab;
    public Camera rayCastCamera = null;
    public GameObject root;
    public bool generateController = true;
    public Vector3 tileSize = new Vector3(10, 10, 0);
    public Vector3 tileOffset = new Vector3(0, 0, 0);
    public int gridWith = 7;
    public int gridHeight = 7;
    public int gridDepth = 1;
    public RunnableQueue runnableQueue; 

    public void Start()
    {
        GenerateNodes();
    }

    private void GenerateNodes()
    {
        root = new GameObject("Grid");
        StarNode<MeshNode>[,,] dataGrid = new StarNode<MeshNode>[gridWith, gridHeight, gridDepth];
        for (int i = 0; i < gridWith; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                for (int k = 0; k < gridDepth; k++)
                {
                    MeshNode cell = Instantiate(cellPrefab) as MeshNode;
                    Vector3 worldPosition = new Vector3((tileSize.x + tileOffset.x) * i, (tileSize.y + tileOffset.y) * j, (tileSize.z + tileOffset.z) * k);
                    cell.transform.parent = root.transform;
                    cell.transform.localPosition = worldPosition;
                    cell.Color = Color.black;
                    cell.Text = "(" + i + ", " + j + ", " + k + ")";
                    dataGrid[i, j, k] = new StarNode<MeshNode>(cell, new IntVector3(i, j, k));
                    cell.StarNode = dataGrid[i, j, k];
                }
            }
        }

        IntVector3[] surroundings = gridDepth > 1 ? StarGrid<MeshNode>.SUROUNDINGS_3D_6 : StarGrid<MeshNode>.SUROUNDINGS_2D_4;
        IHeuristic heuristic = (gridDepth > 1) ? (IHeuristic)Manhattan3D.Instance : (IHeuristic)Manhattan2D.Instance;
        StarGrid<MeshNode> starGrid = new StarGrid<MeshNode>(dataGrid, surroundings, heuristic, runnableQueue);
        if (generateController)
        {
            TestGridController controller = root.AddComponent<TestGridController>();
            controller.Grid = starGrid;
            controller.rayCastCamera = rayCastCamera;
        }
    }
}
