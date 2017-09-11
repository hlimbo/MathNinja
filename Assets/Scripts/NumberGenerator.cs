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

    private Stack<GameObject> inactiveGOs;

    private PlatformGenerator pGenerator;

    //for debugging
    public GameObject locatorPrefab;
    private GameObject locatorGO;

    private void Awake()
    {
        possibleAnswers = new List<GameObject>();
        inactiveGOs = new Stack<GameObject>();

        locatorGO = Instantiate<GameObject>(locatorPrefab);
        
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
            possibleAnswers[i].transform.SetParent(pGenerator.platforms[i].transform);

            int randomNumber = Random.Range(min, max + 1);
            possibleAnswers[i].GetComponent<NumberText>().value = randomNumber;
            possibleAnswers[i].GetComponent<NumberText>().text = randomNumber.ToString();
            possibleAnswers[i].GetComponent<BoxCollider2D>().size = possibleAnswers[i].GetComponent<NumberText>().GetPreferredValues();

            float halfPlatformHeight = pGenerator.platforms[i].GetComponent<BoxCollider2D>().size.y / 2;
            float halfPlatformWidth = pGenerator.platforms[i].GetComponent<BoxCollider2D>().size.x / 2;
            possibleAnswers[i].transform.localPosition = new Vector2(halfPlatformWidth + possibleAnswers[i].GetComponent<BoxCollider2D>().size.x, possibleAnswers[i].GetComponent<BoxCollider2D>().size.y / 2 + halfPlatformHeight);
        }

        //StartCoroutine(GenerateAnswers());

        //init events
        WorldEventSystem.OnPreTimerElapsed += ReactivateNumberGOs;
        WorldEventSystem.OnPreTimerElapsed += ConstructPossibleAnswers;
        WorldEventSystem.OnPostTimerElapsed += DeactivateNumberGOs;
    }

    //private void OnEnable()
    //{
    //    WorldEventSystem.OnPreTimerElapsed += ReactivateNumberGOs;
    //    WorldEventSystem.OnPreTimerElapsed += ConstructPossibleAnswers;
    //    WorldEventSystem.OnPostTimerElapsed += DeactivateNumberGOs;
    //}

    private void OnDestroy()
    {
        WorldEventSystem.OnPreTimerElapsed -= ReactivateNumberGOs;
        WorldEventSystem.OnPreTimerElapsed -= ConstructPossibleAnswers;
        WorldEventSystem.OnPostTimerElapsed -= DeactivateNumberGOs;
    }

    //function bound to event OnPreTimerElapsed
    private void ReactivateNumberGOs()
    {
        while (inactiveGOs.Count != 0)
        {
            GameObject reactivatedGO = inactiveGOs.Pop();
            reactivatedGO.SetActive(true);
        }
    }

    //function bound to event OnPostTimerElapsed
    private void DeactivateNumberGOs()
    {
        //set all numberGOs as disabled...this only works if an answer wasn't grabbed
        foreach (GameObject numberGO in possibleAnswers)
        {
            numberGO.SetActive(false);
            inactiveGOs.Push(numberGO);
        }
    }

    //function bound to event OnPreTimerElapsed
    private void ConstructPossibleAnswers()
    {

        //find a set of platforms that are not about to enter the left most edge of the screen or are already past the left most edge of the screen
        List<int> platformIndices = new List<int>();
        float camHalfWidth = (Camera.main.orthographicSize * Camera.main.aspect * 2) / 2;
        //correct answers should only spawn towards the right halfside of the camera
        float xSpread = 0.0f;
        float leftCamEdge = Camera.main.transform.position.x - camHalfWidth * xSpread;
        for (int i = 0; i < pGenerator.platforms.Count; ++i)
        {
            //this works or using Transform.transformPoint where the passed parameter is relative to the object calling this method
            float worldPosX = pGenerator.transform.position.x + pGenerator.platforms[i].transform.position.x;
            Vector3 absolutePos = pGenerator.transform.TransformPoint(pGenerator.platforms[i].transform.position);
            float leftPlatformSide = absolutePos.x - pGenerator.platformPrefab.GetComponent<BoxCollider2D>().size.x / 2;
            if (leftPlatformSide > leftCamEdge)
            {
                Debug.Assert(absolutePos.x == worldPosX);
                Debug.Log("WorldPosX: " + worldPosX);
                Debug.Log(leftPlatformSide + " > " + leftCamEdge);
                platformIndices.Add(i);
            }
        }

        //temporary hack ~ generate possible answers!
        int platformAnswerIndex = pGenerator.platforms.Count - 1;
        if (platformIndices.Count != 0)
        {
            platformAnswerIndex = platformIndices[Random.Range(0, platformIndices.Count)];
            Debug.Log("Platform correct answer index: " + platformAnswerIndex);
            float worldPosX = pGenerator.transform.position.x + pGenerator.platforms[platformAnswerIndex].transform.position.x;
            Debug.Log("Platform world position x: " + worldPosX);

            //debugging
            locatorGO.transform.SetParent(pGenerator.platforms[platformAnswerIndex].transform);
            locatorGO.transform.localPosition = new Vector2();
        }

        for (int i = 0; i < spawnCount; ++i)
        {
            if (i == platformAnswerIndex)
            {
                //grab the platform that will have the correct answer!
                GameObject correctNumberGO = pGenerator.platforms[platformAnswerIndex].transform.GetChild(0).gameObject;
                NumberText numberText = correctNumberGO.GetComponent<NumberText>();
                BoxCollider2D numberBox = correctNumberGO.GetComponent<BoxCollider2D>();

                numberText.value = NumberEventManager.product;
                numberText.text = NumberEventManager.product.ToString();
                Vector2 boxColliderSize = numberText.GetPreferredValues();
                numberBox.size = boxColliderSize;
            }
            else
            {
                GameObject numberGO = possibleAnswers[i];
                //to ensure correct answers don't get overridden ~ there has to be a better way to write this logically
                if (numberGO.transform.parent != pGenerator.platforms[platformAnswerIndex].transform)
                {
                    NumberText numberText = numberGO.GetComponent<NumberText>();
                    BoxCollider2D numberBox = numberGO.GetComponent<BoxCollider2D>();

                    int randomNumber = Random.Range(min, max + 1);
                    numberText.value = randomNumber;
                    numberText.text = randomNumber.ToString();
                    Vector2 boxColliderSize = numberText.GetPreferredValues();
                    numberBox.size = boxColliderSize;
                }
            }

        }
    }

    private IEnumerator GenerateAnswers()
    {
        while(!NinjaController.IsDead)
        {
            ReactivateNumberGOs();
            ConstructPossibleAnswers();

            yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);

            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration)
            {
                //interruptable timer condition setup ~  we won't need to turn this into an event
                //since when the player touches a possible answer, the timer will already be interrupted
                //when ProblemState == ANSWER_PENDING
                //As a result, this check becomes redundant and not necessary to include as a function
                //to bind to the OnTimerInterrupted delegate
                if(NumberEventManager.ProblemState != NumberEventManager.Problem_State.NO_ANSWER)
                {
                    //find all game objects that become disabled and push them into our stack
                    //this only works if the answer wasn't grabbed
                    //foreach(GameObject numberGO in possibleAnswers)
                    //{
                    //    if (!numberGO.activeInHierarchy)
                    //        inactiveGOs.Push(numberGO);
                    //}
                    break;
                }

                yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
            }

            DeactivateNumberGOs();

            yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        }

        yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
    }
}
