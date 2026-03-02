using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour
{
    private Animator valveanimator;

    public ParticleSystem GasMainParticles;
    public ParticleSystem GasValveParticles;

    public BoxCollider gasCollider;

    public RedSmokeEffects RedSmoke;

    void Start()
    {
        valveanimator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Transform child1 = transform.Find("Hand_Rocket");
        Transform child2 = transform.Find("Hand_Red");
        Transform child3 = transform.Find("Hand_Blue");
        Transform child4 = transform.Find("Hand_Pressure");

        bool shouldAnimate = false;

        if (child1 != null && Input.GetMouseButton(1))
        {
            shouldAnimate = true;
        }

        if (child2 != null && Input.GetMouseButton(1))
        {
            shouldAnimate = true;
        }

        if (child3 != null && Input.GetMouseButton(0))
        {
            shouldAnimate = true;
        }

        if (child4 != null && Input.GetMouseButton(1))
        {
            shouldAnimate = true;
        }

        valveanimator.speed = shouldAnimate ? 0.3f : 0;
    }

    public void TurnOffGas()
    {
        GasMainParticles.Stop();
        GasValveParticles.Stop();
        gasCollider.enabled = false;

        RedSmoke.RemoveEffects();
    }
}
