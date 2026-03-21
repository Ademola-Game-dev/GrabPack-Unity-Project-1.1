using UnityEngine;

[RequireComponent(typeof(PowerActivator))]
public class DetectGear : MonoBehaviour
{
    public GameObject gearToDetect;
    public GameObject gearVisual;

    private bool complete = false;
    private PowerActivator powerActivator;

    void Awake()
    {
        powerActivator = GetComponent<PowerActivator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (complete)
            return;

        if (other.gameObject == gearToDetect)
        {
            Destroy(gearToDetect);

            if (gearVisual != null)
                gearVisual.SetActive(true);

            complete = true;

            if (powerActivator != null)
                powerActivator.Activate();
        }
    }
}