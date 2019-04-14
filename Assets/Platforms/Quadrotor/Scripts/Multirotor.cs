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

    [HeaderAttribute("Communications")]
    public SimCom comms;

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        
    }

    // ------------------------------------------------------------------------
    
    // Update is called once per frame
    void Update()
    {
        
    }

    // ------------------------------------------------------------------------

    void FixedUpdate()
    {
        Vector3 accel, gyro;
        long timestampTicks = imu.GetMeasurement(out accel, out gyro);

        // Not only does this send IMU data, but it also acts as an indication
        // that a new physics update has occurred.
        comms.SendIMU(timestampTicks, accel, gyro);
    }
}
