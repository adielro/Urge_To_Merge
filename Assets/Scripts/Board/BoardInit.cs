using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes the board grid and loads saved game state.
/// </summary>
public class BoardInit : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int rows, columns;
    [SerializeField] private GameObject board;

    private GridLayoutGroup gridLayoutGroup;

    void Start()
    {
        SetGridLayout();
        GenerateBoard();
        SaveSystem.Instance.LoadGame(); // Load game state after board generation
    }

    private void SetGridLayout()
    {
        gridLayoutGroup = board.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                var spawnedTile = Instantiate(cellPrefab);
                spawnedTile.transform.SetParent(board.transform);
                spawnedTile.name = $"Tile {x} {y}";
            }
        }
    }
}
