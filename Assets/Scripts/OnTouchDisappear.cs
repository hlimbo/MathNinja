using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class OnTouchDisappear : MonoBehaviour {

    private TMP_Text tmp_text;

    private void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            NumberEventManager.attempt_answer = tmp_text.text;
            //gameObject.SetActive(false);
        }
    }

 
}
