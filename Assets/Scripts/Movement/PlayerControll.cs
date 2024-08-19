using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public MovementBehaviour movement;
    public PincerBehaviour pincer;
    public new CameraBehaviour camera;

    private bool lostConcentration = false;

    void Update()
    {
        movement.SetMovementInput(new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")));
        movement.JumpPower(Input.GetAxis("Jump"));
        pincer.SetPincerForce(lostConcentration ? 0 : Input.GetAxis("Fire1"));
        camera.SetLookDirection(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        lostConcentration = false;
    }

    public void BreakConcentration()
    {
        lostConcentration = true;
    }
}
