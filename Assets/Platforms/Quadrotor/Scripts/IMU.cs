using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMU : MonoBehaviour {

    [HeaderAttribute("Accelerometer Noise Parameters")]
    public float acc_stddev = 0.001f; // [m/s/s]
    public float acc_bias_range = 0.0f; // [m/s/s]
    public float acc_bias_walk_stddev = 0.0f; // [m/s/s]

    [HeaderAttribute("Gyro Noise Parameters")]
    public float gyro_stddev = 0.001f; // [rad/s]
    public float gyro_bias_range = 0.0f; // [rad/s]
    public float gyro_bias_walk_stddev = 0.0f; // [rad/s]

    // random number generator for noise models
    private System.Random rand = new System.Random();

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
        accel = AccelerometerNoiseModel(accel_body);
        gyro = GyroNoiseModel(angvel_body);

        // stamp this measurement with number of ticks since UNIX epoch.
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks;
    }

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    float randn()
    {
        // Box-Muller transform
        // Used to generate i.i.d normals -- N(0,1)
        // see: https://stackoverflow.com/a/218600/2392520

        double u1 = 1.0 - rand.NextDouble(); // uniform(0,1]
        double u2 = 1.0 - rand.NextDouble(); // uniform(0,1]

        // normal(0,1)
        double normal = Math.Sqrt(-2.0*Math.Log(u1)) * Math.Sin(2.0*Math.PI * u2);
        return (float)normal;
    }

    // ------------------------------------------------------------------------

    Vector3 AccelerometerNoiseModel(Vector3 accel)
    {
        Vector3 eta = new Vector3(randn(), randn(), randn());
        return accel + acc_stddev*eta;
    }

    // ------------------------------------------------------------------------

    Vector3 GyroNoiseModel(Vector3 gyro)
    {
        Vector3 eta = new Vector3(randn(), randn(), randn());
        return gyro + gyro_stddev*eta;
    }

    // ------------------------------------------------------------------------

    void Start() {
        // grab a reference to the first rigid body above me
        rb = GetComponentInParent<Rigidbody>();
    }

}
