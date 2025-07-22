using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SceneTrigger : MonoBehaviour {
    [Header("遷移設定")]
    [Tooltip("遷移先のシーン名")]
    public string targetScene;
    [Tooltip("（任意）この名前のオブジェクトの位置にプレイヤーを移動させる")]
    public string destinationObjectName; // ★ 出現地点のオブジェクト名を指定するフィールド

    [Header("条件")]
    [Tooltip("（任意）このアイテムIDを持っていないと遷移できない")]
    public string requiredItemId;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (isTransitioning || !other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(requiredItemId) && !GameManager.Instance.HasItem(requiredItemId)) {
            EventManager.Instance.PublishDialogue("鍵がかかっているようだ…", null);
            return;
        }

        StartCoroutine(TransitionToScene());
    }

    private IEnumerator TransitionToScene()
    {
        isTransitioning = true;
        Scene currentScene = gameObject.scene;

        // 次のシーンを追加ロード
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        // ★ プレイヤーを新しいシーンの指定位置へ移動させる
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && !string.IsNullOrEmpty(destinationObjectName))
        {
            GameObject destination = GameObject.Find(destinationObjectName);
            if (destination != null)
            {
                // ★ Z座標を-1に設定するロジックに変更
                Vector3 destPos = destination.transform.position;
                player.transform.position = new Vector3(destPos.x, destPos.y, -1f);
                
                Debug.Log($"プレイヤーを '{destination.name}' の位置(Z=-1)に移動しました。");
            }
            else
            {
                Debug.LogWarning($"出現地点 '{destinationObjectName}' がシーン '{targetScene}' に見つかりません！");
            }
        }

        // 現在のシーンをアン���ード
        yield return SceneManager.UnloadSceneAsync(currentScene);

        isTransitioning = false;
    }
}