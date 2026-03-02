using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public float interactionRange = 2f;
    public LayerMask buttonLayer; 
    public LayerMask CodeCheckerLayer; 


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastInteraction();
        }
    }

    void RaycastInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, buttonLayer))
        {
            keyButton KeyButton = hit.transform.gameObject.GetComponent<keyButton>();
            KeyButton.pushed();
        }
        if (Physics.Raycast(ray, out hit, interactionRange, CodeCheckerLayer))
        {
            CheckCode checkcode = hit.transform.gameObject.GetComponent<CheckCode>();
            checkcode.Check();
        }
    }
}