using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _targetTransform;
    public float offSetZ, offSetX;

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }
    
    void LateUpdate()
    {
        if (!_targetTransform) return;

        var newPos = transform.position;
        newPos.z = _targetTransform.position.z + offSetZ;
        newPos.x = _targetTransform.position.x + offSetX;
        transform.position = newPos;
    }
}
