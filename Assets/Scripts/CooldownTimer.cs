using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownTimer : MonoBehaviour {

    [SerializeField]
    private float duration;
    [SerializeField]
    private float countdownTime;

    private Image cooldownImage;
    private Text cooldownText;

    [SerializeField]
    private float updateFrequency;

    [SerializeField]
    private bool canStartTimer = false;

    [SerializeField]
    private int corCount;

	void Awake ()
    {
        cooldownImage = transform.GetChild(0).GetComponent<Image>();
        Debug.Assert(cooldownImage != null);
        cooldownText = transform.GetChild(1).GetComponent<Text>();
        Debug.Assert(cooldownText != null);
	}

    private void Start()
    {
        countdownTime = duration;
        cooldownText.text = duration.ToString();
        corCount = 0;
        updateFrequency = NumberEventManager.UpdateFrequency;
        duration = NumberEventManager.UpdateDuration;

        //StartCoroutine(BeginCooldownTimer());

        WorldEventSystem.OnPreTimerElapsed += InitializeCooldownValues;
        WorldEventSystem.OnPostTimerElapsed += ResetCooldownValues;
        WorldEventSystem.OnCurrentTimerElapsed += UpdateCooldownValues;
    }

    //private void OnEnable()
    //{
    //    WorldEventSystem.OnPreTimerElapsed += InitializeCooldownValues;
    //    WorldEventSystem.OnPostTimerElapsed += ResetCooldownValues;
    //    WorldEventSystem.OnCurrentTimerElapsed += UpdateCooldownValues;
    //}

    private void OnDestroy()
    {
        WorldEventSystem.OnPreTimerElapsed -= InitializeCooldownValues;
        WorldEventSystem.OnPostTimerElapsed -= ResetCooldownValues;
        WorldEventSystem.OnCurrentTimerElapsed -= UpdateCooldownValues;
    }

    // Update is called once per frame
    void Update ()
    {
        //if (NinjaController.IsDead)
        //{
        //    if (corCount != 0)
        //    {
        //        corCount--;
        //        StopCoroutine(BeginCooldownTimer());
        //    }
        //}
        //else if (!canStartTimer && corCount == 0)
        //{
        //    StartCoroutine(BeginCooldownTimer());
        //}

        ////debugging
        //if (corCount > 1)
        //{
        //    Debug.Log("corCount: " + corCount);
        //}
	}

    //bound to event OnPreTimerElapsed
    private void InitializeCooldownValues()
    {
        cooldownImage.fillAmount = 1.0f;
        countdownTime = duration;
        cooldownText.text = countdownTime.ToString();
        countdownTime -= WorldEventSystem.UpdateFrequency;
    }

    //bound to event OnPostTimerElapsed
    private void ResetCooldownValues()
    {
        if (countdownTime < 0.0f)
            countdownTime = 0.0f;
        if(cooldownImage.fillAmount < .1f)
            cooldownImage.fillAmount = 0.0f;
        cooldownText.text = countdownTime.ToString();
    }

    //bound to event OnCurrentTimerElapsed
    private void UpdateCooldownValues()
    {
        cooldownText.text = countdownTime.ToString();
        cooldownImage.fillAmount = 1.0f - (WorldEventSystem.ElapsedTime / duration);
        countdownTime -= WorldEventSystem.UpdateFrequency;
    }

    //i probably need to rewrite this such that the timings match with the NumberEventManager's coroutine timings
    private IEnumerator BeginCooldownTimer()
    {
        while(!NinjaController.IsDead)
        {
            InitializeCooldownValues();
            yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);

            while(NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration)
            {
                //...
                UpdateCooldownValues();
                yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
            }

            ResetCooldownValues();
            yield return new WaitForSeconds(NumberEventManager.DisplayDelay);

        }


        //corCount++;
        //canStartTimer = true;
        //cooldownImage.fillAmount = 1.0f; 
        //countdownTime = duration;
        //cooldownText.text = countdownTime.ToString();
        //countdownTime -= NumberEventManager.UpdateFrequency;
        //yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);

        //int numIterations = 0;
        //float elapsedTime = NumberEventManager.elapsedTime;
        //while (elapsedTime < duration)
        //{
        //  //note: I don't think these conditions are necessary anymore since 
        //  //there seems to be 1 condition only needed to interrupt the main timer!
        //    if (NumberEventManager.user_answer != NumberEventManager.NO_ANSWER)
        //    {
        //        canStartTimer = false;
        //        yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        //        corCount--;
        //        yield break;
        //    }
        //    else if(NinjaController.IsDead)
        //    {
        //        yield break;
        //    }

        //    cooldownText.text = countdownTime.ToString();
        //    cooldownImage.fillAmount = 1.0f - (elapsedTime/ duration);
        //    countdownTime -= NumberEventManager.UpdateFrequency;

        //    yield return new WaitForSeconds(NumberEventManager.UpdateFrequency);
        //    elapsedTime = NumberEventManager.elapsedTime;
        //    numIterations++;
        //}

        ////countdownTime = 0.0f;
        //cooldownImage.fillAmount = 0.0f;
        //cooldownText.text = countdownTime.ToString();
        //canStartTimer = false;
        //yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        //corCount--;
        //yield return null;

    }
}
