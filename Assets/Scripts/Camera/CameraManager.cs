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

    [SerializeField]
    private float maxAngularVelocity; // Maximum angular velocity for player rotation

    private float xRotation = 0f;
    private Quaternion targetCameraRotation;
    private Rigidbody playerRigidbody;

    private Camera mainCamera;


    [Header("ASD Settings")]
    [SerializeField]
    private float normalFOV = 60f;        // Normal FOV
    [SerializeField]
    private float adsFOV = 40f;          // FOV while aiming down sights
    [SerializeField]
    private float adsTransitionSpeed = 5f; // Speed of FOV transition

    private bool isADS = false;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;

        // Cache the player's Rigidbody component
        playerRigidbody = playerObj.GetComponent<Rigidbody>();

        // Initialize target camera rotation
        targetCameraRotation = transform.localRotation;

        // Set the maximum angular velocity for the Rigidbody
        if (playerRigidbody != null)
        {
            maxAngularVelocity = mouseSensitivity / 2;
            playerRigidbody.maxAngularVelocity = maxAngularVelocity;
        }
    }

    private void FixedUpdate()
    {
        HandlePlayerRotation();
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleFOV();
    }

    private void HandlePlayerRotation()
    {
        if (playerRigidbody == null) return;

        // Get horizontal mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        // Calculate angular velocity for smooth rotation
        Vector3 angularVelocity = new Vector3(0f, mouseX * Mathf.Deg2Rad, 0f);
        playerRigidbody.angularVelocity = angularVelocity;
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


    public void SetADS(bool isAiming)
    {
        isADS = isAiming;
    }

    private void HandleFOV()
    {
        // Determine the target FOV
        float targetFOV = isADS ? adsFOV : normalFOV;

        // Smoothly transition to the target FOV
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * adsTransitionSpeed);
    }

}
