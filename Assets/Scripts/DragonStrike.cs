using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DragonStrike : MonoBehaviour
{

    public bool canUltimate = true;

    public Transform arrow;
    private Rigidbody arrowRb;

    private Vector3 arrowLocalPos;
    private Vector3 arrowLocalRot;
    public float arrowForce = 3;
    public float arrowPullDistance = .2f;
    public float arrowPullDuration = 1;

    public float dragonSummonWait = 1;
    public GameObject dragonStrikePrefab;
    public GameObject portalPrefab;

    // Start is called before the first frame update
    void Start()
    {

        arrowRb = arrow.GetComponent<Rigidbody>();
        arrowLocalPos = arrow.localPosition;
        arrowLocalRot = arrow.localEulerAngles;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && canUltimate)
        {
            StartCoroutine(UltimateCourotine());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

    }

    IEnumerator UltimateCourotine()
    {
        canUltimate = false;

        //Pull Arrow
        arrow.DOLocalMoveZ(arrow.localPosition.z - arrowPullDistance, arrowPullDuration).SetEase(Ease.OutExpo);
        arrow.GetComponent<Renderer>().material.DOFloat(1, "_GlowPower", arrowPullDuration);
        //Field of View
        Camera.main.DOFieldOfView(65, arrowPullDuration).SetEase(Ease.OutSine);

        //--------WAIT----------
        yield return new WaitForSeconds(arrowPullDuration);
        //--------WAIT----------

        ThrowArrow();

        //Field of View
        Camera.main.DOFieldOfView(60, .5f).SetEase(Ease.OutBack);
        Quaternion q = Quaternion.LookRotation(transform.forward, Vector3.up);

        //Time for Dragon to summon
        yield return new WaitForSeconds(dragonSummonWait);

        GameObject dragon = Instantiate(dragonStrikePrefab, arrow.position, q);
        GameObject portal = Instantiate(portalPrefab, arrow.position - (arrow.forward * 5) + (Vector3.up*1.2f), q);

        //Show Portal
        portal.transform.DOScale(0, .2f).SetEase(Ease.OutSine).From();
        portal.GetComponent<Renderer>().material.DOFloat(1, "_Amount", 4f).SetDelay(8).OnComplete(() => Destroy(portal));

        //Particles
        ParticleSystem[] portalParticles = portal.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem p in portalParticles)
        {
            p.Play();
        }

        //Extras
        arrow.GetComponent<TrailRenderer>().emitting = false;
        arrow.GetComponent<Renderer>().enabled = false;
        arrow.parent = transform.GetChild(0);
        arrow.GetComponent<Renderer>().enabled = true;
        arrow.localPosition = arrowLocalPos;
        arrow.localEulerAngles = arrowLocalRot;
        arrowRb.isKinematic = true;
        arrow.GetComponent<Renderer>().material.SetFloat("_GlowPower", 1);
        arrow.GetComponent<Renderer>().material.DOFloat(0, "_GlowPower", .5f);

        //Shake
        Camera.main.transform.DOShakePosition(.2f, .5f, 20, 90, false, true);


        //Enabling Ultimate
        yield return new WaitForSeconds(8);
        canUltimate = true;
    }

    private void ThrowArrow()
    {
        arrow.parent = null;
        arrowRb.isKinematic = false;
        arrowRb.AddForce(-arrow.forward * arrowForce, ForceMode.Impulse);
        arrow.GetComponent<TrailRenderer>().enabled=true;

        arrow.GetComponent<Renderer>().material.SetFloat("_GlowPower", 0);

        ParticleSystem[] particles = arrow.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
    }

}
