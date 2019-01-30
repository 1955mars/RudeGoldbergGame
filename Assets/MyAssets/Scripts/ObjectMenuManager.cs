using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMenuManager : MonoBehaviour
{
    public List<GameObject> objectList; //handled automatically at start
    public List<GameObject> objectPrefabList; //set manually in inspector and MUST match order
                                              //of scene menu objects
    public int currentObject = 0;
    public List<int> maxObjectSpawnCount;
    public List<Text> objectNames;
    public List<string> onlyNames;

    // Use this for initialization
    void Start()
    {
        foreach (Transform child in transform)
        {
            objectList.Add(child.gameObject);
        }
        Debug.Log("Object list size " + objectList.Count);
        gameObject.SetActive(false);
        objectList[currentObject].SetActive(true);
        UpdateText();
    }

    public void MenuLeft()
    {
        objectList[currentObject].SetActive(false);
        currentObject--;
        if (currentObject < 0)
        {
            currentObject = objectList.Count - 1;
        }
        objectList[currentObject].SetActive(true);
        UpdateText();
    }
    public void MenuRight()
    {
        objectList[currentObject].SetActive(false);
        currentObject++;
        if (currentObject > objectList.Count - 1)
        {
            currentObject = 0;
        }
        objectList[currentObject].SetActive(true);
        UpdateText();
    }
    public void SpawnCurrentObject()
    {
        if(maxObjectSpawnCount[currentObject] <= 0)
        {
            return;
        }
        Instantiate(objectPrefabList[currentObject],
            objectList[currentObject].transform.position,
                objectList[currentObject].transform.rotation);
        maxObjectSpawnCount[currentObject]--;
        UpdateText();
    }

    // Update is called once per frame
    void UpdateText()
    {
        objectNames[currentObject].text = onlyNames[currentObject] + "[" + maxObjectSpawnCount[currentObject] + "]";
    }
}
