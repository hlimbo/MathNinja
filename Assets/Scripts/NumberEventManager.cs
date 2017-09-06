using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NumberEventManager : MonoBehaviour
{
    public enum Problem_State
    {
        NO_ANSWER,
        ANSWER_PENDING,
        CORRECT_ANSWER,
        WRONG_ANSWER
    };

    //used as a key to notify other scripts to know if it is not ready to retrieve
    //the correct answer from the product variable
    public const int NO_PRODUCT = -1;
    public const int NO_ANSWER = -2;

    //this string will display the correct answer after time is up
    public static string answerText;
    public static string questionText;
    public static int user_answer;

    public static int num1 { get; private set; }
    public static int num2 { get; private set; }
    //correct answer for the question
    public static int product { get; private set; }

    //how long it takes to update using a coroutine
    public float updateDuration;

    //how often the interruptable timer updates within its updateDuration
    public float updateFrequency;

    //how long it takes to display the answer in seconds
    public float displayDelay;

    private static GameObject multiplicationQuestion;
    private static TextMeshProUGUI[] gameTexts;

    //used for other scripts that require timing
    public static float startTime { get; private set; }
    public static float elapsedTime { get; private set; }
    public static float UpdateDuration { get; private set; }
    public static float UpdateFrequency { get; private set; }
    public static float DisplayDelay { get; private set; }

    //should consider turning into an enum instead
    //with the following states:
    // NO_ANSWER,CORRECT_ANSWER,WRONG_ANSWER
    public static Problem_State ProblemState;

    [SerializeField]
    public float displayElapsedTime = 0.0f;

    private void Awake()
    {
        user_answer = NO_ANSWER;
        UpdateDuration = updateDuration;
        UpdateFrequency = updateFrequency;
        DisplayDelay = displayDelay;
        elapsedTime = 0.0f;
        ProblemState = Problem_State.NO_ANSWER;
    }

    private void Start()
    {
        multiplicationQuestion = GameObject.Find("MultiplicationQuestion");
        Debug.Assert(multiplicationQuestion != null);

        gameTexts = multiplicationQuestion.GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Assert(gameTexts.Length != 0);

        StartCoroutine(GenerateQuestion());
    }

    //used primarily for when a user decides to change timer values while in unity playmode
    //to reflect the changes that happen for all scripts that require access to the timing variables
    //private void Update()
    //{
    //    //temporary ~ can remove after the final build
    //    UpdateDuration = updateDuration;
    //    UpdateFrequency = updateFrequency;
    //    DisplayDelay = displayDelay;
    //}

    private IEnumerator GenerateQuestion()
    {
        //stop generating new math questions if player is dead
        while (!NinjaController.IsDead)
        {
            int[] values = RandomNumbers(0, 12);
            product = Product(values);
            questionText = string.Format("{0}  x  {1}  =  ", values[0], values[1]);
            answerText = "?";
            gameTexts[0].text = questionText;
            gameTexts[1].text = answerText;
            gameTexts[1].color = Color.white;
            user_answer = NO_ANSWER;
            ProblemState = Problem_State.NO_ANSWER;

            //need to interrupt  WaitForSeconds if player was able to grab an answer in less than
            //updateDuration's time frame.
            startTime = Time.time;

            yield return new WaitForSeconds(updateFrequency);

            displayElapsedTime = elapsedTime = Time.time - startTime;
            while (elapsedTime < updateDuration)
            {
                //I have to possibly check twice for the correct answer
                if (ProblemState == Problem_State.ANSWER_PENDING)
                {
                    break;
                }

                yield return new WaitForSeconds(updateFrequency);
                displayElapsedTime = elapsedTime = Time.time - startTime;
            }

            //check again after time is up
            if (ProblemState == Problem_State.ANSWER_PENDING)
            {
                gameTexts[1].text = user_answer.ToString();
                //evaluate if answer is right or wrong
                ProblemState = (user_answer == product) ? Problem_State.CORRECT_ANSWER : Problem_State.WRONG_ANSWER;
            }

            if (ProblemState == Problem_State.CORRECT_ANSWER)
            {
                gameTexts[1].text = user_answer.ToString();
                gameTexts[1].color = Color.green;
               // Debug.Log("My AnswerC: " + user_answer.ToString());
               // Debug.Log("The AnswerC: " + product.ToString());
            }
            else
            {
                //Debug.Log("My Answer: " + user_answer.ToString());
               // Debug.Log("The Answer: " + product.ToString());
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
