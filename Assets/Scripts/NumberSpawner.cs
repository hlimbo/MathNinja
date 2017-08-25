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


    float disableDuration = 1.0f;
    float startTime;
    float elapsedTime = 0.0f;
    int index = -1;

    // respawn number touched by player
    //prob will have to write more coroutines for this one as well
    void Update ()
    {
        //for (int i = 0; i < possibleAnswers.Count; ++i)
        //{
        //    index = -1;
        //    if (!possibleAnswers[i].activeInHierarchy)
        //    {
        //        index = i;
        //        startTime = Time.time;
        //        elapsedTime = Time.time - startTime;
        //        while (elapsedTime < disableDuration)
        //        {
        //            elapsedTime = Time.time - startTime;
        //        }

        //        possibleAnswers[index].SetActive(true);
        //        break;
        //    }
        //}

        //if (index == -1)
        //    startTime = Time.time;

        //if (index != -1)
        //{
        //    elapsedTime = Time.time - startTime;
        //    while (elapsedTime < disableDuration)
        //    {
        //        elapsedTime = Time.time - startTime;
        //    }

        //    possibleAnswers[index].SetActive(true);
        //}
    }
   
}
