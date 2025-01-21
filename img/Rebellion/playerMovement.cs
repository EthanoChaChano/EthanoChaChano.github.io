using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    #region
    //      Things needed to implement         //
    //-----------------------------------------//
    //Basic Movement: Walk                      DONE
    //Basic Movement: Run                       DONE
    //Jump                                      DONE
    //Camera Look                               DONE
    //Complex Movement: Crouch/Slide            DONE
    //Complex Movement: Wall Jump/Climb         STILL TO IMPLEMENT (Wall Jump)
    //Animation                                 DONE
    //Head Bob                                  STILL TO IMPLEMENT
    //FOV Change                                DONE
    //-----------------------------------------//
    #endregion

    #region Public Variables
    [Header("Movement Settings")]
    //public float walkSpeed = 5f;
    public float currentSpeed;
    public float minSpeed = 1f;
    public float maxSpeed = 10f;
    private float t;
    private float n;
    private float playerHeight;

    [Header("Jump Settings")]
    [Range(4, 20)]
    public int jumpHeight;
    public float gravity = 9.81f;
    private float g;

    [Header("Look Settings")]
    public float lookSensitivity = 2;
    public int lookClampDown = 90;
    public int lookClampUp = -60;
    public int thirdpersonClampDown = 30;

    [Header("Camera Settings")]
    public int fovWalk = 60;
    public int fovRun = 60;
    public float offset;
    #endregion

    #region Private Variables
    //GAME OBJECTS//
    private CharacterController controller;
    private Camera[] cam;
    private Animator anim;

    //CAMERA ROTATION//
    private float rotX1 = 0f;
    private float rotX2 = 0f;
    private float rotY = 0f;

    //MOVEMENT//
    private Vector2 movement;
    private Vector3 movementVector;
    private bool isCrouching;
    private bool isClimbing;
    private bool isFalling;
    #endregion

    void Start()
    {
        //Get Components
        controller = GetComponent<CharacterController>();
        cam = GetComponentsInChildren<Camera>();
        anim = GetComponentInChildren<Animator>();

        //Hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Set initial values
        cam[1].fieldOfView = fovWalk;
        g = gravity;
        playerHeight = controller.height;
    }

    void Update() {
        Vector3 playerMove;

        if (!controller.isGrounded) { movementVector.y -= gravity * Time.deltaTime; }   //Gravity

        //Changes character speed and FOV between lower and upper bound by value t//
        if (t <= 1 && (movementVector.x != 0 || movementVector.z != 0)){ t += 0.01f;      
        } else if (t >= 0 && movementVector.x == 0 && movementVector.z == 0) { t = 0f; cam[0].fieldOfView = fovWalk; currentSpeed = minSpeed; }
        cam[0].fieldOfView = Mathf.Lerp(fovWalk, fovRun, t);
        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);

        //Updates character's movement
        movementVector.x = movement.x * currentSpeed;
        movementVector.z = movement.y * currentSpeed;

        playerMove = transform.TransformDirection(movementVector);
        controller.Move(playerMove * Time.deltaTime);                               //Moves player to Vector3 position

        //Updates camera rotation and position
        cam[1].transform.LookAt(transform.position);                                //Forces third person camera to look at player
        cam[1].transform.parent.localRotation = Quaternion.Euler(rotX2, 0, 0);      //Update third person camera position based on parent
        cam[0].transform.localRotation = Quaternion.Euler(rotX1, 0, 0);             //Updates first person camera rotation
        cam[0].transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);

        transform.rotation = Quaternion.Euler(0, rotY, 0);                          //Updates player rotation for camera

        checkClimb();
        checkCrouched();

        animControls();

        //Extra (not implemented)
        //checkWall();
    }

    #region Player Input
    public void Look(InputAction.CallbackContext value)
    {
        //Gets input from mouse, put these inputs into the appropriate rotation value, clamps value so looking around is limited
        Vector2 look = value.ReadValue<Vector2>();

        rotY += look.x * lookSensitivity;
        rotX1 -= look.y * lookSensitivity;

        rotX1 = Mathf.Clamp(rotX1, lookClampUp, lookClampDown);
        rotX2 = Mathf.Clamp(rotX1, lookClampUp, thirdpersonClampDown);
    }

    public void Jump()
    {
        //Gets input from spacebar, check if player is grounded and not crouched, jump by 'jumpHeight' value
        if (controller.isGrounded && !isCrouching)
        {
            movementVector.y = jumpHeight;
        }
    } 

    public void Movement(InputAction.CallbackContext value)
    {
        //Get input from WASD or arrow keys
        //Lots of the movement has to continuosly refresh so it is done in Update() 
        movement = value.ReadValue<Vector2>();
    } 

    public void Crouch()
    {
        //Get input from shift, set bool to opposite (like a switch)
        isCrouching = !isCrouching;
    }

    public void CamChange() {
        //Get input from F, toggles between the 2 cameras
        cam[0].enabled = cam[1].enabled;
        cam[1].enabled = !cam[1].enabled;
    }
    #endregion

    #region Check Updates
    void checkClimb()
    {
        //Shoots  raycast forward, checking if player is looking at a climbable wall, if moving forward: climb OR if looking down: descend. else stay on wall
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + new Vector3(0, 0, 0), transform.forward, out hitInfo, transform.localScale.x) && hitInfo.transform.CompareTag("Climbable"))
        {
            if (movementVector.z > 0)
            {
                movementVector.y = jumpHeight * transform.localScale.y;
                isClimbing = true;
            }
            else if (movementVector.z == 0 && rotX1 > 75)
            {
                movementVector.y = -jumpHeight * transform.localScale.y * 0.5f;
                isClimbing = true;
            }
            else { movementVector.y = 0; gravity = 0f; }
        }
        else{ gravity = g; isClimbing = false; }
    } 

    void checkWall() 
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.right, out hitInfo, 1f) || Physics.Raycast(transform.position - new Vector3(0, 1, 0), -transform.right, out hitInfo, 1f) && !hitInfo.transform.CompareTag("Player"))
        {
            if (movementVector.y > 0)
            {
                movementVector.x += jumpHeight *20;
                movementVector.z += jumpHeight *20;
                
                controller.Move(transform.TransformDirection(movementVector) * Time.deltaTime);
            }
        }
    } //NOT FULLY IMPLEMENTED

    void checkCrouched() 
    {
        //Check if bool (toggled by OnCrouch), is true OR if player is already underneath an object and not climbing. If so, lower the player height and gradually reduce speed
        if ((isCrouching || (controller.height == controller.height/2)) && !isClimbing)
        {
            controller.height = playerHeight / 2;
            if (currentSpeed > minSpeed)      //SLIDING
            {
                t -= 0.02f;
            }
        }
        if (!Physics.Raycast(transform.position, Vector3.up, transform.localScale.y) && !isCrouching)
        {
            controller.height = playerHeight;

        }
    }

    void animControls()
    {
        //All animation in order: Movement, Crouch, Jump, Climb (lots of internal work in the Animator)
        if ((movementVector.x != 0 || movementVector.z != 0))
        {
            if (movementVector.z > 0) { anim.SetInteger("Direction", 1); }      //Forward
            else if (movementVector.x < 0) { anim.SetInteger("Direction", 2); } //Left
            else if (movementVector.z < 0) { anim.SetInteger("Direction", 3); } //Backward
            else if (movementVector.x > 0) { anim.SetInteger("Direction", 4); } //Right
        }else { anim.SetInteger("Direction", 0); }                              //Reset Movement

        if (controller.height == playerHeight / 2)
        {
            anim.SetBool("isCrouching", true);                                  //Crouch (if player is moving, Slide before Crouch)

            if (n >= 0 && n < 1 && currentSpeed != minSpeed) { n += .01f; } else { n = 1; }
            anim.transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Lerp(-1.95f, -0.835f, n), transform.position.z);
        }else
        {
            anim.SetBool("isCrouching", false);                            //Reset Crouch
            anim.transform.position = new Vector3(transform.position.x, transform.position.y + -1.55f, transform.position.z);
            n = 0;
        }

        if (movementVector.y > 0 && !isClimbing)
        {
            anim.SetBool("isJumping", true);
        }else { anim.SetBool("isJumping", false); }                             //Reset Jump

        //isClimbing should ideally detect what object it is climbing (likely using more Raycasts) e.g. wall, pipe, ladder
        if (isClimbing || gravity != g)
        {
            if (movementVector.z > 0 || rotX1 > 75){ anim.SetInteger("isClimbing", 2); }
            else { anim.SetInteger("isClimbing", 1); }
        }else { anim.SetInteger("isClimbing", 0); }                             //Reset Climb
    }



    #endregion
}
