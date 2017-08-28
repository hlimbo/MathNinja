using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//an event driven approach might have been better here since coroutines require lots of synchronization
public class NumberSpawner : MonoBehaviour {

    [SerializeField]
    private List<GameObject> possibleAnswers;
    public int size;
    public GameObject numberPrefab;
    public float xOffset;
    public float yOffset;

    //for rng
    public int min, max;

    //workaround could be to store my own copies of the random values as ints since the text rendered on screen
    //doesn't reflect its associated string value sometimes.
    public static int[] numbers;//use this to compare against answer

    private Stack<GameObject> inactiveGOs;

    private void Awake()
    {
        possibleAnswers = new List<GameObject>();
        inactiveGOs = new Stack<GameObject>();

        numbers = new int[3];
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

        StartCoroutine(GenerateAnswers());
    }

    private void Update()
    {
        if(NinjaController.IsDead)
        {
            //if not all numberGOs became inactive... set them as inactive game objects
            //put this here so the numberGOs immediately are set inactive
            if(inactiveGOs.Count != possibleAnswers.Count)
            {
                foreach (GameObject numberGO in possibleAnswers)
                {
                    if (numberGO.activeInHierarchy)
                    {
                        numberGO.SetActive(false);
                        inactiveGOs.Push(numberGO);
                    }
                }
            }
        }
    }

    //this timer will be in sync with the NumberEventManager timing system found in GenerateQuestion Coroutine
    private IEnumerator GenerateAnswers()
    {
        //stop the coroutine when the player is dead
        while(!NinjaController.IsDead)
        {
            //reactivates any disabled gameobjects
            while (inactiveGOs.Count != 0)
            {
                GameObject reactivatedGO = inactiveGOs.Pop();
                reactivatedGO.SetActive(true);
            }

            GeneratePossibleAnswers(min, max);

            yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);

            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration)
            {
                //interruptable timer condition setup
                if(NumberEventManager.user_answer != NumberEventManager.NO_ANSWER)
                {
                    //find all game objects that become disabled
                    foreach(GameObject numberGO in possibleAnswers)
                    {
                        if(!numberGO.activeInHierarchy)
                            inactiveGOs.Push(numberGO);
                    }

                    break;
                }

                yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
            }

            //set all numberGOs as disabled
            foreach (GameObject numberGO in possibleAnswers)
            {
                numberGO.SetActive(false);
                inactiveGOs.Push(numberGO);
            }

            yield return new WaitForSeconds(NumberEventManager.DisplayDelay);

        }

        yield return null;
    }

    //resizes width and height of text's box collider when given a random number
    private void GeneratePossibleAnswers(int min,int max)
    {
        //temporary hack
        int correctAnswerIndex = Random.Range(0, size);
        int index = 0;

        foreach(GameObject numberGO in possibleAnswers)
        {
            NumberText numberText = numberGO.GetComponent<NumberText>();
            BoxCollider2D numberBox = numberGO.GetComponent<BoxCollider2D>();
            Debug.Assert(numberText != null, "NumberText Component is not attached to this game object");
            Debug.Assert(numberBox != null, "BoxCollider2D Component is not attached to this game object");

            if (index++ == correctAnswerIndex)
            {
                numberText.value = NumberEventManager.product;
                numberText.text = NumberEventManager.product.ToString();
                Vector2 boxColliderSize = numberText.GetPreferredValues();
                numberBox.size = boxColliderSize;
            }
            else
            {
                int randomNumber = Random.Range(min, max);
                numberText.value = randomNumber;
                numberText.text = randomNumber.ToString();
                Vector2 boxColliderSize = numberText.GetPreferredValues();
                numberBox.size = boxColliderSize;
            }
            
        }
    }

    //private void DebugPrintNumbers()
    //{
    //    Debug.Log("Numbers set: ");
    //    for(int i = 0;i < possibleAnswers.Count;++i)
    //    {
    //        GameObject numberGO = possibleAnswers[i];
    //        NumberText numberText = numberGO.GetComponent<NumberText>();
    //        Debug.Log(numberText.text);
    //        int.TryParse(numberText.text,out numbers[i]);
    //    }
    //}

}
