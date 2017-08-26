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
    public float yOffset;

    //for rng
    public int min, max;

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

        GeneratePossibleAnswers(0, 144);

     

    }

    //resizes width and height of text's box collider when given a random number
    private void GeneratePossibleAnswers(int min,int max)
    {
        foreach(GameObject numberGO in possibleAnswers)
        {
            TextMeshPro numberText = numberGO.GetComponent<TextMeshPro>();
            BoxCollider2D numberBox = numberGO.GetComponent<BoxCollider2D>();

            string randomNumber = Random.Range(min, max).ToString();
            Vector2 boxColliderSize = numberText.GetPreferredValues(randomNumber);
            numberBox.size = boxColliderSize;
            numberText.SetText(randomNumber);
            
        }
    }
 
}
