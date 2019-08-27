using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    private const float StartDelay = 3;
    private const float ChangeTextDelay = 2;
    public Text visibleText;

    private const string Text1 = "You go inside the tower, but the door closes behind";
    private const string Text2 = "You see a sign and decide to check what it says...";

    private void Start()
    {
        StartCoroutine(ChangeTextAfterDelay(ChangeTextDelay));
    }

    private IEnumerator ChangeTextAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        visibleText.text = Text1;
        
        yield return new WaitForSeconds(delay);
        visibleText.text = Text2;

        StartCoroutine(LoadLevelAfterDelay(StartDelay));
    }

    private static IEnumerator LoadLevelAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Floor1", LoadSceneMode.Single);
    }
}
