using UnityEngine;
using System.Collections;

public class ProjectLaser : MonoBehaviour
{
	public GameObject laser;
	public float offset = 0.1f;

	private GameObject laserInstance;

	Transform thisTransform;
	Transform currentLaserDot;

    private void Awake()
    {
		thisTransform = transform;
    }
    private void OnDisable()
    {
        if (laserInstance) laserInstance.SetActive(false);
    }
    void Update ()
	{
		RaycastHit hit;
		if(Physics.Raycast(thisTransform.position, thisTransform.forward,out hit))
		{
			if(laserInstance == null)
			{
				laserInstance = Instantiate(laser,hit.point + hit.normal*offset,Quaternion.identity) as GameObject;
				currentLaserDot = laserInstance.transform;

            }
			else
			{
				laserInstance.SetActive(true);
                currentLaserDot.position = hit.point + hit.normal*offset;
			}
		}

		else
		{
			if(laserInstance != null)
			{
				laserInstance.SetActive(false);
			}
		}
	}
}
