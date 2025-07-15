using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class SnapObjectToGround : MonoBehaviour
{
    public List<Transform> objectsToSnap = new List<Transform>();

    public LayerMask groundLayer;

    public float raycastHeight = 10f;

    public float raycastDistance = 20f;

    [Button("Snap")]
    public void SnapAll()
    {
        foreach (Transform obj in objectsToSnap)
        {
            Vector3 rayOrigin = obj.position + Vector3.up * raycastHeight;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
            {
                Vector3 newPos = hit.point;
                if (NavMesh.SamplePosition(newPos, out NavMeshHit hit2, 2.0f, NavMesh.AllAreas))
                {
                    newPos = hit2.position;
                }
                obj.position = newPos;
            }
        }
    }
}
