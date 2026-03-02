using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LaunchHand : MonoBehaviour
{
    public string Hand = "Left";
    public Transform handTransform;
    public Transform handOrigin;
    public float maxRange = 10f;
    public float speed = 20f;

    private Transform originalParent;
    private bool isReturning = false;
    public bool returned = true;

    public Animator playeranimations;

    public string GrabableLayer;

    public bool CanReturn = true;
    public bool canDrag = false;
    public float pullSpeed = 5f;

    public CablePhysics CableSim;


    //public LineRenderer cableRenderer;

    public GameObject hitOBJ;

    public Vector3 selftransform;

    public bool holdingbattery = false;
    public GameObject battery;
    public Transform batterypos;

    public Animator handgrabbing;
    public Barricade br;

    public AudioSource globalAudio;
    public AudioClip firesfx;
    public AudioClip grabsfx;
    public AudioClip dragsfx;
    public GameObject dragsounds;
    public GameObject pressurebuild;
    public AudioClip pressureRelease;

    private bool lockReturn = false;

    public RotateArms aimOverride;


    public bool isPressureHand = false;
    public float pressure = 0;

    public GameObject SMOKE;

    public GameObject gauge;
    public Image guageUI;
    public ParticleSystem impact;
    void Start()
    {
        //cableRenderer.enabled = false;

        originalParent = handTransform.parent;

        selftransform = gameObject.transform.localScale;
    }

    void Update()
    {


        if (CanReturn == false && !lockReturn)
        {
            if (Hand == "Left" && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ReturnHand());
                CanReturn = true;
                canDrag = false;

            }
            else if (Hand == "Right" && Input.GetMouseButtonDown(1))
            {
                StartCoroutine(ReturnHand());
                CanReturn = true;
                canDrag = false;

            }
        }

        if (isReturning) return;
        if (!returned) return;

        if (Hand == "Left" && Input.GetMouseButtonDown(0))
        {
            FireHand();
        }
        else if (Hand == "Right" && Input.GetMouseButtonDown(1))
        {
            if (!playeranimations.GetBool("switch"))
            {
                FireHand();

            }
        }


    }
    public void EnableDrag()
    {
        canDrag = true;
    }

    public void FireHand()
    {
        globalAudio.PlayOneShot(firesfx, 0.7f);

        CableSim.isActive = true;


        if (Hand == "Right")
        {
            aimOverride.rightActive = true;
        }
        if (Hand == "Left")
        {
            aimOverride.leftActive = true;
        }

        if (holdingbattery)
        {
            holdingbattery = false;
            battery.transform.parent = null;
            Rigidbody batteryRB = battery.GetComponent<Rigidbody>();
            BoxCollider col = battery.GetComponent<BoxCollider>();

            batteryRB.isKinematic = false;

            if (Hand == "Left")
            {
                batteryRB.AddForce(-transform.up * 18f, ForceMode.Impulse);
                aimOverride.leftActive = false;

            }
            if (Hand == "Right")
            {
                batteryRB.AddForce(transform.up * 18f, ForceMode.Impulse);
                aimOverride.rightActive = false;

            }

            CableSim.isActive = false;

            col.enabled = true;
            battery = null;
            handgrabbing.SetBool("grabbing", false);

            return;



        }
        CanReturn = true;
        //cableRenderer.enabled = true;


        if (isReturning) return;
        playeranimations.SetTrigger("shoot");
        handTransform.parent = null;
        returned = false;
        Vector3 targetPoint;
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange))
        {
            targetPoint = hit.point;

            if (Hand == "Right")
            {
                aimOverride.rightHitPoint = targetPoint;
            }
            if (Hand == "Left")
            {
                aimOverride.leftHitPoint = targetPoint;
            }

            if (hit.collider.gameObject.tag == (GrabableLayer))
            {
                CanReturn = false;
                //handTransform.parent = hit.collider.gameObject.transform;



                hitOBJ = hit.collider.gameObject;
                if (LayerMask.LayerToName(hitOBJ.layer) == "Battery")
                {
                    holdingbattery = true;
                    handgrabbing.SetBool("grabbing", true);
                }
                if (hitOBJ.GetComponent<HandScanner>() == true)
                {
                    handgrabbing.SetBool("grabbing", false);
                }
                if (hitOBJ.GetComponent<Rigidbody>() != null)
                {
                    if (hitOBJ.GetComponent<Barricade>() != null)
                    {
                        br = hitOBJ.GetComponent<Barricade>();

                    }
                    if (!isPressureHand)
                    {
                        handgrabbing.SetBool("grabbing", true);

                    }

                    Invoke("EnableDrag", 0.5f);

                }
                if (LayerMask.LayerToName(hitOBJ.layer) == "Grabanimation" || LayerMask.LayerToName(hitOBJ.layer) == "Minecart" || LayerMask.LayerToName(hitOBJ.layer) == "KeyCard")
                {
                    handgrabbing.SetBool("grabbing", true);
                }


                if (Hand == "Right")
                {
                    Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, -hit.normal);
                    handTransform.rotation = Quaternion.LookRotation(projectedForward, -hit.normal);
                }
                if (Hand == "Left")
                {
                    Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                    handTransform.rotation = Quaternion.LookRotation(projectedForward, hit.normal);
                }

            }
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxRange;
        }
        Vector3 impactPoint = hit.point;

        StartCoroutine(MoveHand(targetPoint, impactPoint));
    }

    private IEnumerator MoveHand(Vector3 target, Vector3 impactPoint)
    {
        Vector3 start = handTransform.position;
        float distance = Vector3.Distance(start, target);
        float duration = distance / speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            handTransform.position = Vector3.Lerp(start, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        handTransform.position = target;
        if (hitOBJ != null)
        {
            handTransform.position = impactPoint;
            handTransform.SetParent(hitOBJ.transform, true);
            if (LayerMask.LayerToName(hitOBJ.layer) == "Battery")
            {

                battery = hitOBJ;

                //handTransform.parent = hitOBJ.transform;
                //hitOBJ.transform.parent = handTransform;




            }


            lockReturn = true;
            StartCoroutine(UnlockReturn());
        }

        yield return new WaitForSeconds(0.1f);

        if (CanReturn)
        {
            StartCoroutine(ReturnHand());
        }
    }

    private IEnumerator UnlockReturn()
    {
        yield return new WaitForSeconds(0.1f);
        lockReturn = false;
    }

    public void return1()
    {
        if (lockReturn) return;

        CanReturn = true;
        StartCoroutine(ReturnHand());
    }

    private IEnumerator ReturnHand()
    {
        handTransform.parent = null;

        if (battery != null)
        {
            battery.transform.position = batterypos.position;
            battery.transform.rotation = batterypos.rotation;

            Rigidbody rb = battery.GetComponent<Rigidbody>();
            if (rb != null)
            {
                battery.transform.parent = null;

                rb.isKinematic = true;

                BoxCollider col1 = battery.GetComponent<BoxCollider>();
                col1.enabled = false;
                battery.transform.parent = handTransform;

            }
        }

        handgrabbing.SetBool("grabbing", true);

        //dragsounds.SetActive(false);
        pressure = 0;
        globalAudio.PlayOneShot(grabsfx, 0.7f);

        br = null;
        Transform batterychild = transform.Find("Battery");
        if (batterychild == null)
        {
            batterychild = transform.Find("Gear");

        }
        if (batterychild == null)
        {
            holdingbattery = false;
            battery = null;
        }


        isReturning = true;
        canDrag = false;
        hitOBJ = null;
        Vector3 startPosition = handTransform.position;
        Quaternion startRotation = handTransform.rotation;

        float duration = Vector3.Distance(startPosition, handOrigin.position) / speed;
        float elapsedTime = 0f;

        while (CableSim.GetCablePoints().Count > 1)
        {
            List<Vector3> points = CableSim.GetCablePoints();

            if (points.Count <= 2)
                break;

            Vector3 targetPoint = points[points.Count - 2];

            while (Vector3.Distance(handTransform.position, targetPoint) > 0.05f)
            {
                handTransform.position = Vector3.MoveTowards(
                    handTransform.position,
                    targetPoint,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            CableSim.RemoveLastWrapPoint();
        }

        while (Vector3.Distance(handTransform.position, handOrigin.position) > 0.1f)
        {
            handTransform.position = Vector3.MoveTowards(
                handTransform.position,
                handOrigin.position,
                speed * Time.deltaTime
            );

            yield return null;
        }

        if (batterychild == null)
        {
            handgrabbing.SetBool("grabbing", false);

        }

        handTransform.position = handOrigin.position;
        handTransform.rotation = handOrigin.rotation;
        handTransform.parent = originalParent;
        isReturning = false;
        returned = true;
        // playeranimations.SetTrigger("return");
        canDrag = false;
        // cableRenderer.enabled = false;
        CableSim.InitializeCable();
        CableSim.isActive = false;
        gameObject.transform.localScale = selftransform;


        if (Hand == "Right")
        {
            aimOverride.rightActive = false;
        }
        if (Hand == "Left")
        {
            aimOverride.leftActive = false;
        }
    }
    void LateUpdate()
    {
        if (!CanReturn && canDrag)
        {
            bool isLeftHandActive = Hand == "Left" && Input.GetMouseButton(0);
            bool isRightHandActive = Hand == "Right" && Input.GetMouseButton(1);

            bool leftReleased = Hand == "Left" && Input.GetMouseButtonUp(0);
            bool rightReleased = Hand == "Right" && Input.GetMouseButtonUp(1);

            if (isLeftHandActive || isRightHandActive)
            {
                GameObject hitOBJ = handTransform.parent?.gameObject;
                if (hitOBJ != null && hitOBJ.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = hitOBJ.GetComponent<Rigidbody>();
                    string objectLayer = LayerMask.LayerToName(hitOBJ.layer);
                    Vector3 targetPos = handOrigin.position;

                    if (!isPressureHand)
                    {
                        dragsounds.SetActive(true);
                    }

                    if (objectLayer == "Minecart")
                    {
                        Vector3 moveDirection = (targetPos - hitOBJ.transform.position).normalized;

                        float forwardSpeed = Vector3.Dot(moveDirection, hitOBJ.transform.forward);

                        rb.AddForce(hitOBJ.transform.forward * forwardSpeed * pullSpeed, ForceMode.Acceleration);
                    }


                    else
                    {
                        if (!isPressureHand)
                        {
                            if (hitOBJ.GetComponent<Breakable>() == null)
                            {
                                if (br != null)
                                {
                                    if (br.Pulled == true)
                                    {
                                        Vector3 direction = targetPos - rb.position;

                                        float springStrength = pullSpeed * 25f; 
                                        float damping = 8f;                      

                                        Vector3 force =
                                            direction * springStrength
                                            - rb.velocity * damping;

                                        rb.AddForce(force, ForceMode.Force);
                                    }
                                }

                                if (br == null)
                                {
                                    Vector3 direction = targetPos - rb.position;

                                    float springStrength = pullSpeed * 25f;  // tune this
                                    float damping = 8f;                      // tune this

                                    Vector3 force =
                                        direction * springStrength
                                        - rb.velocity * damping;

                                    rb.AddForce(force, ForceMode.Force);
                                }
                            }
                        }
                        else
                        {
                            SMOKE.SetActive(true);
                            gauge.SetActive(true);
                            pressurebuild.SetActive(true);

                            if (pressure < 10f)
                            {
                                pressure += Time.deltaTime * 4f;
                                pressure = Mathf.Min(pressure, 10f);
                                guageUI.fillAmount = pressure / 10f;

                            }



                            if (hitOBJ.GetComponent<Breakable>() == true)
                            {
                                Breakable breakable = hitOBJ.GetComponent<Breakable>();

                                if (breakable.SwitchMaterials == true)
                                {
                                    if (pressure < 5f)
                                    {
                                        breakable.renderer.material = breakable.pristine;
                                    }
                                    else if (pressure >= 5f && pressure <= 9f)
                                    {
                                        breakable.renderer.material = breakable.damaged;
                                    }
                                    else if (pressure > 9f)
                                    {
                                        breakable.renderer.material = breakable.broken;
                                    }
                                }


                            }

                        }

                    }
                }
            }
            if (isLeftHandActive == false && isRightHandActive == false)
            {
                dragsounds.SetActive(false);

            }
            if ((leftReleased || rightReleased) && isPressureHand)
            {
                GameObject grabbedObj = handTransform.parent?.gameObject;


                if (grabbedObj != null)
                {
                    Rigidbody rb = grabbedObj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {


                        if (grabbedObj.GetComponent<Breakable>() == false)
                        {
                            Vector3 launchDir = (grabbedObj.transform.position - handOrigin.position).normalized;

                            float launchForce = pressure * 100f;
                            handgrabbing.SetBool("grabbing", true);

                            rb.isKinematic = false;
                            rb.AddForce(launchDir * launchForce, ForceMode.Impulse);
                            impact.Play();
                        }

                        if (grabbedObj.GetComponent<Breakable>() == true)
                        {

                            handgrabbing.SetBool("grabbing", false);

                            if (pressure > 4)
                            {
                                Breakable breakable = grabbedObj.GetComponent<Breakable>();

                                //Vector3 launchDir = (grabbedObj.transform.position - handOrigin.position).normalized;

                                breakable.breakObject();
                                //float launchForce = pressure * 100f;

                                //rb.isKinematic = false;
                                //rb.AddForce(launchDir * launchForce, ForceMode.Impulse);

                                impact.Play();

                            }

                        }


                    }
                }
                SMOKE.SetActive(false);
                gauge.SetActive(false);
                pressure = 0f;
                pressurebuild.SetActive(false);
                globalAudio.PlayOneShot(pressureRelease, 2.0f);

                return1();
            }
        }
    }

    public void ForceImmediateReturn()
    {
        pressure = 0;
        StopAllCoroutines();
        CableSim.isActive = false;
        isReturning = false;
        returned = true;
        canDrag = false;
        hitOBJ = null;
        handgrabbing.SetBool("grabbing", false);
        CanReturn = true;
        handTransform.parent = originalParent;
        handTransform.position = handOrigin.position;
        handTransform.rotation = handOrigin.rotation;

        //cableRenderer.enabled = false;
        CableSim.InitializeCable();

        if (Hand == "Right")
        {
            aimOverride.rightActive = false;
        }
        if (Hand == "Left")
        {
            aimOverride.leftActive = false;
        }
    }


}


