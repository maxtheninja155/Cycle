using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform gunTip;

    Camera mainCamera;

    [Header("Damage Stats")]
    public ushort damage;
    public ushort firstDropoffDamage;
    public ushort secondDropoffDamage;

    public ushort clipSize;
    public ushort currentBulletCount;

    [Header("Range Stats")]
    public ushort maxRange;
    public ushort firstDropOffRange;
    public ushort secondDropOffRange;

    [Header("Speed Stats")]
    public float reloadSpeed; // Time taken to reload in seconds
    public float fireRate;    // Number of shots per second

    [Header("ADS Settings")]
    public Transform adsPosition;         // Transform for the ADS position
    public Transform defaultPosition;    // Transform for the default position
    public float adsTransitionSpeed = 5f; // Speed of ADS transition

    private bool isADS = false;
    private bool isReloading = false;


    private LineRenderer lineRenderer;
    private float nextTimeToFire = 0f;

    void Start()
    {
        // Ensure the camera is at the center of the player's perspective
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        currentBulletCount = clipSize;

        // Set up the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleADS();
    }

    public void Fire()
    {
        if (currentBulletCount <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            currentBulletCount--;

            // Cast a ray from the camera outward
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            Vector3 hitPoint = ray.origin + ray.direction * maxRange;

            if (Physics.Raycast(ray, out hit, maxRange))
            {
                hitPoint = hit.point;

                // Calculate the distance from the gunTip to the hit point
                float distance = Vector3.Distance(gunTip.position, hit.point);

                // Call the Damage method with the hit object and distance
                Damage(hit.collider.gameObject, distance);
            }

            // Update the line renderer for visuals
            RenderBulletPath(hitPoint);
        }
    }

    public void Damage(GameObject objHit, float distance)
    {
        ushort appliedDamage = 0;
        int rangeCategory = 0;

        // Determine the range category based on the distance
        if (distance < firstDropOffRange)
        {
            rangeCategory = 0; // Within first drop-off range
        }
        else if (distance >= firstDropOffRange && distance < secondDropOffRange)
        {
            rangeCategory = 1; // Between first and second drop-off
        }
        else if (distance >= secondDropOffRange && distance <= maxRange)
        {
            rangeCategory = 2; // Between second drop-off and max range
        }
        else
        {
            rangeCategory = 3; // Beyond max range
        }

        // Use a switch statement to set the appropriate damage
        switch (rangeCategory)
        {
            case 0:
                appliedDamage = damage;
                break;
            case 1:
                appliedDamage = firstDropoffDamage;
                break;
            case 2:
                appliedDamage = secondDropoffDamage;
                break;
            default:
                appliedDamage = 0; // No damage beyond max range
                break;
        }

        // Apply damage if the object is hittable
        if (objHit.layer == LayerMask.NameToLayer("Hittable"))
        {
            HealthManager health = objHit.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(appliedDamage);
                Debug.Log($"you just hit: {objHit.name}. At a range of: {distance} Meters");
            }
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


    public void TryReload()
    {
        if (isReloading || currentBulletCount == clipSize) return;

        StartCoroutine(Reload());
    }

    public bool IsReloading()
    {
        return isReloading;
    }


    private void RenderBulletPath(Vector3 hitPoint)
    {
        // Set the start point at the gunTip and the end point at the hit location
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, hitPoint);

        // Enable the line renderer and disable it after a short duration
        lineRenderer.enabled = true;
        StartCoroutine(DisableLineRenderer());
    }

    private IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }

    public void SetADS(bool isAiming)
    {
        isADS = isAiming;
    }

    private void HandleADS()
    {
        // Determine the target position
        Vector3 targetPosition = isADS ? adsPosition.localPosition : defaultPosition.localPosition;

        // Smoothly transition to the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * adsTransitionSpeed);
    }
}
