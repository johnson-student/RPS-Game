using UnityEngine;
using UnityEngine.SceneManagement;

// Attached to the "MenuManager" object in MenuScene.
// The Play and Quit buttons call these public functions (On Click).
public class MenuController : MonoBehaviour
{
    // Load the game scene. The name must match the scene file name.
    public void OnPlayClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Close the game (does nothing inside the Unity Editor, only in a build)
    public void OnQuitClick()
    {
        Debug.Log("Quit clicked");
        Application.Quit();
    }
}
