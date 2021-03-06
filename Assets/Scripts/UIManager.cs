﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private bool makeTimeSpeedSliderOptional = true;
    
    [SerializeField]
    private Slider timeSpeedSlider = null;

    [SerializeField]
    private Text timeSpeedSliderLabel = null;

    [SerializeField]
    private Slider noiseScaleSlider = null;

    [SerializeField]
    private Text noiseScaleSliderLabel = null;

    [SerializeField]
    private Material deformationMaterial = null;

    [SerializeField]
    private bool useSamplesFromAudio = false;

    private SpectrumManager spectrumManager = null;

    [SerializeField]
    private float sampleMultipler = 100.0f;

    [SerializeField]
    private float delayBy = 1.0f;
    private float elapsedTime = 0;

    public PostProcessVolume volumen;
    public ColorGrading colors;

    void Start() 
    {
        spectrumManager = SpectrumManager.Instance;
        volumen.profile.TryGetSettings(out colors);
        timeSpeedSlider.onValueChanged.AddListener((value) => timeSpeedSliderChanged(value));
        noiseScaleSlider.onValueChanged.AddListener((value) => noiseScaleSliderChanged(value));
    }
    
    void Update()
    {
        if(deformationMaterial != null)
        {
            if(useSamplesFromAudio)
            {
                float value = spectrumManager.Samples[0] * sampleMultipler;
                float clampValue = Mathf.Clamp(value, 0.1f, 100.0f);

                if(elapsedTime >= delayBy)
                {
                    noiseScaleSliderChanged(clampValue);
                    elapsedTime = 0;
                }
                else {
                    elapsedTime += Time.deltaTime * 1.0f;
                    Debug.Log(elapsedTime);
                }
            }
        }
    }

    void timeSpeedSliderChanged(float newValue) 
    {
        if(makeTimeSpeedSliderOptional){
            return;
        }

        timeSpeedSlider.value = newValue;
        deformationMaterial.SetFloat("_TimeSpeed", timeSpeedSlider.value);
        timeSpeedSliderLabel.text = $"Time Speed: {timeSpeedSlider.value}";
    }

     void noiseScaleSliderChanged(float newValue) 
     {
        noiseScaleSlider.value = newValue;
        deformationMaterial.SetFloat("_NoiseScale", noiseScaleSlider.value);
        colors.hueShift =  new FloatParameter {value = noiseScaleSlider.value*10};
        colors.hueShift.overrideState = true;
        colors.enabled = new BoolParameter{value = true};
        noiseScaleSliderLabel.text = $"Noise Scale: {noiseScaleSlider.value}";
    }
}
