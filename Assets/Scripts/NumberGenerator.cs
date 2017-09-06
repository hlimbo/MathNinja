using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this will replace NumberSpawner script
public class NumberGenerator : MonoBehaviour {

    [SerializeField]
    private List<GameObject> possibleAnswers;
    public int spawnCount;
    public GameObject numberPrefab;
    public int min, max;

    private PlatformGenerator pGenerator;

    private void Awake()
    {
        possibleAnswers = new List<GameObject>();
    }

    void Start ()
    {
        Debug.Assert(numberPrefab != null);
        pGenerator = FindObjectOfType<PlatformGenerator>();
        
		for(int i = 0;i < spawnCount;++i)
        {
            GameObject numberGO = Instantiate<GameObject>(numberPrefab);
            possibleAnswers.Add(numberGO);
        }

        //parent each possible answer to a platform
        for(int i = 0;i < pGenerator.platforms.Count;++i)
        {
            possibleAnswers[i].transform.parent = pGenerator.platforms[i].transform;

            int randomNumber = Random.Range(min, max + 1);
            possibleAnswers[i].GetComponent<NumberText>().value = randomNumber;
            possibleAnswers[i].GetComponent<NumberText>().text = randomNumber.ToString();
            possibleAnswers[i].GetComponent<BoxCollider2D>().size = possibleAnswers[i].GetComponent<NumberText>().GetPreferredValues();

            float halfPlatformHeight = pGenerator.platforms[i].GetComponent<BoxCollider2D>().size.y / 2;
            float halfPlatformWidth = pGenerator.platforms[i].GetComponent<BoxCollider2D>().size.x / 2;
            possibleAnswers[i].transform.localPosition = new Vector2(halfPlatformWidth * 0.5f, possibleAnswers[i].GetComponent<BoxCollider2D>().size.y / 2 + halfPlatformHeight);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
