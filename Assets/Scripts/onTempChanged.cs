using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onTempChanged : MonoBehaviour
{
    public Material _9deg;
    public Material _19deg;
    public Material _29deg;
    public Material _39deg;

    void OnTemperatureChange(object instance , TemperatureChangingArgs arg)
    {
        Debug.Log(arg.temperature);
        switch(arg.temperature)
        {
            case "Chilly":
                this.GetComponent<MeshRenderer>().material = _9deg;
                break;
            case "Normal":
                this.GetComponent<MeshRenderer>().material = _19deg;
                break;
            case "Hot":
                this.GetComponent<MeshRenderer>().material = _29deg;
                break;
            case "Very Hot":
                this.GetComponent<MeshRenderer>().material = _39deg;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        EventManager.Instance.TemperatureChanging += OnTemperatureChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
