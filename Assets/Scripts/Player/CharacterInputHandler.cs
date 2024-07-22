using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{ 
    
    private NetworkInputData _inputData;

    private bool _isTurboPressed;
    private bool _isFirePressed;
    void Start()
    {
        _inputData = new NetworkInputData();

    }

    // Update is called once per frame
    void Update()
    {
        _inputData.horizontalInput = Input.GetAxis("Horizontal");
        _inputData.verticalInput = Input.GetAxis("Vertical");
        _isTurboPressed |= Input.GetKey(KeyCode.Space);
    }

    public NetworkInputData GetLocalInputs()
    {
        _inputData.turboPressed = _isTurboPressed;
        _isTurboPressed = false;
        return _inputData;
    }
}
