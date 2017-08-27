using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumberEventManager : MonoBehaviour
{
    //used as a key to notify other scripts to know if it is not ready to retrieve
    //the correct answer from the product variable
    public static int NO_PRODUCT = -1;

    //this string will display the correct answer after time is up
    public static string answer;
    public static string question;
    public static string attempt_answer = null;

    public static int num1;
    public static int num2;
    //correct answer for the question
    public static int product;

    //how long it takes to update using a coroutine
    public float updateDuration;

    //how often the interruptable timer updates within its updateDuration
    public float updateFrequency;

    //how long it takes to display the answer in seconds
    public float displayDelay;

    private static GameObject multiplicationQuestion;
    private static TMP_Text[] gameTexts;

    private static bool hasCorrectAnswer;

    //used for other scripts that require timing
    public static float startTime { get; private set; }
    public static float elapsedTime { get; private set; }
    public static float UpdateDuration { get; private set; }
    public static float UpdateFrequency { get; private set; }
    public static float DisplayDelay { get; private set; }
    public static bool HasCorrectAnswer { get; private set; }

    [SerializeField]
    public float displayElapsedTime = 0.0f;

    private void Awake()
    {
        attempt_answer = null;
        UpdateDuration = updateDuration;
        UpdateFrequency = updateFrequency;
        DisplayDelay = displayDelay;
        elapsedTime = 0.0f;
        HasCorrectAnswer = hasCorrectAnswer = false;
    }

    private void Start()
    {
        multiplicationQuestion = GameObject.Find("MultiplicationQuestion");
        Debug.Assert(multiplicationQuestion != null);

        gameTexts = multiplicationQuestion.GetComponentsInChildren<TMP_Text>();
        Debug.Assert(gameTexts.Length != 0);

        StartCoroutine(GenerateQuestion());
    }

    //used primarily for when a user decides to change timer values while in unity playmode
    //to reflect the changes that happen for all scripts that require access to the timing variables
    private void Update()
    {
        //temporary ~ can remove after the final build
        UpdateDuration = updateDuration;
        UpdateFrequency = updateFrequency;
        DisplayDelay = displayDelay;
    }

    private IEnumerator GenerateQuestion()
    {
        //stop generating new math questions if player is dead
        while (!NinjaController.IsDead)
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
                    HasCorrectAnswer = hasCorrectAnswer;
                    break;
                }

                yield return new WaitForSeconds(updateFrequency);
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

            product = NO_PRODUCT;
            yield return new WaitForSeconds(displayDelay);
        }

        yield return null;
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
