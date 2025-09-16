// GridPlacementSystem.cs
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPlacementSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 1f;
    private Dictionary<Vector3, GridCell> gridDictionary = new Dictionary<Vector3, GridCell>();

    [Header("Object to Place")]
    [SerializeField] private GameObject objectToPlacePrefab;

    [SerializeField] private GameObject unavailableCellPrefab;
    private GameObject previewObject;

    private Vector3 snappedMousePosition;
    private Vector3 gridMousePosition;
    // --- NEW TILEMAP VARIABLES ---
    [Header("Grid Visualization")]
    [SerializeField] private Tilemap visualTilemap;
    [SerializeField] private TileBase gridCellTile;
    private GameObject cellUnavailable;
    private List<GameObject> activeCells = new List<GameObject>();
    void Start()
    {
        DrawGridWithTilemap();
        BuildGrid();
    }

    // --- NEW TILEMAP FUNCTION ---
    void DrawGridWithTilemap()
    {
        if (visualTilemap == null || gridCellTile == null)
        {
            Debug.LogWarning("Visual Tilemap or Tile is not assigned. Grid will not be drawn.");
            return;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Tilemaps use Vector3Int for cell positions
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                visualTilemap.SetTile(cellPosition, gridCellTile);
            }
        }
    }
    void BuildGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = new Vector3(x, y, 0) * cellSize;
                Vector3 snappedCellPosition = new Vector3(x, y, 0) * cellSize + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                Debug.Log("cellPosition: " + cellPosition);
                GameObject cellunavailable = Instantiate(unavailableCellPrefab, snappedCellPosition, Quaternion.identity);
                cellunavailable.SetActive(false);
                gridDictionary[cellPosition] = new GridCell(cellPosition, unavailbeCellPrefab: cellunavailable);
            }
        }
    }
    void Update()
    {
        HandleMouseInput();
        HandlePlacementPreview();
        if (gridDictionary.ContainsKey(gridMousePosition) && gridDictionary[gridMousePosition].isOccupied)
        {
            if (gridDictionary[gridMousePosition].unavailbeCellPrefab.activeSelf == false)
            {
                foreach (GameObject cell in activeCells)
                {
                    cell.SetActive(false);
                    activeCells.Remove(cell);
                }
                gridDictionary[gridMousePosition].unavailbeCellPrefab.SetActive(true);
                activeCells.Add(gridDictionary[gridMousePosition].unavailbeCellPrefab);
            }
        }
        else
        {
            foreach (GameObject cell in activeCells)
            {
                cell.SetActive(false);
                activeCells.Remove(cell);
            }
        }
    }

    private void HandleMouseInput()
    {
        // 1. Get mouse position in World Space
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // 2. Snap the position to the grid
        int x = Mathf.FloorToInt(mouseWorldPosition.x / cellSize);
        int y = Mathf.FloorToInt(mouseWorldPosition.y / cellSize);

        // Ensure the snapped position is within the grid bounds (optional but good practice)
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        snappedMousePosition = new Vector3(x, y, 0) * cellSize + new Vector3(cellSize, cellSize, 0) * 0.5f;
        gridMousePosition = new Vector3(x, y, 0) * cellSize;

        // 3. Handle placement on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }

    private void HandlePlacementPreview()
    {
        // If we have a prefab assigned, manage the preview object
        if (objectToPlacePrefab != null)
        {

            if (previewObject == null)
            {

                previewObject = Instantiate(objectToPlacePrefab);

                // Instantiate a preview object if we don't have one
                // Tip: You might want to change its color or make it semi-transparent
                // For example, change its sprite renderer's color:
                // previewObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            }

            // Move the preview object to the snapped mouse position
            previewObject.transform.position = snappedMousePosition;
        }
    }

    private void PlaceObject()
    {
        if (objectToPlacePrefab != null)
        {
            // You would add logic here to check if the cell is already occupied
            if (gridDictionary.ContainsKey(gridMousePosition) && !gridDictionary[gridMousePosition].isOccupied)
            {
                Debug.Log($"Placing object at: {gridMousePosition}");
                Instantiate(objectToPlacePrefab, snappedMousePosition, Quaternion.identity);
                gridDictionary[gridMousePosition].isOccupied = true;
            }
            else
            {
                Debug.Log("Cell is already occupied.");
            }

        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position from the screen and convert it to world coordinates
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = -Camera.main.transform.position.z; // Ensure it's on the same Z-plane as your game
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // This draws a visual grid in the Scene view for easy editing.
    private void OnDrawGizmos()
    {
        // if (Application.isPlaying) return; // Optional: Only draw when not in play mode

        Gizmos.color = Color.gray;
        Vector3 offset = new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);

        // Draw vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }


    void OnMouseDown()
    {

    }
}

