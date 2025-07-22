using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BootstrapManager : MonoBehaviour {
    [Tooltip("スタートボタン")] public Button startButton;
    [Tooltip("読み込むコアシーン名")] public string coreSceneName = "GameCore";

    private void Awake() {
        // ボタンが設定されていなければエラーを出す
        if (startButton == null) {
            Debug.LogError("[BootstrapManager] スタートボタンがInspectorで設定されていません！");
            return;
        }
        startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked() {
        startButton.interactable = false;
        // GameCoreシーンをロードするだけ。これ以降の処理はGameManagerが引き継ぐ。
        SceneManager.LoadScene(coreSceneName);
    }
}