using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    [Header("UIパーツ")]
    public Text resultTimeText;
    public Button backToTitleButton;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            float time = GameManager.Instance.ElapsedTime;
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int milliseconds = (int)((time * 100) % 100);
            resultTimeText.text = "TIME: " + string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }

        if (backToTitleButton != null)
        {
            backToTitleButton.onClick.AddListener(OnBackToTitleClicked);
        }

        Time.timeScale = 0f;
    }

    void OnBackToTitleClicked()
    {
        Time.timeScale = 1f;
        
        SceneManager.LoadScene("Bootstrap");
    }
}