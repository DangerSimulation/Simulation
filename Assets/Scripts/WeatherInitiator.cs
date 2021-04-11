using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherInitiator : MonoBehaviour
{
    
    public ScreenFader screenFader = null;

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

        yield return screenFader.StartFadeIn();
        Debug.Log("Weather Changed!");
        var main = ps.main;
        switch (weatherName)
        {
            case "Rain":
                main.maxParticles = 5000;
                break;
            case "BlueSky":
                main.maxParticles = 0;
                break;
            case "Cloudy":
                main.maxParticles = 0;
                break;
        }
        yield return screenFader.StartFadeOut();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
