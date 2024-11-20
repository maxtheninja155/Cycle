using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    

    [Header("References")]
    private Rigidbody rb;

    public Gun gc;

    [Header("Debugging")]
    public State currentState;
    public float currentSpeed;

    [SerializeField]
    private Vector3 velocity;

    [SerializeField]
    private Vector3 movement;

    [SerializeField]
    private bool isMovingFoward;
    [SerializeField]
    private bool isMovingBackward;
    [SerializeField]
    private bool isMovingRight;
    [SerializeField]
    private bool isMovingLeft;
    [SerializeField]
    private bool isSprinting;
    [SerializeField]
    private bool isRolling;


    [Header ("Controls")]
    [SerializeField]
    private KeyCode forwardKey;
    [SerializeField]
    private KeyCode backwardKey;
    [SerializeField]
    private KeyCode rightKey;
    [SerializeField]
    private KeyCode leftKey;
    [SerializeField]
    private KeyCode sprintKey;
    [SerializeField]
    private KeyCode rollKey;
    [SerializeField]
    private KeyCode shootKey;

    [Header ("Movement Stats")]
    public int walkSpeed;

    public float sprintSpeedMultiplier;
    
    public float maxSpeed;


    public enum State
    {
        Idle,
        Walking,
        Sprinting,
        Rolling
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetState(State.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler();
    }

    public void InputHandler()
    {

        /*
         *  MOVEMENT
         */

        if (Input.GetKey(forwardKey))
        {
            isMovingFoward = true;
            SetState(State.Walking);
        }

        if (Input.GetKeyUp(forwardKey) && (CurrentStateChecker(State.Walking) || CurrentStateChecker(State.Sprinting)))
        {
            isMovingFoward = false;
            SetState(State.Idle);
        }

        if (Input.GetKey(backwardKey))
        {
            isMovingBackward = true;
            SetState(State.Walking);
        }

        if (Input.GetKeyUp(backwardKey) && (CurrentStateChecker(State.Walking) || CurrentStateChecker(State.Sprinting)))
        {
            isMovingBackward = false;
            SetState(State.Idle);
        }

        if (Input.GetKey(rightKey))
        {
            isMovingRight = true;
            SetState(State.Walking);
        }

        if (Input.GetKeyUp(rightKey) && (CurrentStateChecker(State.Walking) || CurrentStateChecker(State.Sprinting)))
        {
            isMovingRight = false;
            SetState(State.Idle);
        }

        if (Input.GetKey(leftKey))
        {
            isMovingLeft = true;
            SetState(State.Walking);
        }

        if (Input.GetKeyUp(leftKey) && (CurrentStateChecker(State.Walking) || CurrentStateChecker(State.Sprinting)))
        {
            isMovingLeft = false;
            SetState(State.Idle);
        }

        if(Input.GetKey(sprintKey) && Input.GetKey(forwardKey))
        {
            isSprinting = true;
            SetState(State.Sprinting);
        }

        if (Input.GetKeyUp(sprintKey))
        {
            isSprinting = false;
            SetState(State.Walking);
        } 

        /*
         * ATTACKING
         */

        if(Input.GetMouseButton(0))
        {
            gc.Fire();
        }

    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;

        currentSpeed = rb.velocity.magnitude;

        MovementHandler();
        ClampSpeed();
    }


    public void MovementHandler()
    {
        movement = Vector3.zero;
        int directionCount = 0;

        if (isMovingFoward)
        {
            movement += transform.forward;
            directionCount++;
        }

        if (isMovingBackward)
        {
            movement -= transform.forward;
            directionCount++;
        }

        if (isMovingRight)
        {
            movement += transform.right;
            directionCount++;
        }

        if (isMovingLeft)
        {
            movement -= transform.right;
            directionCount++;
        }

        // Adjust vector magnitude for diagonal movement
        if (directionCount > 1)
        {
            movement /= Mathf.Sqrt(2); // Divides by sqrt(2) (~1.414) for proper scaling
        }

        if (movement != Vector3.zero)
        {
            rb.AddForce(movement * walkSpeed);
        }

        if(isSprinting && CurrentStateChecker(State.Sprinting))
        {
            rb.AddForce(movement * walkSpeed * sprintSpeedMultiplier);
        }
    }



    private void ClampSpeed()
    {
        // Check if the current speed exceeds maxSpeed
        if (rb.velocity.magnitude > maxSpeed)
        {
            // Normalize the velocity and scale it to maxSpeed
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(isSprinting)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed * sprintSpeedMultiplier;
        }
    }


    public void SetState(State currentStateP)
    {
        currentState = currentStateP;
    }

    public bool CurrentStateChecker(State stateToCompareTo)
    {
        if(stateToCompareTo == currentState)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
