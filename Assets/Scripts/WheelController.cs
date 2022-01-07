using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public GameObject[] wheelsToRotate;
    public TrailRenderer[] trails;
    public float rotationSpeed;
    private Animator anime;
    // Start is called before the first frame update
    void Start()
    {
        //Catch
        anime = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    { 
        //For the wheels to rotate visually
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        foreach (var wheel in wheelsToRotate)
        {
            wheel.transform.Rotate(Time.deltaTime * verticalAxis * rotationSpeed,0,0, Space.Self);
        }

        if (horizontalAxis > 0)
        {
            //turning right
            anime.SetBool("goingLeft", false);
            anime.SetBool("goingRight", true);
        }else if (horizontalAxis < 0)
        {
            //turning left
            anime.SetBool("goingRight", false);
            anime.SetBool("goingLeft", true);
        }
        else
        {
            //must be going straight
            anime.SetBool("goingRight", false);
            anime.SetBool("goingLeft", false);
        }
        //if turning in any direction it wont skids
        if (horizontalAxis != 0)
        {
            foreach (var trail in trails)
            {
                trail.emitting = true;
            }
        }
        else
        {
            foreach (var trail in trails)
            {
                trail.emitting = false;
            } 
        }
    }
}
