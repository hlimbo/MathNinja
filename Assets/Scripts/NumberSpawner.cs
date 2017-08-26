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
        //todo: generating possible answers needs to be in sync with NumberEventManager's timing system
        GeneratePossibleAnswers(0, 144);

    }

    //this timer will be in sync with the NumberEventManager timing system found in GenerateQuestion Coroutine
    private IEnumerator GenerateAnswers()
    {
        while(true)
        {
            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration + 1)
            {
                if(NumberEventManager.attempt_answer != null)
                {
                    break;
                }

                yield return new WaitForSeconds(1.0f);
            }
        }


        yield return null;
    }

    //resizes width and height of text's box collider when given a random number
    private void GeneratePossibleAnswers(int min,int max)
    {
        //temporary hack
        int correctAnswerIndex = 1;
        int index = 0;

        foreach(GameObject numberGO in possibleAnswers)
        {
            TextMeshPro numberText = numberGO.GetComponent<TextMeshPro>();
            BoxCollider2D numberBox = numberGO.GetComponent<BoxCollider2D>();
            Debug.Assert(numberText != null, "TextMeshPro Component is not attached to this game object");
            Debug.Assert(numberBox != null, "BoxCollider2D Component is not attached to this game object");

            if (index++ == correctAnswerIndex)
            {
                //this will only be a problem if I decide to call this on Update()
                if(NumberEventManager.product == NumberEventManager.NO_PRODUCT)
                {
                    Debug.Log("a new product for the correct answer to the new question has not been evaluated yet.");
                    Debug.Log("this can be due to NumberEventManager's coroutine displayDelay placed in GenerateQuestion()");
                }
                else
                {
                    string correctAnswer = NumberEventManager.product.ToString();
                    Vector2 boxColliderSize = numberText.GetPreferredValues(correctAnswer);
                    numberBox.size = boxColliderSize;
                    numberText.SetText(correctAnswer);
                }
            }
            else
            {
                string randomNumber = Random.Range(min, max).ToString();
                Vector2 boxColliderSize = numberText.GetPreferredValues(randomNumber);
                numberBox.size = boxColliderSize;
                numberText.SetText(randomNumber);
            }
            
        }
    }
 
}
