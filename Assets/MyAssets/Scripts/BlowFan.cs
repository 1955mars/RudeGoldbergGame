using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowFan : MonoBehaviour {

    public float blowForce = 1.5f;

    private void OnTriggerEnter(Collider coli)
    {
        if (coli.gameObject.CompareTag("Throwable"))
        {
            Rigidbody rigidBody = coli.GetComponent<Rigidbody>();
            rigidBody.velocity = transform.forward * rigidBody.velocity.magnitude * -blowForce;

        }
    }
}
