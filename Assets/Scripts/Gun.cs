using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform gunTip;

    Camera mainCamera;

    [Header("Damage Stats")]
    public ushort damage;
    public ushort rangeFirstCutoffDamageReduction;
    public ushort rangeSecondCutoffDamageReduction;

    public ushort clipSize;
    public ushort currentBulletCount; // Made public for UI access

    [Header("Range Stats")]
    public ushort maxRange;
    public ushort minRange;

    public ushort rangeFirstCutoffDistance;
    public ushort rangeSecondCutoffDistance;

    [Header("Speed Stats")]
    public float reloadSpeed; // Time taken to reload in seconds
    public float fireRate; // Number of shots per second

    // Colors to alternate between for the ray
    private Color[] bulletColors = { Color.green, Color.blue, Color.yellow, Color.red };
    private byte currentColorIndex = 0; // Using byte for optimization

    // Timer to control firing rate
    private float nextTimeToFire = 0f;

    private bool isReloading = false;

    // Line renderer for visualizing the raycast
    private LineRenderer lineRenderer;

    /*
     *                                 DEBUGGING
     * =======================================================================
     */

    void Start()
    {
        // Ensure the camera is at the center of the player's perspective
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        // Initialize current bullet count to the full clip size
        currentBulletCount = clipSize;

        // Set up the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = bulletColors[currentColorIndex];
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {

    }

    public void Fire()
    {
        if (currentBulletCount <= 0)
        {
            // Start the reload process if there are no bullets left
            StartCoroutine(Reload());
            return;
        }

        // Check if the gun can fire based on the fire rate
        if (Time.time >= nextTimeToFire)
        {
            // Update the next time the gun can fire
            nextTimeToFire = Time.time + 1f / fireRate;

            // Reduce the current bullet count by 1
            currentBulletCount--;

            // Cast a ray from the center of the camera outward
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            // Draw the ray in the game view using LineRenderer
            Vector3 endPosition = ray.origin + ray.direction * maxRange;

            if (Physics.Raycast(ray, out hit, maxRange))
            {
                endPosition = hit.point;

                Debug.Log($"Hit: {hit.collider.name}");

                // Check if the hit object has the "Hittable" layer or tag
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Hittable"))
                {
                    // Disable the hit game object
                    hit.collider.gameObject.SetActive(false);
                    Debug.Log($"Hittable object disabled: {hit.collider.name}");
                }
            }

            // Set the positions for the line renderer
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, endPosition);
            lineRenderer.startColor = bulletColors[currentColorIndex];
            lineRenderer.endColor = bulletColors[currentColorIndex];
            lineRenderer.enabled = true;

            // Alternate the color for the next shot
            currentColorIndex = (byte)((currentColorIndex + 1) % bulletColors.Length);

            // Disable the line renderer after a short delay to simulate a shot
            StartCoroutine(DisableLineRenderer());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Wait for the duration of the reload speed
        yield return new WaitForSeconds(reloadSpeed);

        // Refill the current bullet count
        currentBulletCount = clipSize;
        Debug.Log("Reload complete!");

        isReloading = false;
    }

    private IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.1f); // Time the line is visible
        lineRenderer.enabled = false;
    }
}
