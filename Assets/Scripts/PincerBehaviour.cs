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

    public MyAnimationCollection closeAnimation;
    public MyAnimationCollection openAnimation;

    private SpringJoint joint;
    private float force;
    public void SetPincerForce(float value)
    {
        force = value;
    }

    private bool hasPinced = true;
    private bool hasClosed = false;
    void Update()
    {
        if (force > 0)
        {
            if (hasPinced == false)
            {
                foreach (var anim in closeAnimation.animations)
                    anim.Play();
                hasClosed = true;
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

        if (joint == null && hasClosed)
            if (closeAnimation.animations.Length > 0 && !closeAnimation.animations[0].animate)
            {
                foreach (var anim in openAnimation.animations)
                    anim.Play();
                hasClosed = false;
            }
    }
}
