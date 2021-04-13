using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherInitiator : MonoBehaviour
{

    //public ScreenFader screenFader = null;

    public Material clear;
    public Material cloudy;
    public Material rainy;
    public Light light;

    private bool changeLight;
    private float val;

    // Start is called before the first frame update
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        EventManager.Instance.WeatherChanging += OnWeatherChanged;
    }

    public void OnWeatherChanged(object reference, WeatherChangingArgs args)
    {
        StartCoroutine(LoadWeather(args.type));
    }

    private IEnumerator LoadWeather(string weatherName)
    {

        //yield return screenFader.StartFadeIn();
        Debug.Log("Weather Changed!");
        var main = ps.main;
        switch (weatherName)
        {
            case "Rain":
                main.maxParticles = 5000;
                RenderSettings.skybox = rainy;
                light.intensity = 1f;
                break;
            case "BlueSky":
                main.maxParticles = 0;
                RenderSettings.skybox = clear;
                light.intensity = 2f;
                break;
            case "Cloudy":
                main.maxParticles = 0;
                RenderSettings.skybox = cloudy;
                light.intensity = 1.5f;
                break;
        }
        //yield return screenFader.StartFadeOut();
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
