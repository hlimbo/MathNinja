using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberEventManager : MonoBehaviour
{
    public static string answer;
    public static string question;
    public static string attempt_answer = null;

    public static int num1;
    public static int num2;
    public static int product;

    //how long it takes to update using a coroutine
    public float updateDuration;

    //how long each timestep is in the coroutine
    public float timeStep;

    //how long it takes to display the answer in seconds
    public float displayDelay;

    private static GameObject multiplicationQuestion;
    private static TMP_Text[] gameTexts;

    private static bool hasCorrectAnswer = false;

    //used for other scripts that require timing
    public static float startTime { get; private set; }
    public static float elapsedTime { get; private set; }
    public static float UpdateDuration { get; private set; }
    public static float TimeStep { get; private set; }
    public static float DisplayDelay { get; private set; }

    [SerializeField]
    public float displayElapsedTime = 0.0f;

    private void Awake()
    {
        attempt_answer = null;
        UpdateDuration = updateDuration;
        TimeStep = timeStep;
        DisplayDelay = displayDelay;
        elapsedTime = 0.0f;
    }

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
        while (true)
        {
            int[] values = RandomNumbers(0, 12);
            product = Product(values);
            question = string.Format("{0}  x  {1}  =  ", values[0], values[1]);
            answer = "?";
            hasCorrectAnswer = false;
            gameTexts[0].text = question;
            gameTexts[1].text = answer;
            gameTexts[1].color = Color.white;
            attempt_answer = null;


            //need to interrupt  WaitForSeconds if player was able to grab an answer in less than
            //updateDuration's time frame.
            startTime = Time.time;
            displayElapsedTime = elapsedTime = 0.0f;
            while (elapsedTime < updateDuration + 1)
            {

                if (attempt_answer != null)
                {
                    int a_answer = int.Parse(attempt_answer);
                    gameTexts[1].text = attempt_answer;
                    hasCorrectAnswer = (a_answer == product);
                    break;
                }

                yield return new WaitForSeconds(1.0f);
                displayElapsedTime = elapsedTime = Time.time - startTime;
            }


            if (hasCorrectAnswer)
            {
                gameTexts[1].text = attempt_answer;
                gameTexts[1].color = Color.green;
            }
            else
            {
                gameTexts[1].text = product.ToString();
                gameTexts[1].color = Color.red;
            }

            yield return new WaitForSeconds(displayDelay);
        }
    }

    //generates random numbers from minValue inclusive, to maxValue exclusive
    private int[] RandomNumbers(int minValue, int maxValue, int length = 2)
    {
        int[] numbers = new int[length];
        for (int i = 0; i < length; ++i)
            numbers[i] = Random.Range(minValue, maxValue);
        return numbers;
    }

    private int Product(int[] values, int length = 2)
    {
        int product = 1;
        for (int i = 0; i < length; ++i)
            product *= values[i];
        return product;
    }
}
