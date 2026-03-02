using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHandle : MonoBehaviour
{

    public Rigidbody Player;
    public float forceStrength = 10f;
    public float yForceMultiplier = 0.5f; 
    public bool isgrabbingRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

        Transform child1 = transform.Find("Hand_Rocket");
        Transform child2 = transform.Find("Hand_Red");
        Transform child3 = transform.Find("Hand_Blue");
        Transform child4 = transform.Find("Hand_Pressure");


        if (child1 != null || child2 != null || child3 != null || child4 != null)
        {

            if (child1 != null && Input.GetMouseButton(1))
            {

                Vector3 direction = (transform.position - Player.position).normalized;

                direction.y *= yForceMultiplier;

                Player.AddForce(direction * forceStrength, ForceMode.Force);

                isgrabbingRight = true;
            }

            if (child2 != null && Input.GetMouseButton(1))
            {
                Vector3 direction = (transform.position - Player.position).normalized;

                direction.y *= yForceMultiplier;

                Player.AddForce(direction * forceStrength, ForceMode.Force);

                isgrabbingRight = true;

            }

            if (child4 != null && Input.GetMouseButton(1))
            {
                Vector3 direction = (transform.position - Player.position).normalized;

                direction.y *= yForceMultiplier;

                Player.AddForce(direction * forceStrength, ForceMode.Force);

                isgrabbingRight = true;

            }

            if (child3 != null && Input.GetMouseButton(0))
            {
                if (!isgrabbingRight)
                {
                    Vector3 direction = (transform.position - Player.position).normalized;

                    direction.y *= yForceMultiplier;

                    Player.AddForce(direction * forceStrength, ForceMode.Force);
                }

            }

            if (child1 == null && child2 == null && child4 == null)
            {
                isgrabbingRight = false;

            }




        }

    }
}
