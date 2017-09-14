using UnityEngine.SceneManagement;
using UnityEngine;

public class playAgainButton : MonoBehaviour {
    void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
	}
}
