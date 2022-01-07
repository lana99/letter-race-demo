using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    private float normalDrag;
    public float modifiedDrag;

    //Control how fast the car will travel
    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;

    //bc of drag , gravity
    public float airDrag;
    public float groundDrag;
    public float alignToGroundTime;
   
    //Refer to the spheres rigidbody
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    
    void Start()
    {
        //detaches the rigidbody/sphere from the car, the sphere isnt a child of the car after the game starts
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;

        normalDrag = sphereRB.drag;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        // moveInput *= fwdSpeed; //förkortning mI * fS, nytt värde för varje update, 

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;
        //Calculate Drag
        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;
    
        
        // set cars position to sphere
        transform.position = sphereRB.transform.position;

        //set cars rotation, och rotera endast med fart
        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");//time.deltatime -> makes it frame independent, multi med axis ist för bool
        transform.Rotate(0, newRotation, 0, Space.World);

        //raycast ground check
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);


        //rotate car to parallel to ground
       
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        // if (isCarGrounded)
        // {
        //     sphereRB.drag = groundDrag;
        // }
        // else
        // {
        //     sphereRB.drag = airDrag;
        // }
        
    }
    //Need fixedUpdate bc physics body
    
    
    private void FixedUpdate()
    {
        if(isCarGrounded)
        {
            //move car
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            //extra gravity
            sphereRB.AddForce(transform.up * -30f);
            carRB.MoveRotation(transform.rotation);
        }
        


    }
}
