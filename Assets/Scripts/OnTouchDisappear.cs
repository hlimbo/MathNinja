using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnTouchDisappear : MonoBehaviour {



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            NumberEventManager.attempt_answer = GetComponent<TMP_Text>().text;
            //StartCoroutine(NumberEventManager.EvaluateAnswer());
        }
    }
}
