using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{ 
    
    private NetworkInputData _inputData;

    private bool _isTurboPressed;
    private bool _isCheckPressed;
    private bool _isFirePressed;
    private bool _isColorPressed;
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
        _isCheckPressed |= Input.GetKeyDown(KeyCode.Return);
        _isColorPressed |= Input.GetKeyDown(KeyCode.E);
    }

    public NetworkInputData GetLocalInputs()
    {
        _inputData.turboPressed = _isTurboPressed;
        _inputData.checkButton = _isCheckPressed;
        _inputData.changeColor = _isColorPressed;
        _isTurboPressed = false;
        _isCheckPressed = false;
        _isColorPressed = false;
        return _inputData;
    }
}
