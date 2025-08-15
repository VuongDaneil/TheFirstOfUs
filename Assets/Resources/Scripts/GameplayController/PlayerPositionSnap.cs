using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionSnap : MonoBehaviour
{
    public Transform PlayerTransform;
    public CharacterController PlayerMovement;
    public List<Transform> ListPositions;

    int currentPosIndex = 0;

    private void OnEnable()
    {
        currentPosIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            try
            {
                PlayerMovement.enabled = false;
                PlayerTransform.position = ListPositions[currentPosIndex].position;
                PlayerMovement.enabled = true;
            }
            catch { }

            currentPosIndex++;
            if (currentPosIndex >= ListPositions.Count) currentPosIndex = 0;
        }
    }
}
