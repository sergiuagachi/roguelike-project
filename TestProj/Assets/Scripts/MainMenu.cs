using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startGameButton;
    public Button quitGameButton;
    
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        quitGameButton.onClick.AddListener(QuitGame);
    }

    void StartGame() {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }

    void QuitGame() {
        Application.Quit();
    }
}
