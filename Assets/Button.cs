using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    
    public Propeller[] props;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    void OnMouseDown() {
        // Debug.Log("Hi");

        // // Propeller p = FindObjectOfType<Propeller>();

        // GameObject quad = GameObject.Find("Quadrotor");

        // Propeller[] ps = quad.GetComponentsInChildren<Propeller>();

        // foreach (var p in ps) {
        //     Debug.Log(p.name);
        // }
        

        props[0].stop();

    }
}
