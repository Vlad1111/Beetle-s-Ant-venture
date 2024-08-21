using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAnimation : MonoBehaviour
{
    public Transform subject;
    public AnimationCurve positionX;
    public AnimationCurve positionY;
    public AnimationCurve positionZ;
    public AnimationCurve rotationX;
    public AnimationCurve rotationY;
    public AnimationCurve rotationZ;
    public Vector2 timeMargines;
    public float animationSpeed = 1;
    public bool loop;
    public bool setToOrigin;
    public bool playOnStart;
    public bool animate = false;

    private float curentTime = 0;
    private Vector3 originalPosition;
    private Vector3 originalRotation;

    private void Start()
    {
        if (subject == null)
            subject = transform;
        originalPosition = subject.localPosition;
        originalRotation = subject.localEulerAngles;
        animate = playOnStart;
    }

    public void Play()
    {
        curentTime = 0;
        animate = true;
    }

    void LateUpdate()
    {
        if(animate)
        {
            float time = Mathf.Lerp(timeMargines.x, timeMargines.y, curentTime);

            var positionOffset = new Vector3(positionX.Evaluate(time), positionY.Evaluate(time), positionZ.Evaluate(time));
            positionOffset += originalPosition;

            var rotationOffset = new Vector3(rotationX.Evaluate(time), rotationY.Evaluate(time), rotationZ.Evaluate(time));
            rotationOffset += originalRotation;

            subject.localPosition = positionOffset;
            subject.localEulerAngles = rotationOffset;

            curentTime += Time.deltaTime * animationSpeed;
            if(curentTime > 1)
            {
                if(!loop)
                {
                    if (setToOrigin)
                        subject.localEulerAngles = originalRotation;
                    animate = false;
                }
                curentTime = 0;
            }
        }
    }
}
