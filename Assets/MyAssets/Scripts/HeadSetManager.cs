﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class HeadSetManager : MonoBehaviour
{
    public GameObject viveRig;
    public GameObject oculusRig;
    private bool hmdChosen;

    // Use this for initialization
    void Start()
    {
        if (UnityEngine.XR.XRDevice.model == "vive" || UnityEngine.XR.XRSettings.loadedDeviceName.Contains("Vive"))
        {
            viveRig.SetActive(true);
            oculusRig.SetActive(false);
            hmdChosen = true;
        }
        else if (UnityEngine.XR.XRDevice.model == "oculus" || UnityEngine.XR.XRSettings.loadedDeviceName.Contains("Oculus"))
        {
            oculusRig.SetActive(true);
            viveRig.SetActive(false);
            hmdChosen = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!hmdChosen)
        {
            if (UnityEngine.XR.XRDevice.model == "vive")
            {
                viveRig.SetActive(true);
                oculusRig.SetActive(false);
                hmdChosen = true;
            }
            else if (UnityEngine.XR.XRDevice.model == "oculus")
            {
                oculusRig.SetActive(true);
                viveRig.SetActive(false);
                hmdChosen = true;
            }
        }
        if (!UnityEngine.XR.XRDevice.isPresent)
        {
            hmdChosen = false;
            //Do code to sync up the CameraRigs states 
            //such as position here
        }

    }
}
