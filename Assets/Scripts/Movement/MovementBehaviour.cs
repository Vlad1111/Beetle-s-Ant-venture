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
    public float jumpPower;

    public Transform bodyMeshesParent;
    public TerrainPointDetection bodyTerrainCollider;
    public TerrainPointDetection[] feetTerrainCollider;
    public Transform breathingPosition;
    public float breathTime;
    private float curentBreathing;
    public Transform breathBar;
    public Transform breathBubbles;

    private Vector2 movement;
    private float jump;

    void Start()
    {
        curentBreathing = breathTime;
    }

    public void SetMovementInput(Vector2 movement)
    {
        this.movement = movement;
    }

    public void JumpPower(float jump)
    {
        this.jump = jump;
    }

    void Update()
    {
        float submergedFeel = 0;
        foreach (var f in feetTerrainCollider)
            if (f && f.raycasHit != null)
                if (f.raycasHit.Value.point.y <= 0)
                    submergedFeel++;
        submergedFeel /= feetTerrainCollider.Length;

        float distanceToGround;
        Vector3 desiredBodyAngles = Vector3.zero;
        if (bodyTerrainCollider.raycasHit != null)
        {
            var terrainRay = bodyTerrainCollider.raycasHit.Value;
            var pointN = Quaternion.AngleAxis(-transform.localEulerAngles.y, Vector3.up) * terrainRay.normal;
            var newAngles = new Vector3(Mathf.Asin(pointN.z), 0, -Mathf.Asin(pointN.x));
            desiredBodyAngles = newAngles * Mathf.Rad2Deg;

            distanceToGround = (bodyTerrainCollider.transform.position - terrainRay.point).magnitude / bodyTerrainCollider.distance;

            if (jump > 0)
                rb.velocity += (1 - submergedFeel) * Vector3.up * jumpPower * Time.deltaTime;
        }
        else
        {
            if (rb.velocity.y < 0)
                rb.velocity += 2 * Physics.gravity * Time.deltaTime;
            distanceToGround = 1;
        }
        bodyMeshesParent.localRotation = Quaternion.Lerp(bodyMeshesParent.localRotation, Quaternion.Euler(desiredBodyAngles), 2 * Time.deltaTime);
        //bodyMeshesParent.Rotate(desiredAxes, transform.localEulerAngles.y);

        float speed = Mathf.Lerp(baseSpeed, underwaterSpeed, submergedFeel + distanceToGround / 2);
        float maxSpeed = Mathf.Lerp(this.maxSpeed, speed, submergedFeel + distanceToGround / 2);
        var horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (Mathf.Abs(movement.x) > 0)
            rb.velocity += transform.forward * movement.x * speed * (maxSpeed - horizontalSpeed) * Time.deltaTime;
        else rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z) * breackPower * Time.deltaTime;
        transform.localEulerAngles += new Vector3(0, rotationSpeed * movement.y * Time.deltaTime, 0);

        if (breathingPosition.position.y <= 0)
        {
            curentBreathing -= Time.deltaTime;
            breathBubbles.gameObject.SetActive(true);
            if(curentBreathing < 0)
            {
                curentBreathing = 0;
                if(gameObject.name == "Player")
                {
                    GameBehaviour.Instance.PlayerDrwoned();
                }
            }
        }
        else
        {
            curentBreathing += Time.deltaTime / 2;
            if (curentBreathing > breathTime)
                curentBreathing = breathTime;
            breathBubbles.gameObject.SetActive(false);
        }
        if(curentBreathing < breathTime)
        {
            breathBar.gameObject.SetActive(true);
            breathBar.localScale = new Vector3(1, curentBreathing / breathTime, 1);
        }
        else
        {
            breathBar.gameObject.SetActive(false);
        }
    }
}
