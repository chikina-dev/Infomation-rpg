using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("プレイヤーがこの距離以内に入るとインタラクト可能になる")]
    public float interactionRadius = 2f;
    [Tooltip("ゲームクリア時に遷移するシーン名")]
    public string gameEndSceneName = "GameEnd";

    [Header("メッセージ")]
    [Tooltip("条件未達の場合に表示するメッセージ")]
    public string insufficientPartsMessage = "部品が足りていないようだ…";
    [Tooltip("（任意）インタラクト可能な時に表示するメッセージ")]
    public string interactionPrompt = "SP5を起動しますか？ (E)";

    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private bool hasBeenTriggered = false;

    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (hasBeenTriggered || playerTransform == null) return;

        // プレイヤーとの距離を計算
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= interactionRadius)
        {
            // プレイヤーが範囲内に入った
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                EventManager.Instance.PublishDialogue(interactionPrompt, null);
            }

            // Eキーが押されたらチェックを実行
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckCompletion();
            }
        }
        else
        {
            // プレイヤーが範囲外に出た
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
            }
        }
    }

    private void CheckCompletion()
    {
        hasBeenTriggered = true;

        // GameManagerで部品と設計図が揃っているかチェック
        if (GameManager.Instance.HasAllParts() && GameManager.Instance.HasBlueprint())
        {
            // 条件達成：ゲームクリア処理
            Debug.Log("全ての部品が揃っている！ゲームクリア！");
            GameManager.Instance.ClearGame();
            SceneManager.LoadScene(gameEndSceneName, LoadSceneMode.Additive);
        }
        else
        {
            // 条件未達：メッセージ表示
            Debug.Log("部品が足りていません。");
            EventManager.Instance.PublishDialogue(insufficientPartsMessage, null);
            hasBeenTriggered = false;
        }
    }

    // Gizmoでインタラクト範囲を視覚的に表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
