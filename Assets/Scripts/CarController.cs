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

    public List<AxieInfo> axieInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    void wheelPosition(WheelCollider collider)
    {
        if(collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 positionWheel;
        Quaternion rotationWheel;
        collider.GetWorldPose(out positionWheel, out rotationWheel);

        visualWheel.transform.position = positionWheel;
        visualWheel.transform.rotation = rotationWheel;

    }


    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach(AxieInfo axieInfo in axieInfos)
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

            wheelPosition(axieInfo.rigthWheel);
            wheelPosition(axieInfo.leftWheel);

        }
    }
}
