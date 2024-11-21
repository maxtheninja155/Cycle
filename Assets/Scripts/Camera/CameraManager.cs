using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObj;

    [SerializeField]
    private float mouseSensitivity = 100f;

    [SerializeField]
    private float rotationSmoothTime = 0.1f; // Smoothing duration

    private float xRotation = 0f;
    private Quaternion targetPlayerRotation;
    private Quaternion targetCameraRotation;

    private void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;

        // Initialize target rotations
        targetPlayerRotation = playerObj.transform.rotation;
        targetCameraRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        HandlePlayerRotation();
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
    }

    private void HandlePlayerRotation()
    {
        // Get horizontal mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;

        // Calculate target rotation for the player
        targetPlayerRotation *= Quaternion.Euler(0f, mouseX, 0f);

        // Smoothly interpolate the player's rotation
        Rigidbody playerRigidbody = playerObj.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            Quaternion smoothedRotation = Quaternion.Slerp(playerRigidbody.rotation, targetPlayerRotation, rotationSmoothTime / Time.fixedDeltaTime);
            playerRigidbody.MoveRotation(smoothedRotation);
        }
    }

    private void HandleCameraRotation()
    {
        // Get vertical mouse input
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate target rotation for the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 85f); // Clamp to prevent over-rotation
        targetCameraRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Smoothly interpolate the camera's rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetCameraRotation, rotationSmoothTime / Time.deltaTime);
    }
}
