using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{

    public Rigidbody Player;
    public float jumpForce = 10f; 
    public LaunchHand rockethand;
    public bool launched = false;

    public bool Powered = true;
    public ElectricalReciever powerSource;

    public Material poweredmatieral;
    public Material offMaterial;
    public MeshRenderer renderer;

    public GameObject light;

    public RigidboyPlayerController player;

    public AudioSource GlobalAudio;
    public AudioClip boostsfx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform child1 = transform.Find("Hand_Rocket");

        if (child1 != null)
        {
            if (!launched)
            {
                if (Powered)
                {
                    if (player.isGrounded == true)
                    {
                        Player.velocity = Vector3.zero; 
                        Player.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
                        launched = true;
                        rockethand.ForceImmediateReturn();
                        GlobalAudio.PlayOneShot(boostsfx, 1.0f);

                    }
                    if (player.isGrounded == false)
                    {
                        Player.velocity = Vector3.zero; 
                        Player.AddForce(transform.up * (jumpForce / 2f), ForceMode.Impulse); 
                        launched = true;
                        rockethand.ForceImmediateReturn(); 
                        GlobalAudio.PlayOneShot(boostsfx, 1.0f);
                    }

                }


            }




        }
        if (child1 == null)
        {

            launched = false;




        }

        if (!Powered)
        {
            if (powerSource.CircuitComplete == true)
            {
                Powered = true;
                renderer.material = poweredmatieral;
                light.SetActive(true);

            }
            if (powerSource.CircuitComplete == false)
            {
                light.SetActive(false);

                Powered = false;
                renderer.material = offMaterial;
            }
        }

    }
}
