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
    //private NumberEventManager eventManager;

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
        updateFrequency = NumberEventManager.TimeStep;
        duration = NumberEventManager.UpdateDuration;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!canStartTimer && corCount == 0)
        {
            StartCoroutine(BeginCooldownTimer());
        }

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

        Debug.Assert(cooldownImage.fillAmount == 1.0f);

        int numIterations = 0;
        float elapsedTime = NumberEventManager.elapsedTime;
        while (elapsedTime < duration + 1)
        {
            if (NumberEventManager.attempt_answer != null)
            {
                canStartTimer = false;
                //yield return new WaitForSeconds(eventManager.displayDelay);
                yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
                corCount--;
                yield break;
            }

            cooldownText.text = countdownTime.ToString();
            cooldownImage.fillAmount = 1.0f - (elapsedTime/ duration);
            countdownTime -= updateFrequency;

            yield return new WaitForSeconds(updateFrequency);
            elapsedTime = NumberEventManager.elapsedTime;
            numIterations++;
        }

        countdownTime = 0.0f;
        cooldownImage.fillAmount = 0.0f;
        cooldownText.text = countdownTime.ToString();
        canStartTimer = false;
        //yield return new WaitForSeconds(eventManager.displayDelay);
        yield return new WaitForSeconds(NumberEventManager.DisplayDelay);
        corCount--;
        yield return null;

    }
}
