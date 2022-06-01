using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    public CharacterController controller; //sets what is being controlled by the controller method

    public float movementSpeed = 12f;      //a public number that can change your speed
    public float playerGravity = -9.81f;   //a public number that controls the gravity

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool   isGrounded;

    float backAndForth;  //movement backwards and forwards
    float leftAndRight;  //movement left and right
    public float playerJump = 3f;    //jump stuff

    Vector3 velocity; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //MOVEMENT CODE

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        backAndForth = Input.GetAxis("Vertical"); 
        leftAndRight = Input.GetAxis("Horizontal"); 

        Vector3 move = transform.right * leftAndRight + transform.forward * backAndForth;
       
        controller.Move(move * movementSpeed* Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerJump * -2f * playerGravity);
        } 

        velocity.y += playerGravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        

    
    }

}
