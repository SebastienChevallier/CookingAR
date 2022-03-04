using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrack : MonoBehaviour
{

    private ARTrackedImageManager _ARTrackedImageManager;

    public GameObject[] placeablePrefabs;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Get the manager
        _ARTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        // Instantiate and store the prefabs (limit : one prefab per image, no double)
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    public void OnEnable()
    {
        // Attach the event method
        _ARTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    public void OnDisable()
    {
        // Detach the event method
        _ARTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {

        //Debug.Log("OnImageChanged !");

        foreach (var trackedImage in args.added)
        {
            //Debug.Log("added image : " + trackedImage.referenceImage.name);
            UpdateImage(trackedImage);
        }
        foreach (var trackedImage in args.updated)
        {
            //Debug.Log("updated image : " + trackedImage.referenceImage.name);
            UpdateImage(trackedImage);
        }
        foreach (var trackedImage in args.removed)
        {
            //Debug.Log("removed image : " + trackedImage.referenceImage.name);
            spawnedPrefabs[trackedImage.referenceImage.name].SetActive(false);
        }

    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Transform imageTransform = trackedImage.transform;

        if (!spawnedPrefabs.ContainsKey(imageName))
        {
            Debug.LogWarning("No objects for this image : " + imageName);
            return;
        }

        Debug.Log("Image recognized : " + imageName);

        GameObject prefab = spawnedPrefabs[imageName];
        prefab.transform.SetPositionAndRotation(imageTransform.position, imageTransform.rotation);


        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //Debug.Log(trackedImage.referenceImage.name + " Currently tracking.....");
            prefab.SetActive(true);
        }
        else
        {
            //Debug.Log(trackedImage.referenceImage.name + " No more tracked.....");
            prefab.SetActive(false);
        }

        //foreach(GameObject go in spawnedPrefabs.Values)
        //{
        //    if (go.name != imageName)
        //    {
        //        Debug.Log("Disabling " + go.name + " because of " + imageName);
        //        go.SetActive(false);
        //    }
        //}


    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update Image tracking");
    }
}
