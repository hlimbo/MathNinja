using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchDisappear : MonoBehaviour {



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            //Destroy(this.gameObject);
        }
    }
}
