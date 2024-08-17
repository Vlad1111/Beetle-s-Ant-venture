using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PincerBehaviour : MonoBehaviour
{
    public OnTriggerDetector stickDetector;
    public Vector3 jointAnchor;
    public float springPower;
    public float massMultiplayer;
    public float breakingForce;

    private SpringJoint joint;
    private float force;
    public void SetPincerForce(float value)
    {
        force = value;
    }


    private bool hasPinced = true;
    void Update()
    {
        if (force > 0)
        {
            if (hasPinced == false)
            {
                if (stickDetector.otherCollider != null)
                {
                    var rb = stickDetector.otherCollider.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        joint = gameObject.AddComponent<SpringJoint>();
                        joint.anchor = jointAnchor;
                        joint.spring = springPower;
                        joint.massScale = massMultiplayer;
                        joint.breakForce = breakingForce;
                        joint.connectedBody = rb;
                    }
                }
            }
            hasPinced = true;
        }
        else
        {
            if (hasPinced)
            {
                if (joint)
                    Destroy(joint);
                hasPinced = false;
            }
        }
    }
}
