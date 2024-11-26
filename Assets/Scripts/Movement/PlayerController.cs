using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    

    [Header("References")]
    private Rigidbody rb;
    public Gun gc;
    public CameraManager cm;

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
    [SerializeField]
    private bool hasJumped;

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
    [SerializeField]
    private KeyCode reloadKey;
    [SerializeField]
    private KeyCode adsKey;
    [SerializeField]
    private KeyCode jumpKey;



    [Header ("Movement Stats")]
    public int walkSpeed;

    public float sprintSpeedMultiplier;
    
    public float maxSpeed;

    public float jumpheight;


    [Header("Ground Check")]
    public bool isGrounded;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundCheckRadius = 0.2f;

    [SerializeField]
    private LayerMask groundLayer;

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
         * JUMPING
         */
        if(Input.GetKeyDown(jumpKey) && isGrounded)
        {
            hasJumped = true;
        }


        /*
         * ATTACKING
         */

        if(Input.GetKey(shootKey) && !gc.IsReloading())
        {
            gc.Fire();
        }

        // Handle ADS input
        bool isAiming = Input.GetKey(adsKey);
        gc.SetADS(isAiming);
        cm.SetADS(isAiming);

        if (Input.GetKeyDown(reloadKey)) // Replace with your reloadKey variable if defined
        {
            gc.TryReload();
        }

    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
        currentSpeed = rb.velocity.magnitude;

        MovementHandler();
        ClampSpeed();
        JumpHandler();
        UpdateGroundedState();

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


    // Handles the jumping logic
    private void Jump()
    {
        rb.AddForce(Vector3.up * Mathf.Sqrt(jumpheight * 2f * Physics.gravity.magnitude), ForceMode.Impulse);
    }


    private void JumpHandler()
    {
        if (hasJumped && isGrounded)
        {
            Jump();
            hasJumped = false; // Reset hasJumped after initiating a jump
        }
    }
    private void UpdateGroundedState()
    {
        isGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
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
