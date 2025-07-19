using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterWingRotating : MonoBehaviour
{
    public Transform WingTransform;
    public float RotationSpeed = 10f;

    private void Update()
    {
        WingTransform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }
}
