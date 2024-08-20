using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour
{
    public Transform[] bridgeParts;
    public MyAnimation onFinishAnimation;
    private int curentIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Stick")
        {
            Destroy(other.gameObject);
            if (curentIndex < bridgeParts.Length)
            {
                bridgeParts[curentIndex].gameObject.SetActive(true);
                curentIndex++;
                SoundManager.PlaySfxClip("Place");
                if (curentIndex == bridgeParts.Length)
                {
                    curentIndex++;
                    SoundManager.PlaySfxClip("Bell");
                    onFinishAnimation.Play();
                }
            }
        }
    }
}
