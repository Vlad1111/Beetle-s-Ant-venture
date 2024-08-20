using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAnimationCollection : MonoBehaviour
{
    public MyAnimation[] animations;
    public bool loadOnStart = true;
    private void Start()
    {
        animations = GetComponentsInChildren<MyAnimation>();
    }
}
