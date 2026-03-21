using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class magnetHand : MonoBehaviour
{
    public SkinnedMeshRenderer renderer;

    public Material positive;
    public Material negative;

    public bool postiveforce = true;

    public ParticleSystem blastparticles;

    public AudioClip blastsfx;
    private AudioSource audioSource;
    void Start()
    {
        UpdateVisuals();
        audioSource = GetComponent<AudioSource>();

    }

    public void TogglePolarity()
    {
        postiveforce = !postiveforce;
        blastparticles.Play();
        audioSource.PlayOneShot(blastsfx, 1.0f);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (postiveforce)
        {
            renderer.material = positive;
        }
        if (!postiveforce)
        {
            renderer.material = negative;
        }
    }


}
