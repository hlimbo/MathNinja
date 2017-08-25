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

        while (Time.time - startTime < duration + 1)
        {
            cooldownText.text = countdownTime.ToString();
            cooldownImage.fillAmount = (1.0f - (Time.time - startTime) / duration);
            countdownTime -= updateFrequency;
            yield return new WaitForSeconds(updateFrequency);
        }

        canStartTimer = false;
        cooldownText.text = "0";
        cooldownImage.fillAmount = 0.0f;
        yield return new WaitForSeconds(eventManager.displayDelay);
    }
}
