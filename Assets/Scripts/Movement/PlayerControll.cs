using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public MovementBehaviour movement;
    public PincerBehaviour pincer;
    public new CameraBehaviour camera;
    void Update()
    {
        movement.SetMovementInput(new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")));
        pincer.SetPincerForce(Input.GetAxis("Fire1"));
        camera.SetLookDirection(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }
}
