using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void TransitionTo(string fromScene, string toScene)
    {
        SceneManager.UnloadSceneAsync(fromScene);
        SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
    }
}
