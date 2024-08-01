using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private RawImage bg;
    [SerializeField] private TextMeshProUGUI tmp;

    public void ShowScreen()
    {
        bg.enabled = true;
        tmp.enabled = true;
    }

    public void HideScreen()
    {
        bg.enabled = false;
        tmp.enabled = false;
    }
}
