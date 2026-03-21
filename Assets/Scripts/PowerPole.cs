using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PowerPole : MonoBehaviour
{
    // Static cache of all active PowerPoles to avoid expensive FindObjectsOfType calls
    private static List<PowerPole> activePoles = new List<PowerPole>();
    
    public static List<PowerPole> GetActivePoles()
    {
        return activePoles;
    }

    public GameObject glow;
    public float glowcounter = 0.1f;
    public ElectricalSource source;

    private AudioSource GlobalAudio;
    public AudioClip connect;
    public AudioClip disconnect;


    public bool powered = false;
    private bool touchedThisFrame = false;


    private bool wasPowered = false;
    
    void OnEnable()
    {
        // Register this pole when enabled
        if (!activePoles.Contains(this))
            activePoles.Add(this);
    }
    
    void OnDisable()
    {
        // Unregister this pole when disabled
        activePoles.Remove(this);
    }

    void Awake()
    {
        GlobalAudio = GetComponent<AudioSource>();

    }

    public void Startglow()
    {
        if (source.Powering)
        {
            glow.SetActive(true);
            powered = true;
            touchedThisFrame = true;
            glowcounter = 0.1f;
        }
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

        if (powered && !wasPowered)
        {
            GlobalAudio.PlayOneShot(connect, 0.7f);
        }
        else if (!powered && wasPowered)
        {
            GlobalAudio.PlayOneShot(disconnect, 0.7f);
        }

        wasPowered = powered;

        touchedThisFrame = false;
    }
}

