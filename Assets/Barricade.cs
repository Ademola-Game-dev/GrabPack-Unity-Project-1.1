using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    Rigidbody rb;
    public float pullFactor = 1f;
    public bool isgrabbingRight;

    public bool Pulled = false;
    public float rotationSpeed = 1f; 

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (pullFactor < 0)
        {
            rb.isKinematic = false;
            Pulled = true;
        }
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
                isgrabbingRight = true;
                pullFactor -= Time.deltaTime;
                if (!Pulled)
                {
                    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                }
            }

            if (child2 != null && Input.GetMouseButton(1))
            {
                isgrabbingRight = true;
                pullFactor -= Time.deltaTime;

                if (!Pulled)
                {
                    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                }
            }
            if (child3 != null && Input.GetMouseButton(0))
            {
                if (!isgrabbingRight)
                {
                    pullFactor -= Time.deltaTime;
                    if (!Pulled)
                    {
                        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                    }
                }



            }

            if (child4 != null && Input.GetMouseButton(1))
            {
                isgrabbingRight = true;
                pullFactor -= Time.deltaTime;

                if (!Pulled)
                {
                    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                }
            }


            if (child1 == null && child2 == null && child4 == null)
            {
                isgrabbingRight = false;

            }




        }

    }
}

