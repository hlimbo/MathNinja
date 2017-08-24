using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplicationPrompt : MonoBehaviour {

    private Text prompt;
    private MultiplicationTableGenerator generator;

	// Use this for initialization
	void Start () {
        prompt = GetComponent<Text>();
        generator = FindObjectOfType<MultiplicationTableGenerator>().GetComponent<MultiplicationTableGenerator>();

	}
	
	// Update is called once per frame
	void Update ()
    {
        prompt.text = generator.num1 + " x " + generator.num2;
		
	}
}
