using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruthSensor : MonoBehaviour {

    // The rigid body to get ground truth of
    private Rigidbody rb;

    // ------------------------------------------------------------------------

    public long GetMeasurement(out Vector3 x, out Vector3 v, out Quaternion q)
    {
        // position of the body w.r.t the world
        x = rb.position;

        // velocity of the body w.r.t the world
        v = rb.velocity;

        // Orientation of the body w.r.t the world
        q = rb.rotation;

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
