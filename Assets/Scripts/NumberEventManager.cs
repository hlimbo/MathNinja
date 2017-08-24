using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberEventManager : MonoBehaviour {

    public static string answer;
    public static string question;
    public static string attempt_answer = null;

    public static int num1;
    public static int num2;
    public static int product;

    //how long it takes to update using a coroutine
    public float updateDuration;

    //how long it takes to display the answer in seconds
    public float displayDelay;

    private static GameObject multiplicationQuestion;
    private static TMP_Text[] gameTexts;

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
            //int[] values = RandomNumbers(1, 12);
            int[] values = new int[2];
            values[0] = 2;
            values[1] = 2;
            product = Product(values);
            question = string.Format("{0}  x  {1}  =  ", values[0], values[1]);
            answer = "?";
            gameTexts[1].text = answer;

            gameTexts[0].text = question;
            gameTexts[1].text = answer;

            yield return new WaitForSeconds(updateDuration);

            //reveal answer

            //todo: call a function that updates the answer text being displayed on screen
            gameTexts[1].text = product.ToString();

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

    public static IEnumerator EvaluateAnswer()
    {
        while (true)
        {
            //evaluate if attempted answer retrieved is correct
            if (attempt_answer != null)
            {
                int a_answer = int.Parse(attempt_answer);

                //display a_answer;
                gameTexts[1].text = attempt_answer;

                if (a_answer != product)
                    Debug.Log("Incorrect answer");
                else
                    Debug.Log("Correct!");

                attempt_answer = null;
                break;
            }

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

}
