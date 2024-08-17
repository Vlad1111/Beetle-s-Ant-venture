using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDetector : MonoBehaviour
{
    public string searchForTag;
    public Collider otherCollider;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == searchForTag)
            otherCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == searchForTag)
            otherCollider = null;
    }
}
