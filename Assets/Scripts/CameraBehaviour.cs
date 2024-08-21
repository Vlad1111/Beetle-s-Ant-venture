using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform cameraParent;
    public Vector2 sensitibility;
    private Vector2 movement;
    private float verticalMovement;
    public void SetLookDirection(Vector2 axes, float verticalMovement)
    {
        movement = axes;
        this.verticalMovement = verticalMovement;
    }

    void Update()
    {
        var angles = cameraParent.localEulerAngles;
        angles += new Vector3(movement.y * sensitibility.y, movement.x * sensitibility.x, 0) * Time.deltaTime * SettingBehaviour.settings.lookSensitivity;

        angles.x %= 360;
        if(angles.x > 180)
            angles.x -= 360;
        angles.y %= 360;
        if (angles.y > 180)
            angles.y -= 360;

        if (angles.x > 45)
            angles.x = 45;
        else if (angles.x < -25)
            angles.x = -25;

        //if (angles.y > 25)
        //    angles.y = 25;
        //else if (angles.y < -25)
        //    angles.y = -25;

        if(verticalMovement > 0)
        {
            angles.y = Mathf.Lerp(angles.y, 0, 3 * Time.deltaTime);
        }
        //else if (verticalMovement < 0)
        //{
        //    if(angles.y < 0)
        //        angles.y = 360 + angles.y;
        //    angles.y = Mathf.Lerp(angles.y, 180, 3 * Time.deltaTime);
        //}

        cameraParent.localEulerAngles = angles;
    }
}
