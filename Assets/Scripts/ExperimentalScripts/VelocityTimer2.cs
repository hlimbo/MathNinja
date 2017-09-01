using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * s = displacement
 * u = initial velocity
 * v = final velocity
 * a = acceleration
 * t = time
 *
 *
 */ 

//utlizing kinematic equation (suvat): 
/*
 * v^2 = u^2 + 2*a*s;
 * v = sqrt(2*a*s) where u = 0;
 * 
 */ 
public class VelocityTimer2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Physics2D gravity: " + Physics2D.gravity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
