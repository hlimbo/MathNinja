using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownTimer : MonoBehaviour {

    [SerializeField]
    private float startTime;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float countdownTime;

    private Image cooldownImage;
    private TextMeshProUGUI cooldownText;
    private NumberEventManager eventManager;

    public float updateFrequency;

    [SerializeField]
    private bool canStartTimer = false;
    [SerializeField]
    private float elapsedTime = 0.0f;

	void Awake ()
    {
        cooldownImage = transform.GetChild(0).GetComponent<Image>();
        Debug.Assert(cooldownImage != null);
        cooldownText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Debug.Assert(cooldownText != null);

        eventManager = FindObjectOfType<NumberEventManager>();
        Debug.Assert(eventManager != null);
	}

    private void Start()
    {
        duration = eventManager.updateDuration;
        countdownTime = duration;
        cooldownText.text = duration.ToString();
    }

    // Update is called once per frame
    void Update ()
    {
		if(!canStartTimer)
        {
            startTime = NumberEventManager.currentTime;
            Debug.Assert(startTime == NumberEventManager.currentTime);
            StartCoroutine(BeginCooldownTimer());
        }
	}

    private IEnumerator BeginCooldownTimer()
    {

        canStartTimer = true;
        cooldownImage.fillAmount = 1.0f;
  
        countdownTime = duration;
        cooldownText.text = countdownTime.ToString();

        elapsedTime = Time.time - startTime;
        while (elapsedTime < duration + 1)
        {
            cooldownText.text = countdownTime.ToString();
            cooldownImage.fillAmount = 1.0f - (elapsedTime / duration);
            countdownTime -= updateFrequency;
            if (NumberEventManager.attempt_answer != null)
            { 
                break;
            }

            yield return new WaitForSeconds(updateFrequency);
            elapsedTime = Time.time - startTime;
        }

        //not interrupted
        if (elapsedTime > duration + 1)
        {
            canStartTimer = false;
            countdownTime = 0.0f;
            cooldownImage.fillAmount = 0.0f;
            cooldownText.text = countdownTime.ToString();
        }
        yield return new WaitForSeconds(eventManager.displayDelay);

    }
}
