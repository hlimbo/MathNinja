using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * WorldEventSystem (Event Driven Approach)
     * Description:
     * script will be responsible for managing the timing for any
     * scripts whose functions are required to be executed when certain
     * conditions are met.  
 */

public class WorldEventSystem : MonoBehaviour {

    //measured in seconds how long it takes for the main coroutine loop to update
    //this is the longest possible time the loop can continue without it being interrupted
    public float updateDuration;

    //how often the interruptible timer updates within its updateDuration measured in seconds
    public float updateFrequency;

    //how long it takes to start at the beginning of the coroutine loop body which is measured in seconds
    public float displayDelay;

    [SerializeField]
    private float startTime;
    [SerializeField]
    private float elapsedTime;

    public static float ElapsedTime { get; private set; }
    public static float StartTime { get; private set; }
    public static float UpdateDuration { get; private set; }
    public static float UpdateFrequency { get; private set; }
    public static float DisplayDelay { get; private set; }

    //the following events can be subscribed to in other scripts that require
    //to follow the WorldEventSystem's timer for its logic to function properly
    public delegate void PreTimerAction();
    public static event PreTimerAction OnPreTimerElapsed;

    public delegate void PostTimerAction();
    public static event PostTimerAction OnPostTimerElapsed;

    public delegate bool InterruptTimerAction();
    public static event InterruptTimerAction OnTimerInterrupted;

    public delegate void CurrentTimerAction();
    public static event CurrentTimerAction OnCurrentTimerElapsed;

    public delegate void EvaluatePlayerAction();
    public static event EvaluatePlayerAction OnPlayerActionEvaluated;

    //bug: the update duration resets to 0 after replaying game
    private void Awake()
    {
        //should probably move these variables into their own script
        UpdateDuration = updateDuration;
        UpdateFrequency = updateFrequency;
        DisplayDelay = displayDelay;
        ElapsedTime = elapsedTime = 0.0f;

        Debug.Log("In awake");
        Debug.Log("Update Duration: " + UpdateDuration);
        Debug.Log("Update Frequency: " + UpdateFrequency);
        Debug.Log("Display Delay: " + DisplayDelay);
        Debug.Log("Elapsed Time: " + ElapsedTime);
    }

    // Use this for initialization
    void Start () {
        UpdateDuration = updateDuration;
        UpdateFrequency = updateFrequency;
        DisplayDelay = displayDelay;
        ElapsedTime = elapsedTime = 0.0f;

        Debug.Log("In start");
        Debug.Log("Update Duration: " + UpdateDuration);
        Debug.Log("Update Frequency: " + UpdateFrequency);
        Debug.Log("Display Delay: " + DisplayDelay);
        Debug.Log("Elapsed Time: " + ElapsedTime);

        StartCoroutine(TimerEventLoop());
	}

    private IEnumerator TimerEventLoop()
    {
        //ok we have to add a busy wait here to ensure all delegates are loaded up with functions to execute
        //probably should set the script execution order instead
        while (OnPreTimerElapsed == null)
        {
            yield return new WaitForEndOfFrame();
        }

        // while(!NinjaController.IsDead)
        //the issue with this code is that there are multiple ways
        //a player can die and since it only returns the last value of the function
        //subscribed to this delegate..
        // while(!OnPlayerDeath())
        while (!NinjaController.IsDead)
        {
            //event called OnPreTimerLoop where scripts
            //can subscribe to this event if they want their functions
            //to be called at the very beginning of the coroutine loop
            OnPreTimerElapsed();
            //for some reason it the line doesn't get past here?
            StartTime = startTime = Time.time;
            yield return new WaitForSeconds(updateFrequency);
            ElapsedTime = elapsedTime = Time.time - startTime;
            //interruptible timer here
            bool canInterrupt = false;
            while (elapsedTime < updateDuration)
            {
                //if one of the functions held in the delegate return true.. then canInterrupt = true,otherwise remain false
                foreach (InterruptTimerAction interruptAction in OnTimerInterrupted.GetInvocationList())
                {
                    //it needs to run all functions subscribed to this event to ensure data stablility
                    if (interruptAction())
                        canInterrupt = true;
                }

                if (canInterrupt) break;

                OnCurrentTimerElapsed();

                yield return new WaitForSeconds(updateFrequency);
                ElapsedTime = elapsedTime = Time.time - startTime;
            }

            OnPostTimerElapsed();
            OnPlayerActionEvaluated();

            yield return new WaitForSeconds(displayDelay);
        }

        yield return null;
    }
}
