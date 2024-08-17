using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public MovementBehaviour movement;
    public PincerBehaviour pincer;
    void Update()
    {
        movement.SetMovementInput(new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")));
        pincer.SetPincerForce(Input.GetAxis("Fire1"));
    }
}
