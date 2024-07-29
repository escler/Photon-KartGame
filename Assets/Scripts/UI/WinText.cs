using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinText : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public void ShowScreen()
    {
        _tmp.enabled = true;
    }

    public void HideScreen()
    {
        _tmp.enabled = false;
    }
}
