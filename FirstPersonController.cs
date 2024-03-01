using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && isGrounded; 
    private bool ShouldChrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation;
    private bool ShouldHold => Input.GetKeyDown(pickUp);
    private bool ShouldThrow => Input.GetKeyDown(Throw);
    private bool ShouldRotate => Input.GetKey(rotate);

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool usefootsteps = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode pickUp = KeyCode.E;
    [SerializeField] private KeyCode rotate = KeyCode.R;
    [SerializeField] private KeyCode Throw = KeyCode.Mouse0;

    [Header("Movement Parameters")]
    [SerializeField] private float walkspeed = 2.0f;
    [SerializeField] private float sprintspeed = 4.0f;
    [SerializeField] private float crouchspeed = 1.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1,100)] private float lookspeedX = 2.0f;
    [SerializeField, Range(1,100)] private float lookspeedY = 2.0f;
    [SerializeField, Range(1,180)] private float upperlooklimit = 80.0f;
    [SerializeField, Range(1,180)] private float lowerlooklimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float gravity = -11f;
    [SerializeField] private float jumpheight = 1.6f;
    public bool isGrounded;
    public float groundDistance = 0.0f;
    public Transform groundCheck;
    public LayerMask groundMask;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]

    [SerializeField] private float walkBobSpeed = 4.0f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 8.0f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 2.0f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0f;
    private float timer;

    [Header("PickUp Parameters")]
    [SerializeField] private float throwForce = 2000f;
    [SerializeField] private float pickUpRange = 10f;
    [SerializeField] private float rotationSensitivity = 1f;
    private bool canDrop = true;
    public Transform holdPos;
    private GameObject heldObj;
    private Rigidbody heldObjRb;
    public GameObject player;
    private int LayerNumber;

    [Header("FootSteps Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodclips = default;
    [SerializeField] private AudioClip[] floorclips = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching? baseStepSpeed * crouchStepMultiplier : IsSprinting? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    [Header("Health Parameters")]

    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    public Image healthring;
    private float fillVelocity;


    ///////    ///////     ///////

    private Camera playerCamera;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0; //angle to clamp with uper ans lower limit



    void Awake() 
    {
        playerCamera = GetComponentInChildren<Camera>(); //can also initialize by draging component in the inspector
        characterController = GetComponent<CharacterController>();

        defaultYPos = playerCamera.transform.localPosition.y;

        LayerNumber = LayerMask.NameToLayer("holdLayer");

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;

        currentHealth = maxHealth;
    }

    
    void Update()
    {   
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        HealthRingFiller();
        if(CanMove)
        {
            HandleMovementInput();
            if(!ShouldRotate)HandleMouseLook();
            ApplyFinalMovement();
            if(canJump) HandleJump();
            if(canCrouch) HandleCrouch();
            if(canUseHeadbob) HandleHeadbob();
            if(usefootsteps) HandleFootSteps();
            
            if(ShouldHold)
            {
                if(heldObj == null) //if currently not holding anything
                {
                    //perform raycast to check if player is looking at object within pickuprange
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                    {
                        //make sure pickup tag is attached
                        if (hit.transform.gameObject.tag == "canPickUp")
                        {
                            //pass in object hit into the PickUpObject function
                            HandlePickUpObject(hit.transform.gameObject);
                        }
                    }
                }
                else
                {
                    if(canDrop == true)
                    {
                       HandleThrow();
                    }
                }
            }

            if(heldObj != null){HandleMove(); HandleRotate();}
            if(heldObj != null && ShouldThrow && canDrop) HandleThrow();

        }
        
    }



    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching? crouchspeed : IsSprinting? sprintspeed : walkspeed )* Input.GetAxis("Vertical") , (isCrouching? crouchspeed : IsSprinting? sprintspeed : walkspeed ) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY; 

    }

    

    private void HandleMouseLook()
    {
        if(ShouldRotate)
        {
           
            return;
        }
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        rotationX -= Input.GetAxis("Mouse Y") * lookspeedY * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -upperlooklimit, lowerlooklimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler( 0, Input.GetAxis("Mouse X") * lookspeedX * Time.deltaTime, 0 );
        



    }

    private void  ApplyFinalMovement()
    {
        moveDirection.y += gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);

    }

    private void HandleJump()
    {
        if(ShouldJump) {moveDirection.y = Mathf.Sqrt(jumpheight * -1f * gravity);}
    }

    private void HandleCrouch()
    {
        if(ShouldChrouch) StartCoroutine(CrouchStand());
    }

    private void HandleHeadbob()
    { 
        if (!isGrounded) return;
        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f )
        {
            timer += Time.deltaTime * (isCrouching? crouchBobSpeed : IsSprinting? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3( playerCamera.transform.localPosition.x, 
            defaultYPos + Mathf.Sin(timer) * (isCrouching? crouchBobAmount: IsSprinting? sprintBobAmount: walkBobAmount),
            playerCamera.transform.localPosition.z);
        }
    }

    private void HandleFootSteps()
    {
        if(!isGrounded) return;
        if(currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;
        if(footstepTimer <= 0)
        { 
            //if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            //{
               //switch(hit.collider.tag)
                //{
               // case"FOOTSTEPS/Wood":
                    //footstepAudioSource.PlayOneShot(woodclips[Random.Range(0, woodclips.Length-1)]);
                   // break;
                //case"FOOTSTEPS/Floor":
                    footstepAudioSource.PlayOneShot(floorclips[Random.Range(0, floorclips.Length-1)]);
                   // break;
                //default:
                  //  break;
               // }
            //}

            footstepTimer = GetCurrentOffset;
        }

        
    }


    private void HandlePickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //make sure the object has a RigidBody
        {
            heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
            heldObj.layer = LayerNumber; //change the object layer to the holdLayer
            //make sure object doesnt collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
        
    }

    private void HandleMove()
    { 
        //keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.transform.position;
    }

    private void HandleRotate()
    {   
        
        if(ShouldRotate)
        {  
        
        canDrop = false;
        rotationX =0f;
        playerCamera.transform.localRotation = Quaternion.Euler(0f,0f,0f);
        float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
        float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
        //rotate the object depending on mouse X-Y Axis
        heldObj.transform.Rotate(Vector3.up, XaxisRotation, Space.World);
        heldObj.transform.Rotate(Vector3.right, YaxisRotation, Space.World);

        }

        else 
        {   
            
            canDrop = true;
            HandleMouseLook();
        }    

    }
        


    private void HandleThrow()
    {
        ///StopClipping///
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
        ///ThrowObject///
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        if(ShouldThrow)heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;

    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {
            //Die
           Scene currentScene = SceneManager.GetActiveScene();
           SceneManager.LoadScene(currentScene.buildIndex);
        }   
    }

    public void IncreaseHealth(int healthvalue)
    {
        
        currentHealth += healthvalue;
        
        
    }

    void HealthRingFiller()
    {   
        float targetFill = (float) currentHealth / maxHealth;
        float smoothTime = 0.3f;
        float alpha =0.6f;

        healthring.fillAmount = Mathf.SmoothDamp(healthring.fillAmount, targetFill, ref fillVelocity, smoothTime);
        Color healthColor = Color.Lerp(Color.red,Color.green,targetFill);
        healthColor.a = alpha;
        healthring.color = healthColor;
        
        
    }
    


    private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))  yield break;
       
        duringCrouchAnimation = true;
        float timeElapsed = 0;
        float targetheight = isCrouching? standingHeight : crouchingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;
        Vector3 savegc = groundCheck.transform.position;

        

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight,targetheight,timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter,targetCenter,timeElapsed/timeToCrouch);
            groundCheck.transform.position = Vector3.Lerp(currentCenter,targetCenter,timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetheight;
        characterController.center = targetCenter;
        groundCheck.transform.position = savegc;

        isCrouching = !isCrouching;
        duringCrouchAnimation = false;


    }
}
