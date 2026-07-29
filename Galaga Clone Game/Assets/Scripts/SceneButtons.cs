using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    public void StartGame()
    {
  
        PlayerScore.finalScore = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void RestartGame()
    {
    
        PlayerScore.finalScore = 0;
        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMenu()
    {

        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {

        Application.Quit();
    }
}