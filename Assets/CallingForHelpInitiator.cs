using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallingForHelpInitiator : MonoBehaviour
{
    Animator an;
    Animator an1;

    public AudioSource helpAsked;
    public AudioSource there;
    public AudioSource wereYouAble;
    public AudioSource iHope;

    public GameObject secondHuman;
    public GameObject drowningMan;
    GameObject main;

    private bool rotate;
    private bool rotate1;
    // Start is called before the first frame update
    void Start()
    {
        an = this.GetComponent<Animator>();
        an1 = secondHuman.GetComponent<Animator>();
        main = GameObject.Find("OVRPlayerController");

        EventManager.Instance.DrowningManInitiating += OnDrowningManInitiating;
        EventManager.Instance.ShowDrowningManInitiating += OnShowDrowningManInitiating;

    }
    public void OnShowDrowningManInitiating(object reference, EventArgs args)
    {
        this.gameObject.SetActive(false);
        secondHuman.SetActive(true);


        secondHuman.transform.LookAt(drowningMan.transform);

        there.GetComponent<AudioSource>().Play(0);
        an1.Play("Point");

        //rotate1 = true;
    }

    public void OnDrowningManInitiating(object reference, EventArgs args)
    {
        drowningMan.SetActive(true);
        secondHuman.SetActive(false);

        Vector3 pos = main.transform.position - main.transform.forward * 3;

        float y = Terrain.activeTerrain.SampleHeight(pos);

        pos.y = y;


        secondHuman.transform.position = pos;
        secondHuman.transform.LookAt(main.transform);
        this.transform.position = pos;
        this.transform.LookAt(main.transform);
        Vector3 eulerRotation = this.transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);
        secondHuman.transform.rotation = Quaternion.Euler(0, eulerRotation.y, eulerRotation.z);


        helpAsked.GetComponent<AudioSource>().Play(0);
        an.Play("Gest");

        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        rotate = true;

    }

    void SmoothLookAt(GameObject source, Vector3 newDirection)
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            Vector3 lTargetDir = this.transform.position - main.transform.position;
            lTargetDir.y = 0.0f;
            main.transform.rotation = Quaternion.RotateTowards(main.transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 2f);

            Vector3 dir = (this.transform.position - main.transform.position).normalized;
            float dot = Vector3.Dot(dir, main.transform.forward);
            Debug.Log(dot);

            if (dot > 0.9)
            {
                rotate = false;
                UnityEngine.XR.InputTracking.disablePositionalTracking = false;

                an.Play("Gest", 0);
                helpAsked.GetComponent<AudioSource>().Play(0);
            }
        }

        if (rotate1)
        {
            Vector3 lTargetDir = drowningMan.transform.position - secondHuman.transform.position;
            lTargetDir.y = 0.0f;
            secondHuman.transform.rotation = Quaternion.RotateTowards(secondHuman.transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 2f);

            Vector3 dir = (drowningMan.transform.position - secondHuman.transform.position).normalized;
            float dot = Vector3.Dot(dir, secondHuman.transform.forward);
            Debug.Log("Dot: " + dot);

            if (dot > 0.9)
            {
                rotate1 = false;

                an1.Play("Point");
            }
        }

        if (OVRInput.Get(OVRInput.Button.Down, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pressed");



        }
        else if (OVRInput.Get(OVRInput.Button.Up, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pressed");
            an1.Play("Point", 0);
        }
            
    }
}
