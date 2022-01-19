using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float moveInput;
    private float turnInput;
    private bool isFrontOfCarGrounded;
    private bool isBackOfCarGrounded;
    private bool floorCheck;
    private bool groundCheck;
    private bool obstacleCheck;

    //private float normalDrag;
    public float modifiedDrag;
    public float groundDrag;
    
    //Control how fast the car will travel
    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    
    //bc of drag , gravity
    public float airDrag;
    public float gravity;

    public float alignToGroundTime;
    public float alignToObstacleTime;
   
    //Refer to the spheres rigidbody
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    public GameObject frontOfCar;
    public GameObject backOfCar;

    void Start()
    {
        //detaches the rigidbody/sphere from the car, the sphere isnt a child of the car after the game starts
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        // moveInput *= fwdSpeed; //f�rkortning mI * fS, nytt v�rde f�r varje update, 

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;
        //Calculate Drag
        sphereRB.drag = Mathf.Abs(moveInput) > 0 ? modifiedDrag : groundDrag;
    
        
        // set cars position to sphere
        transform.position = carRB.transform.position;
         

        //set cars rotation, och rotera endast med fart
        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");//time.deltatime -> makes it frame independent, multi med axis ist f�r bool
        transform.Rotate(0, newRotation, 0, Space.World);

        //raycast ground check
        Vector3 frontPos = frontOfCar.transform.position;
        Vector3 backPos = backOfCar.transform.position;
        
        RaycastHit frontHit;
        RaycastHit backHit;
        isFrontOfCarGrounded = Physics.Raycast(frontPos, -transform.up, out frontHit, 1f, obstacleLayer);        //raycast ground check
        isBackOfCarGrounded = Physics.Raycast(backPos, -transform.up, out backHit, 1f, obstacleLayer);        //raycast ground check
        //isCarGrounded = Physics.Raycast(transform.position, -transform.up, out backHit, 1f, groundLayer);
        
        
        Debug.DrawRay( frontPos, -transform.up, Color.blue);
        Debug.DrawRay( backPos, -transform.up, Color.magenta);
        
        RaycastHit floorHit;
        floorCheck = Physics.Raycast(transform.position, -transform.up, out floorHit, 1f, groundLayer);
        RaycastHit groundHit;
        groundCheck = Physics.Raycast(transform.position, -transform.up, out groundHit, 1000f, groundLayer);
        RaycastHit obstacleHit;
        obstacleCheck = Physics.Raycast(transform.position, -transform.up, out obstacleHit, 1f, obstacleLayer);

        // Debug.Log("Is Front grounded? " + isFrontOfCarGrounded);
        // Debug.Log("Is Back grounded? " + isBackOfCarGrounded);

        //rotate car to parallel to ground

        
        if(isBackOfCarGrounded && isFrontOfCarGrounded == false)
        {
            //Debug.Log("Rotate forward ");
            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, backHit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToObstacleTime * Time.deltaTime);
        }
        else if (isFrontOfCarGrounded && isBackOfCarGrounded == false)
        {
            //Debug.Log("Rotate back " );
            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, frontHit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToObstacleTime * Time.deltaTime);
        }
        else
        {
            if (obstacleCheck)
            {
                Debug.Log("Rotate to obstacle ground " );
                Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, obstacleHit.normal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToObstacleTime * Time.deltaTime);
            }
            else
            {
                Debug.Log("Rotate to ground " );
                if (floorCheck)
                {
                    Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, groundHit.normal) * transform.rotation;
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToObstacleTime * Time.deltaTime);
                }
                else
                {
                    Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, groundHit.normal) * transform.rotation;
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
                }
                
            }

        }
        // TODO Make a raycast that checks for the ground and its appropriate rotate time and another for obstacles
        //
        // Debug.Log(transform);
        


        //  if (isCarGrounded)
        //  {
        //      sphereRB.drag = groundDrag;
        //  }
        //  else
        //  {
        //     sphereRB.drag = airDrag;
        // }
        
    }
    //Need fixedUpdate bc physics body
    
    
    private void FixedUpdate()
    {
        if(floorCheck || obstacleCheck)
        {
            //move car
            Debug.Log("Add Force");
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            //extra gravity
             sphereRB.AddForce(transform.up * gravity);
             carRB.MoveRotation(transform.rotation);
        }
        


    }
}
