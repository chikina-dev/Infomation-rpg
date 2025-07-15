public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string[] initialScenes = { "GameCore", "1F_Hallway" };
    // Start is called before the first frame update
    void Start()
    {
        foreach (var scene in initialScenes)
        {
            if (!SceneManager.GetSceneByName(scene).isLoaded)
            {
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }
    }
}
