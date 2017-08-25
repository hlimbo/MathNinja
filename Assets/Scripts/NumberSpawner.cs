using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberSpawner : MonoBehaviour {

    [SerializeField]
    private List<GameObject> possibleAnswers;
    public int size;
    public GameObject numberPrefab;
    public float xOffset;

    private void Awake()
    {
        possibleAnswers = new List<GameObject>();
    }

    // Use this for initialization
    void Start ()
    {
        //spawn number objects
        for (int i = 0; i < size; ++i)
        {
            GameObject numberGO = Instantiate<GameObject>(numberPrefab, this.transform);
            numberGO.transform.localPosition = new Vector3((i * xOffset),0, 0);
            possibleAnswers.Add(numberGO);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
