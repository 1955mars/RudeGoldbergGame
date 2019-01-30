using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformCheck : MonoBehaviour
{

    public BallReset ballReset;

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable"))
        {
            Debug.Log("PlayerPlatformCheck - OnTriggerEnter");
            ballReset.PlatformEvent(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable"))
        {
            Debug.Log("PlayerPlatformCheck - OnTriggerExit");
            ballReset.PlatformEvent(false);
        }
    }
}
