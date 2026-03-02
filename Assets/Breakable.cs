using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject ParticleSystem;
    public MeshRenderer renderer;
    public BoxCollider collider;

    public GameObject objToDeactivate;


    public bool SwitchMaterials = false;

    public Material pristine;
    public Material damaged;
    public Material broken;


    public void breakObject()
    {
        ParticleSystem.SetActive(true);
        renderer.enabled = false;
        collider.enabled = false;

        if (objToDeactivate != null)
        {
            objToDeactivate.SetActive(false);
        }

    }
}
