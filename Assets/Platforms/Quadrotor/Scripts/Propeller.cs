using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour {

    [HeaderAttribute("Physical Properties")]
    public bool clockwise = false;
    public float maxThrust = 12.95f; // [N]
    public float maxTorque = 1.0f; // [Nm]

    [HeaderAttribute("Visual Aesthetics")]
    public float rotationMultiplier = 500.0f;

    // PWM command sent to esc/motor
    private float pwm = 0.0f;

    // The rigid body that the props are acting on
    private Rigidbody rb;

    // ------------------------------------------------------------------------

    public void SetMotorPWM(float pwm)
    {
        this.pwm = pwm;
    }

    // ------------------------------------------------------------------------
    // Private Methods
    // ------------------------------------------------------------------------

    // Use this for initialization
    void Start() {
        // grab a reference to the first rigid body above me
        rb = GetComponentInParent<Rigidbody>();
    }

    // ------------------------------------------------------------------------
    
    // Update is called once per frame
    void Update() {
        int dir = GetRotationDirection();
        transform.Rotate(Vector3.up, pwm * dir * rotationMultiplier * Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + transform.up * pwm, Color.white);
    }

    // ------------------------------------------------------------------------

    void FixedUpdate()
    {
        float force = CalculateThrust(pwm);
        rb.AddForceAtPosition(transform.up * force, transform.position);

        int sign = -1*GetRotationDirection();
        float torque = sign * CalculateTorque(pwm);
        rb.AddRelativeTorque(Vector3.up * torque);
    }

    // ------------------------------------------------------------------------

    float CalculateThrust(float pwm)
    {
        return pwm * maxThrust;
    }

    // ------------------------------------------------------------------------

    float CalculateTorque(float pwm)
    {
        return pwm * maxTorque;
    }

    // ------------------------------------------------------------------------

    int GetRotationDirection()
    {
        return (clockwise) ? 1 : -1;;
    }
}
