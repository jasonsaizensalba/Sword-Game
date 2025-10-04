using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject helpUI;
    public bool isHelping = false;

    public void mainMenu()
    {
        SceneManager.LoadScene("Battle");
    }

    public void quit()
    {
        Application.Quit();
    }

    public void help()
    {
        helpUI.SetActive(true);      
    }

    public void closeHelp()
    {
        helpUI.SetActive(false);
    }
}
