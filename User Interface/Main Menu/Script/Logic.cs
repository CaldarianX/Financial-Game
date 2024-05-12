using UnityEngine;
using UnityEngine.SceneManagement;

public class Logic : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
