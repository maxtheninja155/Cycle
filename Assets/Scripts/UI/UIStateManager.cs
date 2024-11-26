using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController pc;
    [SerializeField]
    private Gun gc;

    public TextMeshProUGUI speedTxt;
    public TextMeshProUGUI stateTxt;
    public TextMeshProUGUI clipSizeText;

    [Header("Reload UI")]
    public Slider reloadSlider;           // The slider to display reload progress
    public GameObject reloadSliderParent; // Parent object to show/hide the slider

    private bool isReloading = false;    // Tracks reload state
    private float reloadTime = 0f;       // Total reload time
    private float reloadElapsed = 0f;   // Time elapsed since reload started

    private void Start()
    {
        // Disable the reload slider on start
        if (reloadSliderParent != null)
        {
            reloadSliderParent.SetActive(false);
        }
    }

    private void Update()
    {
        if (pc != null)
        {
            speedTxt.text = $"Speed: {pc.currentSpeed.ToString("F1")}";
            stateTxt.text = $"State: {pc.currentState}";
        }

        if (gc != null)
        {
            clipSizeText.text = $"{gc.currentBulletCount}/{gc.clipSize}";

            // Check reload state
            if (gc.IsReloading())
            {
                if (!isReloading)
                {
                    StartReloadUI(gc.reloadSpeed);
                }

                UpdateReloadUI();
            }
            else if (isReloading)
            {
                EndReloadUI();
            }
        }
    }

    private void StartReloadUI(float reloadDuration)
    {
        isReloading = true;
        reloadTime = reloadDuration;
        reloadElapsed = 0f;
        reloadSlider.value = 0f;

        if (reloadSliderParent != null)
        {
            reloadSliderParent.SetActive(true);
        }
    }

    private void UpdateReloadUI()
    {
        reloadElapsed += Time.deltaTime;
        reloadSlider.value = reloadElapsed / reloadTime;
    }

    private void EndReloadUI()
    {
        isReloading = false;

        if (reloadSliderParent != null)
        {
            reloadSliderParent.SetActive(false);
        }
    }
}
