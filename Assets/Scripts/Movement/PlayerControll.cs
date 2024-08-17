using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public MovementBehaviour movement;

    void Update()
    {
        movement.SetMovementInput(new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")));
    }
}
