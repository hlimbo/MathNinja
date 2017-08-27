using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgainScript : MonoBehaviour
{

    public void ReloadLevel()
    {
        //reload same level again
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
