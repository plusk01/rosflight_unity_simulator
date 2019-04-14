using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMU : MonoBehaviour {

    // The rigid body that the IMU is measuring (i.e., strapped to)
    private Rigidbody rb;

    // used for calculating an estimate of the instantaneous accel
    private Vector3 lastVelocity = new Vector3(0.0f, 0.0f, 0.0f);

    // ------------------------------------------------------------------------

    public long GetMeasurement(out Vector3 accel, out Vector3 gyro)
    {
        // Estimate the instantaneous acceleration
        Vector3 a = (rb.velocity - lastVelocity) / Time.deltaTime;
        lastVelocity = rb.velocity;

        // Orientation of the world w.r.t the body
        Quaternion q_BW = Quaternion.Inverse(rb.rotation);

        // Create the true IMU measurements
        Vector3 accel_body = q_BW * (a - Physics.gravity);
        Vector3 angvel_body = q_BW * rb.angularVelocity;

        // Make some noise!
        accel = accel_body;
        gyro = angvel_body;

        // stamp this measurement with number of ticks since UNIX epoch.
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks;
    }

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------
    
    void Start() {
        // grab a reference to the first rigid body above me
        rb = GetComponentInParent<Rigidbody>();
    }

}
