using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusControllerInputHandler: MonoBehaviour
{

    public OVRInput.Controller thisController;

    //Teleporter
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation;
    public GameObject player;
    public LayerMask laserMask;
    public float yNudgeAmount ; //specific to teleportAimerObject height
    private bool isValidTeleportableLoc = false;
    public Material validTeleportMaterial;
    public Material inValidTeleportMaterial;

    //Grab and Throw
    public float throwForce = 1.5f;

    //Swipe
    private float swipeSum;
    private float touchLast;
    private float touchCurrent;
    private float distance;
    private bool hasSwipedLeft;
    private bool hasSwipedRight;
    public GameObject ObjectMenu;
    public ObjectMenuManager objectMenuManager;
    private bool menuIsSwipable;
    private float menuStickX;

    //Ball
    public BallReset ballReset;

    //For Oculus rift support
    private bool oculus;

    //Is hand busy
    private bool handInUse = false;
    private bool grabbing = false;

    // Use this for initialization
    void Start()
    {
        //trackedObj = GetComponent<SteamVR_TrackedObject>();
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

        if (thisController == OVRInput.Controller.LTouch)
        {
            //bool isHittingWall = false;
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
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
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (isValidTeleportableLoc)
                {
                    player.transform.position = teleportLocation + new Vector3(0f, yNudgeAmount+0.5f, 0f);
                    isValidTeleportableLoc = false;
                }
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);

            }
        }

        else
        {
            if(OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, thisController))
            {
                Debug.Log("Touch");
                ObjectMenu.SetActive(true);
            }
            else
            {
                Debug.Log("No Touch");
                ObjectMenu.SetActive(false);
            }

            menuStickX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, thisController).x;

            Debug.Log("menuStickx = " + menuStickX);

            if (menuStickX < 0.45f && menuStickX > -0.45f)
            {
                menuIsSwipable = true;
                //ObjectMenu.SetActive(true);
            }

            if (menuIsSwipable)
            {
                if(menuStickX >= 0.45f)
                {
                    objectMenuManager.MenuRight();
                    menuIsSwipable = false;
                }
                else if (menuStickX <= -0.45f)
                {
                    objectMenuManager.MenuLeft();
                    menuIsSwipable = false;
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, thisController))
            {
                //Spawn object currently selected by menu
                Debug.Log("Thumbstick button is pressed");
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
            //Debug.Log("trigger value:-" + OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController));
            if (grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) < 0.5f)
            {
                ThrowObject(col);
                grabbing = false;
            }
            else if (!grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) > 0.5f)
            {
                GrabObject(col);
                grabbing = true;
            }
        }
    }

    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform);
        coli.GetComponent<Rigidbody>().isKinematic = true;
        
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
               rigidBody.velocity = OVRInput.GetLocalControllerVelocity(thisController) * throwForce;
               rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(thisController);
            }
        }
        Debug.Log("You have released the trigger");
    }

}


