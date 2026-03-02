using UnityEngine;
using System.Collections;

public class RigidboyPlayerController : MonoBehaviour
{
    public Transform headCheck; 
    public float standUpCheckHeight = 1f; 

    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float crouchMultiplier = 0.5f;

    public float sprintAcceleration = 2f;
    public float jumpForce = 10f;
    public float lookSpeedX = 2f;
    public float lookSpeedY = 2f;
    public Transform playerCamera;

    private Rigidbody rb;
    private float currentMoveSpeed;
    private float targetMoveSpeed;
    private float rotationX = 0f;

    public Animator playeranimations;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    public bool isGrounded;

    public bool IsCrouched = false;

    public Animator croucher;
    private Vector3 moveDirection;

    public GameObject RedHand;
    public GameObject PurpleHand;
    public GameObject FlareHand;

    public string handtoSwitch;

    public LaunchHand purplelauncher;
    public LaunchHand redLauncher;

    public float groundCheckRadius;
    public Transform groundCheck;

    public CapsuleCollider playerstandingCollider;

    private Vector3 currentMoveDirection;
    public float airInputSmooth = 6f;

    public CablePhysics redcable;
    public CablePhysics purplecable;
    public CablePhysics pressurecable;

    public AudioSource footstepSource;
    public AudioClip[] grassFootsteps;
    public AudioClip[] woodFootsteps;
    public AudioClip[] concreteFootsteps;
    public AudioClip[] MetalFootsteps;

    private bool isPlayingFootsteps = false;

    private bool CanStandUp()
    {
        return !Physics.SphereCast(headCheck.position, 0.3f, Vector3.up, out _, standUpCheckHeight, groundLayer);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentMoveSpeed = moveSpeed;
        targetMoveSpeed = moveSpeed;

        playeranimations.SetBool("walk", false);
        playeranimations.SetBool("switch", false);
        playeranimations.SetBool("jump", false);
        playeranimations.SetBool("crouch", false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (redcable.isActive == false && purplecable.isActive == false && pressurecable.isActive == false)
            {
                playeranimations.SetBool("switch", true);
                handtoSwitch = "red";
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (redcable.isActive == false && purplecable.isActive == false && pressurecable.isActive == false)
            {
                playeranimations.SetBool("switch", true);
                handtoSwitch = "purple";
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (redcable.isActive == false && purplecable.isActive == false && pressurecable.isActive == false)
            {
                playeranimations.SetBool("switch", true);
                handtoSwitch = "flare";
            }
        }

        isGrounded = Physics.SphereCast(groundCheck.position, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckRadius + 0.1f, groundLayer);

        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 rawDirection = transform.right * moveX + transform.forward * moveZ;

        if (isGrounded)
        {
            currentMoveDirection = rawDirection;
        }
        else
        {
            currentMoveDirection = Vector3.Lerp(
                currentMoveDirection,
                rawDirection,
                airInputSmooth * Time.deltaTime
            );
        }

        moveDirection = currentMoveDirection.normalized;
        if (moveX != 0 || moveZ != 0)
        {
            playeranimations.SetBool("walk", true);
            if (!isPlayingFootsteps)
            {
                StartCoroutine(PlayFootsteps());
            }
        }
        else
        {
            playeranimations.SetBool("walk", false);
            StopCoroutine(PlayFootsteps());
            isPlayingFootsteps = false;
        }

        if (isGrounded)
        {
            playeranimations.SetBool("jump", false);
        }
        else
        {
            playeranimations.SetBool("jump", true);
        }

        if (!IsCrouched)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                targetMoveSpeed = moveSpeed * sprintMultiplier;
                playeranimations.speed = 1.6f;
            }
            else
            {
                targetMoveSpeed = moveSpeed;
                playeranimations.speed = 1f;
            }
        }

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                targetMoveSpeed = moveSpeed * crouchMultiplier;
                playeranimations.speed = 1f;
                IsCrouched = true;
                croucher.SetBool("Crouched", true);
                playeranimations.SetBool("crouch", true);
                playerstandingCollider.enabled = false;
            }
            else if (IsCrouched)
            {
                if (CanStandUp())
                {
                    croucher.SetBool("Crouched", false);
                    playeranimations.SetBool("crouch", false);
                    IsCrouched = false;
                    playerstandingCollider.enabled = true;
                }
            }
        }

        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetMoveSpeed, sprintAcceleration * Time.deltaTime);
        rb.velocity = new Vector3(moveDirection.x * currentMoveSpeed, rb.velocity.y, moveDirection.z * currentMoveSpeed);

        if (!IsCrouched && Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private IEnumerator PlayFootsteps()
    {
        isPlayingFootsteps = true;

        while (moveDirection.magnitude > 0)
        {
            if (isGrounded)
            {
                float footstepInterval = IsCrouched ? 0.8f : (targetMoveSpeed > moveSpeed ? 0.3f : 0.5f);
                float volume = IsCrouched ? 0.5f : (targetMoveSpeed > moveSpeed ? 0.8f : 0.5f);

                AudioClip footstepClip = GetFootstepSound();
                if (footstepClip != null)
                {
                    footstepSource.PlayOneShot(footstepClip, volume);
                }

                yield return new WaitForSeconds(footstepInterval);
            }
            else
            {
                yield return null;
            }
        }

        isPlayingFootsteps = false;
    }

    private AudioClip GetFootstepSound()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            switch (hit.collider.tag)
            {
                case "Grass": return grassFootsteps[Random.Range(0, grassFootsteps.Length)];
                case "Wood": return woodFootsteps[Random.Range(0, woodFootsteps.Length)];
                case "Concrete": return concreteFootsteps[Random.Range(0, concreteFootsteps.Length)];
                case "Grab-able": return concreteFootsteps[Random.Range(0, concreteFootsteps.Length)];
                case "Metal": return MetalFootsteps[Random.Range(0, MetalFootsteps.Length)];

            }
        }
        return null;
    }

    public void SwitchHand()
    {
        playeranimations.speed = 1f;
        targetMoveSpeed = moveSpeed;

        if (handtoSwitch == "red")
        {
            redhand();
        }
        if (handtoSwitch == "purple")
        {
            purplehand();
        }
        if (handtoSwitch == "flare")
        {
            flarehand();
        }



    }

    public void redhand()
    {
        RedHand.SetActive(true);
        PurpleHand.SetActive(false);
        FlareHand.SetActive(false);
    }

    public void purplehand()
    {
        RedHand.SetActive(false);
        PurpleHand.SetActive(true);
        FlareHand.SetActive(false);
    }

    public void flarehand()
    {
        RedHand.SetActive(false);
        PurpleHand.SetActive(false);
        FlareHand.SetActive(true);
    }
}
