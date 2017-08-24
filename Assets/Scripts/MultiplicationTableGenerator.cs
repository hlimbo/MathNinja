using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicationTableGenerator : MonoBehaviour {

    public float updateDuration;
    private IEnumerator updateLoop;

	// Use this for initialization
	void Start () {

        updateLoop = Loop();
        StartCoroutine(updateLoop);
	}
	
    private IEnumerator Loop()
    {
        while(true)
        {
            int num1 = Random.Range(1, 12);
            int num2 = Random.Range(1, 12);
            Debug.Log("What is " + num1 + " x " + num2 + " ?");
            yield return new WaitForSeconds(updateDuration);
            int product = num1 * num2;
            Debug.Log("Answer: " + product);
        }
    }
}
