using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCompassUI : MonoBehaviour
{
    #region PROPERTIES
    public static ArrowCompassUI Instance { get; private set; }

    public Transform target;
    public RectTransform arrow;
    public Camera mainCamera;
    private Transform camTransform;

    private int frameCounter = 0;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        camTransform = mainCamera.transform;
    }
    void Update()
    {
        if (frameCounter >= 2)
        {
            frameCounter = 0;
            IndicatorHanlde();
        }
        else frameCounter++;
    }
    #endregion

    #region MAIN
    private void IndicatorHanlde()
    {
        if (target == null) return;

        Vector3 dir = target.position - camTransform.position;
        Vector3 flatDir = new Vector3(dir.x, 0, dir.z);

        float angle = Vector3.SignedAngle(camTransform.forward, flatDir, Vector3.up);
        arrow.localEulerAngles = new Vector3(0, 0, -angle);
    }
    #endregion

    #region SUPPORTIVE
    public void SetTarget(Transform newTarget) => target = newTarget;
    #endregion
}
