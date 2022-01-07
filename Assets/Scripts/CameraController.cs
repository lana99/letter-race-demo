using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject car;
    public GameObject cameraFolder;
    public Transform[] camLocations;
    private int locationIndicator = 0;

    [Range(0, 1)] public float smoothTime = .5f;
    
   
    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Player");
        cameraFolder = car.transform.Find("Main Camera").gameObject;
        camLocations = cameraFolder.GetComponentsInChildren<Transform>();
    }

    void FixedUpdate()
    {
        transform.position = camLocations[locationIndicator].position * (1 - smoothTime) + transform.position * smoothTime;
        transform.LookAt(camLocations[0].transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
