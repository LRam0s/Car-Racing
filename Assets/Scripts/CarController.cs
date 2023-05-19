using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [System.Serializable]
    public class AxieInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rigthWheel;
        public bool motor;
        public bool steering;
    }

    [SerializeField] List<AxieInfo> axieInfos;
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;
    private Rigidbody carRB;
    [SerializeField] Vector3 cOM;
    [SerializeField] float brakePower;
    [SerializeField] float brakeInput;
    

    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
        carRB.centerOfMass = cOM;
    }

    public void FixedUpdate()
    {
        
        CarConduction();
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
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        
        foreach (AxieInfo axieInfo in axieInfos)
        {
            if (axieInfo.steering)
            {
                axieInfo.leftWheel.steerAngle = steering;
                axieInfo.rigthWheel.steerAngle = steering;
            }
            if (axieInfo.motor)
            {
                axieInfo.leftWheel.motorTorque = motor;
                axieInfo.rigthWheel.motorTorque = motor;
            }

            WheelPosition(axieInfo.rigthWheel);
            WheelPosition(axieInfo.leftWheel);
            BrakesSystem(axieInfo.rigthWheel, axieInfo.leftWheel);
        }
    }

    private void BrakesSystem( WheelCollider RWheel, WheelCollider Lwheel)
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
}
