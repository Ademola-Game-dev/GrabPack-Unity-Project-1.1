using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalReciever : MonoBehaviour
{
    public List<PowerPole> polesInPuzzle;
    public bool CircuitComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (AllPolesPowered())
        {
            Transform child1 = transform.Find("Hand_Rocket");
            Transform child2 = transform.Find("Hand_Red");
            Transform child3 = transform.Find("Hand_Blue");
            Transform child4 = transform.Find("Hand_Pressure");

            if (child1 != null || child2 != null || child3 != null || child4 != null)
            {

                CircuitComplete = true;

            }
        }
    }

    bool AllPolesPowered()
    {
        foreach (PowerPole pole in polesInPuzzle)
        {
            if (!pole.powered) return false;
        }
        return true;
    }
}
