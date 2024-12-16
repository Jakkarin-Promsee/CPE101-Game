using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    public GameObject deadCanvas;
    // Call this method to reload the current scene
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // Get the current scene's name
        SceneManager.LoadScene(currentSceneName); // Reload the scene

        deadCanvas.SetActive(false);
    }
}
