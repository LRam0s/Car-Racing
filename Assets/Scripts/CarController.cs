using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[SerializeField]
enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};
public class CarController : MonoBehaviour
{
    [System.Serializable]
    public class AxieInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rigthWheel;
        public bool motor;
        public bool steering;
        public bool brake;
    }

    [SerializeField] List<AxieInfo> axieInfos;
    [SerializeField] AxieInfo colliders;
    [SerializeField] float motorPower;
    [SerializeField] float maxSteeringAngle;
    private Rigidbody carRB;
    [SerializeField] Vector3 cOM;
    [SerializeField] float brakePower;
    [SerializeField] float brakeInput;
    [SerializeField] float speed;
    [SerializeField] float speedForSound;
    [SerializeField] float speedForSoundClamped;

    public float motorInput;


    [SerializeField] TextMeshProUGUI speedometerText;

    //RPM variables
    [SerializeField] float RPM;
    [SerializeField] float redLine;
    [SerializeField] float idleRPM;
    [SerializeField] TextMeshProUGUI rpmText;
    [SerializeField] TextMeshProUGUI gearText;
    // [SerializeField] Transform rpmNeedle;
    // [SerializeField] float minNeedleRotation; //Una vez que arme el velocimetro, ver en la rotacion donde empieza y donde termina, y ahi configurar estos valores
    // [SerializeField] float maxNeedleRotation;
    [SerializeField] int currentGear;

    [SerializeField] float[] gearRatios;
    [SerializeField] float diferentialRatio;
    [SerializeField] float currentTorque;
    [SerializeField] float clutch;
    [SerializeField] float wheelRPM;
    [SerializeField] AnimationCurve hpToRPMCurve;
    private GearState gearState;
    [SerializeField] float increaseGearRPM;
    [SerializeField] float decreaseGearRPM;
    [SerializeField] float changeGearTime = 0.5f;



    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = cOM;
        gearState = GearState.Running;
    }

    public void FixedUpdate()
    {
        speedForSound = colliders.rigthWheel.rpm * colliders.rigthWheel.radius * 2f * Mathf.PI / 10f;
        speedForSoundClamped = Mathf.Lerp(speedForSoundClamped, speedForSound, Time.deltaTime);
        CarConduction();
        CalculateSpeedAndRPM();
    }

    void WheelPosition(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        collider.GetWorldPose(out Vector3 positionWheel, out Quaternion rotationWheel);
        visualWheel.transform.SetPositionAndRotation(positionWheel, rotationWheel);

    }

    private void CarConduction()
    {
        motorInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");


        if(gearState != GearState.Changing)
        {
            if(gearState == GearState.Neutral)
            {
                clutch = 0;
                if (Mathf.Abs(motorInput) > 0)
                {
                    gearState = GearState.Running;
                }
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }
         else
        {
            clutch = 0;
        }
        
        float steering = maxSteeringAngle * steeringInput;
        
        foreach (AxieInfo axieInfo in axieInfos)
        {
            if (axieInfo.steering)
            {
                axieInfo.leftWheel.steerAngle = steering;
                axieInfo.rigthWheel.steerAngle = steering;
            }
            if (axieInfo.motor)
            {
                currentTorque = CalculateTorque(motorInput, axieInfo.rigthWheel, axieInfo.leftWheel);
                axieInfo.leftWheel.motorTorque = currentTorque * motorInput;
                axieInfo.rigthWheel.motorTorque = currentTorque * motorInput;

            }
            if (axieInfo.brake)
            {
                BrakesSystem(axieInfo.rigthWheel, axieInfo.leftWheel);
            
            }

            WheelPosition(axieInfo.rigthWheel);
            WheelPosition(axieInfo.leftWheel);
        }
    }

    private void BrakesSystem(WheelCollider RWheel, WheelCollider Lwheel)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            brakeInput = 1;
            

        } else
        {
            brakeInput = 0;
        }
        RWheel.brakeTorque = brakeInput * brakePower *  0.7f;
        Lwheel.brakeTorque = brakeInput * brakePower * 0.7f;
    }

    private float CalculateTorque(float motorInput, WheelCollider RWheel, WheelCollider Lwheel)
    {
        float torque = 0;
        AutomaticGear(motorInput);
        //ManualGear(motorInput);

        
        if (clutch < 0.1f)
        {
            RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * motorInput) + Random.Range(-50, 50), Time.deltaTime);
        }
        else
        {
            wheelRPM = Mathf.Abs((RWheel.rpm + Lwheel.rpm) / 2f) * gearRatios[currentGear] * diferentialRatio;
            RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM - 100, wheelRPM), Time.deltaTime * 3f);
            torque = (hpToRPMCurve.Evaluate(RPM / redLine)* motorPower/RPM) * gearRatios[currentGear] * diferentialRatio * 5252f * clutch;
        }

        return torque;
    }

    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;
        if(currentGear + gearState >= 0)
        {
            if(gearChange > 0)
            {
                yield return new WaitForSeconds(0.7f);
                if(RPM < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if(gearChange < 0)
            {
                yield return new WaitForSeconds(0.1f);
                if(RPM > decreaseGearRPM || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        }
        gearState = GearState.Running;

    }

    private void CalculateSpeedAndRPM()
    {
        speed = Mathf.Round(carRB.velocity.magnitude * 3.6f);
        speedometerText.SetText(speed + " km/h");
        //Acá va a ir el movimiento de la aguja del velocimetro de RPM que todavia no esta
        // rpmNeedle.rotation = Quaternion.Euler(0,0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, RPM / redLine);

        gearText.SetText((gearState == GearState.Neutral) ? "N" : (currentGear + 1).ToString());
        rpmText.SetText((RPM).ToString("0,000") + "rpm");

    }

    private void AutomaticGear(float motorInput)
    {
        if (RPM < idleRPM + 100 && motorInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (gearState == GearState.Running && clutch > 0)
        {
            if (RPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }
    }
    
    //Caja de cambios manual, cuando haga el UI en donde elija que tipo de transmision quiere la agrego y la optimizo
    private void ManualGear(float motorInput)
    {
        if (RPM < idleRPM + 100 && motorInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentGear < 6)
        {
            StartCoroutine(ChangeGear(1));
        }
        if (Input.GetKeyDown(KeyCode.Q) && currentGear >= 1)
        {
            StartCoroutine(ChangeGear(-1));

        }
    }

    public float GetSpeedRatio(float motorInput)
    {
        var gas = Mathf.Clamp(Mathf.Abs(motorInput), 0.5f, 1f);
        return RPM * gas / redLine;
    }

}
