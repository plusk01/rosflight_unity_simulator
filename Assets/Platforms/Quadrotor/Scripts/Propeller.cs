using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour {

    public bool ccw = false;

    private float rotationAmount = 500.0f;

    private bool stopped = false;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        if (stopped) return;

        int dir = (ccw) ? -1 : 1;
        transform.Rotate(Vector3.up, dir * rotationAmount * Time.deltaTime);
    }

    public void stop() {
        stopped = !stopped;
    }
}
