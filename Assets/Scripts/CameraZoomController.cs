using UnityEngine;

// Adding this attribute lets us see the changes in the editor without pressing Play.
[ExecuteAlways]
public class CameraZoomController_V2 : MonoBehaviour
{
    private Camera _camera;

    [Tooltip("CRITICAL: You must add at least one zoom level here. E.g., 5.4 for 1x, 10.8 for 2x.")]
    [SerializeField] private float[] zoomLevels;

    [Tooltip("The starting zoom level index from the array above.")]
    [SerializeField] private int startingZoomIndex = 0;

    // We use a property to ensure changes are always clamped and applied.
    private int _currentZoomIndex;
    private int CurrentZoomIndex
    {
        get { return _currentZoomIndex; }
        set
        {
            // Make sure the new value is valid before applying it
            int newIndex = Mathf.Clamp(value, 0, zoomLevels.Length - 1);

            // Only update the camera if the index has actually changed
            if (_currentZoomIndex != newIndex && _camera != null)
            {
                _currentZoomIndex = newIndex;
                _camera.orthographicSize = zoomLevels[_currentZoomIndex];
                // Debug.Log($"Zoom level changed to: {zoomLevels[_currentZoomIndex]}");
            }
        }
    }

    void OnEnable()
    {
        _camera = GetComponent<Camera>();
        InitializeZoom();
    }

    void Start()
    {
        // This runs only once in Play Mode
        InitializeZoom();
    }

    void Update()
    {
        // --- SAFETY CHECK ---
        // If the array is not set up, do nothing to prevent errors.
        if (zoomLevels == null || zoomLevels.Length == 0)
        {
            return;
        }

        // We only care about input when the application is actually playing.
        if (Application.isPlaying)
        {
            HandleInput();
        }
    }

    private void InitializeZoom()
    {
        // --- SAFETY CHECK ---
        if (zoomLevels == null || zoomLevels.Length == 0)
        {
            if (_camera != null) _camera.orthographicSize = 5f; // A safe default
            return;
        }

        _currentZoomIndex = startingZoomIndex;
        CurrentZoomIndex = _currentZoomIndex; // Use the property to apply the initial size
    }

    private void HandleInput()
    {
        // Check for mouse scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f) // Scrolled Up (Zoom In)
        {
            CurrentZoomIndex--;
        }
        else if (scrollInput < 0f) // Scrolled Down (Zoom Out)
        {
            CurrentZoomIndex++;
        }
    }
}