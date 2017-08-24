using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberEventManager : MonoBehaviour {

    public static string answer;
    public static string question;

    public static int num1;
    public static int num2;
    public static int product;

    //how long it takes to update using a coroutine
    public float updateDuration;

    //how long it takes to display the answer in seconds
    public float displayDelay;

    [SerializeField]
    private GameObject multiplicationQuestion;

    [SerializeField]
    private TMP_Text[] gameTexts;

    private void Start()
    {
        multiplicationQuestion = GameObject.Find("MultiplicationQuestion");
        Debug.Assert(multiplicationQuestion != null);

        gameTexts = multiplicationQuestion.GetComponentsInChildren<TMP_Text>();
        Debug.Assert(gameTexts.Length != 0);

        StartCoroutine(GenerateQuestion());
    }

    private IEnumerator GenerateQuestion()
    {
        while(true)
        {
            int[] values = RandomNumbers(1, 12);
            product = Product(values);
            question = string.Format("{0}  x  {1}  =  ", values[0], values[1]);
            answer = "?";

            //todo: call a function that updates the question text being displayed on screen.
            gameTexts[0].text = question;
            gameTexts[1].text = answer;
            Debug.Log(question + answer);
            yield return new WaitForSeconds(updateDuration);

            //reveal answer

            //todo: call a function that updates the answer text being displayed on screen
            gameTexts[1].text = product.ToString();
            Debug.Log("The Answer is: " + product);

            yield return new WaitForSeconds(displayDelay);
        }
    }

    //generates random numbers from minValue inclusive, to maxValue exclusive
    private int[] RandomNumbers(int minValue,int maxValue,int length = 2)
    {
        int[] numbers = new int[length];
        for(int i = 0;i < length;++i)
            numbers[i] = Random.Range(minValue, maxValue);
        return numbers;
    }

    private int Product(int[] values,int length = 2)
    {
        int product = 1;
        for (int i = 0; i < length; ++i)
            product *= values[i];
        return product;
    }

}
