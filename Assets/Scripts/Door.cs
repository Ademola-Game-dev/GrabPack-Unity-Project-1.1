using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    public bool Locked = false;
    public Animator animator;

    public AudioClip openSFX;
    public AudioClip closeSFX;

    private AudioSource audioSource;

    private bool handTriggered; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Locked)
            return;

        HandleKeyboard();
        HandleHandInteraction();
    }

    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 2f))
            {
                if (hit.collider.GetComponent<Door>() == this)
                {
                    ToggleDoor();
                }
            }
        }
    }

    void HandleHandInteraction()
    {
        bool handAttached = false;

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Hand"))
            {
                handAttached = true;
                break;
            }
        }

        if (handAttached && !handTriggered)
        {
            ToggleDoor();
            handTriggered = true;
        }

        if (!handAttached)
        {
            handTriggered = false;
        }
    }

    public void ToggleDoor()
    {
        if (Locked)
            return;

        bool open = animator.GetBool("open");

        animator.SetBool("open", !open);

        if (open)
            audioSource.PlayOneShot(closeSFX);
        else
            audioSource.PlayOneShot(openSFX);
    }

    public void Unlock()
    {
        Locked = false;
    }

    public void Lock()
    {
        Locked = true;
    }
}