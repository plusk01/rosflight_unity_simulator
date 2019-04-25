using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multirotor : MonoBehaviour {
    [HeaderAttribute("Physical Parameters")]
    public double mass = 1.4; // [kg]

    [HeaderAttribute("Actuators")]
    public Propeller[] propellers;

    [HeaderAttribute("Sensors")]
    public IMU imu;
    public TruthSensor truth;

    [HeaderAttribute("Communications")]
    public SimCom comms;

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        comms.OnMotorCmd += new SimCom.MotorCmdHandler(OnMotorCmd);
    }

    // ------------------------------------------------------------------------
    
    // Update is called once per frame
    void Update()
    {
        
    }

    // ------------------------------------------------------------------------

    void FixedUpdate()
    {
        //
        // Ground Truth
        //

        Vector3 x, v;
        Quaternion q;
        long timestampTicks = truth.GetMeasurement(out x, out v, out q);
        comms.SendTruth(timestampTicks, x, v, q);

        //
        // IMU
        //

        Vector3 accel, gyro;
        timestampTicks = imu.GetMeasurement(out accel, out gyro);

        // Not only does this send IMU data, but it also acts as an indication
        // that a new physics update has occurred.
        comms.SendIMU(timestampTicks, accel, gyro);
    }

    // ------------------------------------------------------------------------

    void OnMotorCmd(int channel, float value)
    {
        if (channel < propellers.Length) {
            propellers[channel].SetMotorPWM(value);
        }
    }
}
