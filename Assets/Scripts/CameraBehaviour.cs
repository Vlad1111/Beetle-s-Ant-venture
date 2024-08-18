using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform cameraParent;
    public Vector2 sensitibility;
    private Vector2 movement;

    public void SetLookDirection(Vector2 axes)
    {
        movement = axes;
    }

    void Update()
    {
        var angles = cameraParent.localEulerAngles;
        angles += new Vector3(movement.y * sensitibility.y, movement.x * sensitibility.x, 0) * Time.deltaTime;

        angles.x %= 360;
        if(angles.x > 180)
            angles.x -= 360;
        angles.y %= 360;
        if (angles.y > 180)
            angles.y -= 360;

        if (angles.x > 45)
            angles.x = 45;
        else if (angles.x < -15)
            angles.x = -15;

        if (angles.y > 25)
            angles.y = 25;
        else if (angles.y < -25)
            angles.y = -25;

        cameraParent.localEulerAngles = angles;
    }
}
