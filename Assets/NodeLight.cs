using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NodeLight : MonoBehaviour
{
    public float rotationVel;
    public float intensityChange;
    public float intensityDuration;
    [SerializeField] Light2D light2D;
    private float startFallOffStrenght;
    
    void Start()
    {
        light2D = GetComponent<Light2D>();
        startFallOffStrenght = light2D.falloffIntensity;
        light2D.falloffIntensity = 1;
        //randomize intensity
        intensityChange = Random.Range(0.8f, .4f);
        intensityDuration = Random.Range(.2f, 1f);
        rotationVel = Random.Range(-20, 20);
        light2D.intensity = .3f;

    }
    public void TurnLightOn(){
        //make a transition on the intensity using DOTween
        
        DG.Tweening.Sequence _sequence = DOTween.Sequence();
        _sequence.Insert(0, DOTween.To(() => light2D.intensity, x => light2D.intensity = x, 1, Random.Range(.5f,3f)))
            .Insert(0,DOTween.To(() => light2D.falloffIntensity, x => light2D.falloffIntensity = x, startFallOffStrenght, Random.Range(.5f,3f)) )
            .OnComplete(() => { StartCoroutine(UpdateLight()); });

    }
    
    public void Blink(){
        StopAllCoroutines();
        DG.Tweening.Sequence _sequence = DOTween.Sequence();
        _sequence.Insert(0, DOTween.To(x => light2D.intensity = x, 2f, 0f, Random.Range(1,4) ).OnComplete(()=>{light2D.enabled = false;}));
    }
    
    IEnumerator UpdateLight()
    {
        while (true)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationVel);
            //use a sin function to change the intensity of the light
            light2D.intensity =  Mathf.Lerp(light2D.intensity, 1 + Mathf.Sin(Time.time * intensityDuration) * intensityChange, .25f * Time.deltaTime);
            yield return null;
        }
    }

}