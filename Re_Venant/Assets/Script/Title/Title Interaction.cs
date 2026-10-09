
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("GameSence");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}