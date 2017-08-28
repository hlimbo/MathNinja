using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(NumberText))]
public class OnTouchDisappear : MonoBehaviour {

    private NumberText numberText;
    private Collider2D numberCollider;

    private void Awake()
    {
        numberText = GetComponent<NumberText>();
        numberCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        numberCollider.enabled = (NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration) ? true : false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            if (NumberEventManager.elapsedTime < NumberEventManager.UpdateDuration && NumberEventManager.ProblemState == NumberEventManager.Problem_State.NO_ANSWER)
            {
                NumberEventManager.ProblemState = NumberEventManager.Problem_State.ANSWER_PENDING;
                NumberEventManager.user_answer = numberText.value;
                gameObject.SetActive(false);
            }
        }
    }

 
}
