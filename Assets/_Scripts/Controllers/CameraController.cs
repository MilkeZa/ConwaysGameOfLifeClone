using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    private Camera mainCamera;


    [SerializeField][Range(0, 500)] private float zoomSpeed = 100f;
    [SerializeField][Range(0, 10)] private float translationSpeed = 0.5f;

    private float minimumOrthographicScale = 0.1f;
    private float maximumOrthographicScale = 25f;

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
            // Get the mouse movement on the x, y axes
            float _movementX = Input.GetAxis("Mouse X");
            float _movementY = Input.GetAxis("Mouse Y");

            // Calculate and apply the mouse movement input
            MoveCamera(_movementX, _movementY);
        }
    }

    private void ZoomCamera(float _scrollInput)
    {
        // Calculate the offset produced by the given scroll input
        float _scaleOffset = -_scrollInput * zoomSpeed * Time.deltaTime;

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

    private void MoveCamera(float _movementX, float _movementY)
    {
        // Calculate the offset on each axis and create a new v3 with the values
        Vector3 _offset = new Vector3(
            _movementX * translationSpeed * Time.deltaTime, 
            _movementY * translationSpeed * Time.deltaTime, 
            0f) * mainCamera.orthographicSize;

        // Apply the offset to the camera and uicanvas transforms
        mainCamera.transform.position -= _offset;
        uiCanvas.transform.position -= _offset;
    }
}
