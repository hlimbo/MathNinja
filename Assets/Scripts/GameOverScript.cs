using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour {

    [SerializeField]
    private GameObject gameOverWidget;

    public float updateFrequency;

    void Start()
    {
        gameOverWidget = GameObject.Find("GameOverWidget");
        Debug.Assert(gameOverWidget != null);
        gameOverWidget.SetActive(false);

        StartCoroutine(CheckForGameOverState());

	}
	
    private IEnumerator CheckForGameOverState()
    {
        while(!NinjaController.IsDead)
        {
            yield return new WaitForSeconds(updateFrequency);
        }

        gameOverWidget.SetActive(true);
        yield return null;
    }

}
