using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.WSA;

public class PlayerScript : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private float turner;
    private float looker;
    public float sensitivity;
    private bool isMoving;

    // Use this for initialization
    void Start()
    {
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
            isMoving = false;
        else
            isMoving = true;

        if (!isMoving)
            return;

        CharacterController controller = GetComponent<CharacterController>();
        // is the controller on the ground?
        //Feed moveDirection with input.
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        //Multiply it by speed.
        moveDirection *= speed;
        //Jumping
        if (Input.GetKey(KeyCode.RightShift))
            moveDirection.y = jumpSpeed;
        else if (Input.GetKey(KeyCode.RightControl))
            moveDirection.y = -jumpSpeed;


        turner = Input.GetAxis("Mouse X") * sensitivity;
        looker = -Input.GetAxis("Mouse Y") * sensitivity;
        
        if (turner != 0)
        {
            //Code for action on mouse moving right
            transform.eulerAngles += new Vector3(0, turner, 0);
        }
        if (looker != 0)
        {
            //Code for action on mouse moving right
            transform.eulerAngles += new Vector3(looker, 0, 0);
        }
        //Applying gravity to the controller
        moveDirection.y -= gravity * Time.deltaTime;
        //Making the character move
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
            Screen.lockCursor = false;
        else
            Screen.lockCursor = true;

    }
}
