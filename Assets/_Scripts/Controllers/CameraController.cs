using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    [SerializeField] private Canvas uiCanvas;
    private Camera mainCamera;

    [SerializeField][Range(0, 500)] private float zoomSpeed = 500f;         // Speed at which camera will zoom in/out
    [SerializeField][Range(0, 500)] private float translationSpeed = 5f;    // Speed at which camera will move side/side, up/down

    [SerializeField] private float minimumOrthographicScale = 0.1f;         // Min. scale (max. zoom) camera will allow
    [SerializeField] private float maximumOrthographicScale = 25f;          // Max. scale (min. zoom) camera will allow

    #endregion

    #region UnityMethods

    private void Start()
    {
        // Grab the main camera object
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Camera can be zoomed in or out by scrolling the scroll wheel on the mouse
        float _scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (_scrollWheelInput != 0f)
        {
            // Calculate and apply the scroll wheel input
            ZoomCamera(_scrollWheelInput);
        }

        // Camera can be moved by pressing and holding the mouses scroll wheel and moving the mouse
        if (Input.GetMouseButton(2))
        {
            // The mouse has movement on both the x and y axes
            float _mouseMovementX = Input.GetAxis("Mouse X");
            float _mouseMovementY = Input.GetAxis("Mouse Y");

            // Calculate and apply the mouse movement input
            MoveCamera(_mouseMovementX, _mouseMovementY);
        }
    }

    #endregion

    /// <summary>
    /// Calculate and apply the new camera orthographic scale (zoom).
    /// </summary>
    /// <param name="_scrollInput">Amount of zoom input.</param>
    private void ZoomCamera(float _scrollInput)
    {
        // Calculate the offset produced by the given scroll input
        float _scaleOffset = -_scrollInput * zoomSpeed * Time.fixedDeltaTime;

        // Clamp the value between the minimum and maximum scales
        float _finalOrthographicScale = Mathf.Clamp(
            _scaleOffset + mainCamera.orthographicSize,
            minimumOrthographicScale,
            maximumOrthographicScale);

        // Create a vector version of the offset for the ui local scale
        Vector3 _finalUIScale = new Vector3(_finalOrthographicScale, _finalOrthographicScale, 1f);

        // Apply the scale to both the camera and the uiCanvas
        mainCamera.orthographicSize = _finalOrthographicScale;
        uiCanvas.transform.localScale = _finalUIScale;
    }

    /// <summary>
    /// Calculate and apply the new camera position.
    /// </summary>
    /// <param name="_movementX">Amount of movement on the x-axis.</param>
    /// <param name="_movementY">Amount of movement on the y-axis.</param>
    private void MoveCamera(float _movementX, float _movementY)
    {
        // Calculate the offset on each axis and create a new v3 with the values
        Vector3 _offset = new Vector3(
            _movementX * translationSpeed * Time.fixedDeltaTime,
            _movementY * translationSpeed * Time.fixedDeltaTime,
            0f) * mainCamera.orthographicSize;

        // Apply the offset to the camera and uicanvas transforms
        mainCamera.transform.position -= _offset;
        uiCanvas.transform.position -= _offset;
    }
}
