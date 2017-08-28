using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownTimer : MonoBehaviour {

    [SerializeField]
    private float duration;
    [SerializeField]
    private float countdownTime;

    private Image cooldownImage;
    private TextMeshProUGUI cooldownText;

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
        cooldownText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Debug.Assert(cooldownText != null);
	}

    private void Start()
    {
        countdownTime = duration;
        cooldownText.text = duration.ToString();
        corCount = 0;
        updateFrequency = NumberEventManager.UpdateFrequency;
        duration = NumberEventManager.UpdateDuration;
    }

    // Update is called once per frame
    void Update ()
    {
        if (NinjaController.IsDead)
        {
            if (corCount != 0)
            {
                corCount--;
                StopCoroutine(BeginCooldownTimer());
            }
        }
        else if (!canStartTimer && corCount == 0)
        {
            StartCoroutine(BeginCooldownTimer());
        }

        //debugging
        if (corCount > 1)
        {
            Debug.Log("corCount: " + corCount);
        }
	}

    //the problem is that it starts another coroutine
    private IEnumerator BeginCooldownTimer()
    {
        corCount++;
        canStartTimer = true;
        cooldownImage.fillAmount = 1.0f; 
        countdownTime = duration;
        cooldownText.text = countdownTime.ToString();

        Debug.Assert(cooldownImage.fillAmount == 1.0f);

        int numIterations = 0;
        float elapsedTime = NumberEventManager.elapsedTime;
        while (elapsedTime < duration)
        {
            if (NumberEventManager.user_answer != NumberEventManager.NO_ANSWER)
            {
                canStartTimer = false;
                yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
                corCount--;
                yield break;
            }
            else if(NinjaController.IsDead)
            {
                yield break;
            }

            cooldownText.text = countdownTime.ToString();
            cooldownImage.fillAmount = 1.0f - (elapsedTime/ duration);
            countdownTime -= updateFrequency;

            yield return new WaitForSeconds(updateFrequency);
            elapsedTime = NumberEventManager.elapsedTime;
            numIterations++;
        }

        if(countdownTime < 0.0f)
        {
            Debug.Log("Countdown time: " + countdownTime);
        }
        //countdownTime = 0.0f;
        cooldownImage.fillAmount = 0.0f;
        cooldownText.text = countdownTime.ToString();
        canStartTimer = false;
        yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        corCount--;
        yield return null;

    }
}
