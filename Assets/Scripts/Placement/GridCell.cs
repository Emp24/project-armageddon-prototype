
using UnityEngine;

public class GridCell
{
    public bool isOccupied { get; set; }
    public bool isAvailable { get; set; }
    public Vector3 position { get; set; }
    public GameObject placedObject { get; set; }
    public GameObject unavailbeCellPrefab;
    public GameObject availableCellPrefab;
    public GridCell(Vector3 position, GameObject placedObject = null, bool isAvailable = true, bool isOccupied = false, GameObject unavailbeCellPrefab = null, GameObject availableCellPrefab = null)
    {
        this.isOccupied = isOccupied;
        this.isAvailable = isAvailable;
        this.position = position;
        this.placedObject = placedObject;
        this.availableCellPrefab = availableCellPrefab;
        this.unavailbeCellPrefab = unavailbeCellPrefab;
    }

}