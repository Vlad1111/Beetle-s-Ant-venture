using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainPointDetection : MonoBehaviour
{
    public RaycastHit? raycasHit;
    public float distance;
    public LayerMask layerMask;
    private void FixedUpdate()
    {
        var ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var point, distance, layerMask))
        {
            raycasHit = point;
        }
        else raycasHit = null;
    }

    public void OnDrawGizmos()
    {
        var endPosition = transform.position + transform.forward * distance;
        Gizmos.DrawLine(transform.position, endPosition);
        if(raycasHit != null)
            Gizmos.DrawLine(raycasHit.Value.point, raycasHit.Value.point + raycasHit.Value.normal * 0.1f);
    }
}
