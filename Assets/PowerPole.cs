using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPole : MonoBehaviour
{
    public GameObject glow;
    public float glowcounter = 0.5f;
    public ElectricalSource source;

    public AudioSource GlobalAudio;

    public bool powered = false;
    private bool touchedThisFrame = false;
    public void Startglow()
    {
        if (source.powering)
        {
            glow.SetActive(true);
            powered = true;
            touchedThisFrame = true;
            glowcounter = 0.5f;
        }
    }

    public void StopGlow()
    {

        //glow.SetActive(false);

    }

    void Update()
    {
        if (!touchedThisFrame)
            glowcounter -= Time.deltaTime;

        if (glowcounter <= 0)
        {
            glow.SetActive(false);
            powered = false;
        }

        touchedThisFrame = false;
    }
}

