using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool Locked = false;
    public Animator animator;

    public HandScanner ConnectedHandScanner;

    public bool hasopened = false;
    private bool JustUnlocked = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (!Locked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Camera cam = Camera.main;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 2f))
                {
                    if (hit.collider.gameObject.GetComponent<Door>())
                    {

                        Door door = hit.collider.gameObject.GetComponent<Door>();
                        door.Open();
                    }
                }

            }


            Transform child1 = transform.Find("Hand_Rocket");
            Transform child2 = transform.Find("Hand_Red");
            Transform child3 = transform.Find("Hand_Blue");
            Transform child4 = transform.Find("Hand_Pressure");

            if (child1 != null || child2 != null || child3 != null || child4 != null)
            {
                if (!JustUnlocked)
                {
                    if (hasopened == false)
                    {
                        Open();
                        hasopened = true;
                    }
                }

            }

            if (child1 == null && child2 == null && child3 == null && child4 == null)
            {
                hasopened = false;
                JustUnlocked = false;

            }
        }
        else if (Locked)
        {
            if (ConnectedHandScanner.SCANNED == true)
            {
                Locked = false;
                JustUnlocked = true;
            }
        }
    }

    public void Open()
    {
        if (Locked) return; 


        if (animator.GetBool("open") == true)
        {
            animator.SetBool("open", false);

        }
        else if (animator.GetBool("open") == false)
        {


            animator.SetBool("open", true);

        }

    }

}
