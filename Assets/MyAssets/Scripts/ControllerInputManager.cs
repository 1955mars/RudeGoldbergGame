using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour
{
    public bool is_lhand;
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;

    //Teleporter
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
    public LayerMask laserMask;
    public float yNudgeAmount = 1f; //specific to teleportAimerObject height
    private bool isValidTeleportableLoc = false;
    public Material validTeleportMaterial;
    public Material inValidTeleportMaterial;

    //Grab and Throw
    public float throwForce = 1.5f;

    //Swipe
    public float swipeSum;
    public float touchLast;
    public float touchCurrent;
    public float distance;
    public bool hasSwipedLeft;
    public bool hasSwipedRight;
    public GameObject ObjectMenu;
    public ObjectMenuManager objectMenuManager;

    //Ball
    public BallReset ballReset;

    //For Oculus rift support
    private bool oculus;

    //Is hand busy
    private bool handInUse = false;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();

        if(UnityEngine.XR.XRDevice.model.Contains("Oculus"))
        {
            oculus = true;
            Debug.Log("Oculus detected");
        }
        else
        {
            oculus = false;
            Debug.Log("Oculus not detected");
        }
        //oculus = false;
    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        if (is_lhand)
        {
            //bool isHittingWall = false;
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                laser.gameObject.SetActive(true);
                teleportAimerObject.SetActive(true);

                laser.SetPosition(0, gameObject.transform.position);
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
                {
                    teleportLocation = hit.point;
                    laser.SetPosition(1, teleportLocation);
                    //aimer position
                    teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
                    isValidTeleportableLoc = true;
                    laser.material = validTeleportMaterial;
                }
                else
                {
                    teleportLocation = transform.forward * 15 + transform.position;
                    RaycastHit groundRay;
                    if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                    {
                        teleportLocation = groundRay.point;
                        isValidTeleportableLoc = true;
                    }
                    else
                    {
                        laser.material = inValidTeleportMaterial;
                        isValidTeleportableLoc = false;
                    }
                    laser.SetPosition(1, transform.forward * 15 + transform.position);
                    //aimer position
                    teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);

                }
               
            }
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                if (isValidTeleportableLoc)
                {
                    handInUse = true;
                    player.transform.position = teleportLocation;
                    isValidTeleportableLoc = false;
                }
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);

            }
        }

        else
        {
            if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                Debug.Log("Get touch down called");
                touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            }
            if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
            {
                Debug.Log("Get touch called");
                    ObjectMenu.SetActive(true);

                touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;

                //For oculus rift
                if (oculus)
                {
                    swipeSum = touchCurrent;
                    //Debug.Log("Swipe Sum = " + swipeSum);
                    if (!hasSwipedRight)
                    {
                        if (swipeSum > 0.5f)
                        {
                            Debug.Log("Swipe Sum = " + swipeSum);
                            //swipeSum = 0;
                            SwipeRight();
                            hasSwipedRight = true;
                        }
                    }
                    else
                    {
                        if (swipeSum < 0.5f)
                        {
                            Debug.Log("Swipe Sum = " + swipeSum);
                            Debug.Log("Swiped right");
                            hasSwipedRight = false;
                        }
                    }
                    if (!hasSwipedLeft)
                    {

                        if (swipeSum < -0.5f)
                        {
                            Debug.Log("Swipe Sum = " + swipeSum);
                            //swipeSum = 0;
                            SwipeLeft();
                            hasSwipedLeft = true;
                            hasSwipedRight = false;
                        }
                    }
                    else
                    {
                        if (swipeSum > -0.5f)
                        {
                            Debug.Log("Swipe Sum = " + swipeSum);
                            Debug.Log("Swiped left");
                            hasSwipedLeft = false;
                        }
                    }
                }
                else
                {
                    distance = touchCurrent - touchLast;
                    touchLast = touchCurrent;
                    swipeSum += distance;

                    if (!hasSwipedRight)
                    {
                        if (swipeSum > 0.5f)
                        {
                            swipeSum = 0;
                            SwipeRight();
                            hasSwipedRight = true;
                            hasSwipedLeft = false;
                        }
                    }
                    if (!hasSwipedLeft)
                    {

                        if (swipeSum < 0.5f)
                        {
                            swipeSum = 0;
                            SwipeLeft();
                            hasSwipedLeft = true;
                            hasSwipedRight = false;
                        }
                    }
                }


            }
            if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                Debug.Log("Get touch up called");
                swipeSum = 0;
                touchCurrent = 0;
                touchLast = 0;
                hasSwipedLeft = false;
                hasSwipedRight = false;
                ObjectMenu.SetActive(false);
            }
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //Spawn object currently selected by menu
                SpawnObject();
            }
        }

    }

    void SpawnObject()
    {
        objectMenuManager.SpawnCurrentObject();
    }

    void SwipeLeft()
    {
        objectMenuManager.MenuLeft();
        Debug.Log("SwipeLeft");
    }
    void SwipeRight()
    {
        objectMenuManager.MenuRight();
        Debug.Log("SwipeRight");

    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Throwable") || col.gameObject.CompareTag("Structure"))
        {
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                ThrowObject(col);
            }
            else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                GrabObject(col);
            }
        }
    }

    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        device.TriggerHapticPulse(2000);
        Debug.Log("You are touching down the trigger on an object");
    }

    void ThrowObject(Collider coli)
    {
        coli.transform.SetParent(null);

        if(coli.gameObject.CompareTag("Throwable"))
        {
            ballReset.SetIgnorePlatformEvents(true);
            Rigidbody rigidBody = coli.GetComponent<Rigidbody>();
            rigidBody.isKinematic = false;
            if (ballReset.IsUseronPlatform())
            {
                rigidBody.velocity = device.velocity * throwForce;
                rigidBody.angularVelocity = device.angularVelocity;
            }
        }
        Debug.Log("You have released the trigger");
    }

}


