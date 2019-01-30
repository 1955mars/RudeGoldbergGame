using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {

    //public GameObject collectibles;
    private Vector3 initPosition;
    public List<GameObject> stars;
    private int totalStars;
    private int starsCollected = 0;

    //LevelLoader
    public SteamVR_LoadLevel levelLoader;

    //Materials
    public Material activeMaterial;
    public Material inActiveMaterial;

    //On Platform
    public bool userOnPlatform = true;
    private bool ignorePlatformEvents = false;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        initPosition = gameObject.transform.position;
        totalStars = stars.Count;
        InActiveColor();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision enter");
        print("Tag = " + collision.gameObject.tag);
        if(collision.gameObject.tag == "Ground")
        {
            rb.velocity = new Vector3(0,0,0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            gameObject.transform.position = initPosition;
            ResetCollectibles();
            InActiveColor();
            SetIgnorePlatformEvents(false);
        }
        else if (collision.gameObject.tag == "Goal")
        {
            if (starsCollected == totalStars)
            {
                levelLoader.Trigger();
            }
        }
    }

    public void CollectStars(GameObject starObject)
    {
        if(userOnPlatform)
        {
            starObject.SetActive(false);
            starsCollected++;
        }
        
    }

    void ResetCollectibles()
    {
        Debug.Log("Reset Collectibles called");
        foreach(GameObject obj in stars)
        {
            obj.SetActive(true);
        }
        starsCollected = 0;
    }

    public void InActiveColor()
    {
        Debug.Log("InActiveColor called");
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = inActiveMaterial;
    }

    public void ActiveColor()
    {
        Debug.Log("ActiveColor called");
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material = activeMaterial;
    }

    public void PlatformEvent(bool onPlatform)
    {
        Debug.Log("Platform event called" + onPlatform);
        if(ignorePlatformEvents)
        {
            return;
        }
        userOnPlatform = onPlatform;
        if (onPlatform)
        {
            InActiveColor();
        }
        else
        {
            ActiveColor();
        }
    }

    public bool IsUseronPlatform()
    {
        return userOnPlatform;
    }

    public void SetIgnorePlatformEvents(bool value)
    {
        Debug.Log("SEtIgnorePlatformEvents called");
        ignorePlatformEvents = value;
    }
}
