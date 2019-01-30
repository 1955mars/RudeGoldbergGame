using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour {

    public BallReset ballReset;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Throwable"))
        {
            ballReset.CollectStars(gameObject);
        }
    }
}
