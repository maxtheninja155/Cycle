using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController pc;
    [SerializeField]
    private Gun gc;

    public TextMeshProUGUI speedTxt;
    public TextMeshProUGUI stateTxt;

    public TextMeshProUGUI clipSizeText;

    private void Update()
    {
        if (pc != null)
        {
            speedTxt.text = $"Speed: {pc.currentSpeed.ToString().Substring(0, Mathf.Min(3, pc.currentSpeed.ToString().Length))}";
            stateTxt.text = $"State: {pc.currentState.ToString()}";

        }

        if(gc != null)
        {
            clipSizeText.text = $"{gc.currentBulletCount}/{gc.clipSize}";

        }
    }

}
