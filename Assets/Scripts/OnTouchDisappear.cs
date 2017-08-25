using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnTouchDisappear : MonoBehaviour {


    private TMP_Text tmp_text;

    private void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();       
    }

    private void Start()
    {
        int randomValue = Random.Range(0, 9);
        tmp_text.text = randomValue.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            NumberEventManager.attempt_answer = tmp_text.text;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //create another random value
        int randomValue = Random.Range(0, 9);
        tmp_text.text = randomValue.ToString();

    }

 
}
