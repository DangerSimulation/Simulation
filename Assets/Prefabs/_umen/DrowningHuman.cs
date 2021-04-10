using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrowningHuman : MonoBehaviour
{
    //adjust this to change speed
    [SerializeField]
    float speed = 5f;
    //adjust this to change how high it goes
    [SerializeField]
    float height = 0.5f;

    Vector3 pos;

    private void Start()
    {
        EventManager.Instance.DrowningManInitiating += OnDrowningManInitiating;
        pos = transform.position;
    }



    public void OnDrowningManInitiating(object reference, EventArgs args)
    {
        
        Debug.Log("Event Initiated");

        // Set Dude to visible
        // Spawn Women and Text

    }

    void Update()
    {

        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
