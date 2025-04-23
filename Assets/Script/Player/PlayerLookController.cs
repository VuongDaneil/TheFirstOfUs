using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLookController : MonoBehaviour
{
    #region PROPERTIES
    [Header("STAT(s)")]
    public int FOV = 70;
    public float Sensitivity = 100f;
    public Vector2 VerticalAngleClamp = new Vector2(-75f, 75f);

    [Header("OBJECT(s)")]
    public Camera PlayerCamera;
    public Transform PlayerHead;

    [Header("DEBUG")]
    [ReadOnly] public bool Controllable = true;
    private float xRotation = 0f;
    private float yRotation = 0f;


    #endregion

    #region UNITY CORE
    private void Awake()
    {
        if (PlayerCamera == null) PlayerCamera = Camera.main;
        PlayerCamera.fieldOfView = FOV;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }


        if (Controllable)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, VerticalAngleClamp.x, VerticalAngleClamp.y);

            PlayerHead.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
    #endregion
}
