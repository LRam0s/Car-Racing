using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [SerializeField] AudioSource runningSound;
    [SerializeField] float runningMaxVolume;
    [SerializeField] float runningMaxPitch;
    [SerializeField] AudioSource idleSound;
    [SerializeField] float idleMaxVolume;
    [SerializeField] AudioSource brakeSound;



    private CarController carController;
    private float speedRatio;
    private float motorInput;
    void Start()
    {
        carController = GetComponent<CarController>();
        motorInput = carController.motorInput;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isBraking);
        if (carController)
        {
            speedRatio = carController.GetSpeedRatio(motorInput);
        }
        idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
        runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
        runningSound.pitch = Mathf.Lerp(0.3f, runningMaxPitch, speedRatio);
    }
}
