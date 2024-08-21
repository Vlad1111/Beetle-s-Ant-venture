using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public static PlayerControll Instance;
    private void Awake()
    {
        Instance = this;
    }

    public MovementBehaviour movement;
    public PincerBehaviour pincer;
    public new CameraBehaviour camera;

    private bool lostConcentration = false;
    private bool wasExcapePressed = false;
    private bool blockMovement = false;

    void Update()
    {
        if (!MenuBehaviour.Instance.IsMenuOpened() && !blockMovement)
        {
            var movementInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            movement.SetMovementInput(movementInput);
            movement.JumpPower(Input.GetAxis("Jump"));
            pincer.SetPincerForce(lostConcentration ? 0 : Input.GetAxis("Fire1"));
            camera.SetLookDirection(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), movementInput.x);

            lostConcentration = false;
        }
        else
        {
            movement.SetMovementInput(new Vector2(0, 0));
            movement.JumpPower(0);
            pincer.SetPincerForce(lostConcentration ? 0 : Input.GetAxis("Fire1"));
            camera.SetLookDirection(new Vector2(0, 0), 0);
        }

        if (Input.GetAxis("Cancel") > 0)
        {
            if (!wasExcapePressed)
                MenuBehaviour.Instance.ToggleMenuOnOff();
            wasExcapePressed = true;
        }
        else wasExcapePressed = false;
    }

    public void BreakConcentration()
    {
        lostConcentration = true;
    }

    public void BlockMovement()
    {
        blockMovement = true;
    }
}
