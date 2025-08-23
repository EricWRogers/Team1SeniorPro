using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Test Scene");
    }

    public void Options()
    {
        Debug.Log("Options menu opened");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("You've quit the game!");
    }
}
