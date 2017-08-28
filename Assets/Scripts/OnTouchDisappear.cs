using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(NumberText))]
public class OnTouchDisappear : MonoBehaviour {

    private NumberText numberText;

    private void Awake()
    {
        numberText = GetComponent<NumberText>();       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            if (NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration && NumberEventManager.user_answer == NumberEventManager.NO_ANSWER)
            {
                NumberEventManager.user_answer = numberText.value;
                gameObject.SetActive(false);
            }
        }
    }

 
}
