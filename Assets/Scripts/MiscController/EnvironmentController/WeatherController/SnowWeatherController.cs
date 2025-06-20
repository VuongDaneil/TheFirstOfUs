using UnityEngine;

public class SnowWeatherController : MonoBehaviour
{
    #region PROPERTIES
    Transform _transform;
    int frameCounter = 0;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        frameCounter = 0;
        _transform = transform;
    }
    private void Update()
    {
        frameCounter++;
        if (frameCounter >= 10)
        {
            _transform.position = Camera.main.transform.position;
            frameCounter = 0;
        }
    }
    #endregion
}