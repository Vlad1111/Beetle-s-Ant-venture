using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    public float maxSpeed;
    public float baseSpeed;
    public float breackPower;
    public float underwaterSpeed;
    public float rotationSpeed;

    public Transform bodyMeshesParent;
    public TerrainPointDetection bodyTerrainCollider;

    private Vector2 movement;

    void Start()
    {
        
    }

    public void SetMovementInput(Vector2 movement)
    {
        this.movement = movement;
    }

    void Update()
    {
        float speed = baseSpeed;
        var horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (Mathf.Abs(movement.x) > 0)
            rb.velocity += transform.forward * movement.x * speed * (maxSpeed - horizontalSpeed) * Time.deltaTime;
        else rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z) * breackPower * Time.deltaTime;
        transform.localEulerAngles += new Vector3(0, rotationSpeed * movement.y * Time.deltaTime, 0);

        //if(Mathf.Abs(movement.y) > 0)
        //{
        //    rb.angularVelocity += new Vector3(0, rotationSpeed * movement.y * Time.deltaTime, 0);
        //    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //}
        //else
        //{
        //    rb.constraints = RigidbodyConstraints.FreezeRotation;
        //}

        if (rb.velocity.y < 0)
            rb.velocity += new Vector3(0, rb.velocity.y, 0) * Time.deltaTime;

        Vector3 desiredBodyAngles = Vector3.zero;
        if(bodyTerrainCollider.raycasHit != null)
        {
            var terrainRay = bodyTerrainCollider.raycasHit.Value;
            var pointN = Quaternion.AngleAxis(-transform.localEulerAngles.y, Vector3.up) * terrainRay.normal;
            var newAngles = new Vector3(Mathf.Asin(pointN.z), 0, -Mathf.Asin(pointN.x));
            desiredBodyAngles = newAngles * Mathf.Rad2Deg;
        }
        bodyMeshesParent.localRotation = Quaternion.Lerp(bodyMeshesParent.localRotation, Quaternion.Euler(desiredBodyAngles), 2 * Time.deltaTime);
        //bodyMeshesParent.Rotate(desiredAxes, transform.localEulerAngles.y);
    }
}
