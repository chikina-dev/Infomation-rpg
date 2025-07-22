using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour {
    public ItemData data;

    [Tooltip("取得時メッセージの書式。{0}にアイテムの表示名が入る。")]
    [TextArea]
    public string pickupMessageFormat = "{0} を手に入れた！";

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;
        if (data == null) {
            Debug.LogWarning("ItemPickup: ItemData が設定されていません");
            return;
        }

        // 1. メッセージを生成してDialogueシステムに表示を依���する
        if (EventManager.Instance != null && !string.IsNullOrEmpty(data.displayName))
        {
            string message = string.Format(pickupMessageFormat, data.displayName);
            // ★ 第2引数に data.icon を渡す
            EventManager.Instance.PublishDialogue(message, data.icon);
        }

        // 2. GameManagerにアイテムを追加する
        GameManager.Instance.AddItem(data.itemId);

        // 3. アイテムのゲームオブジェクトを破棄する
        Destroy(gameObject);
    }
}
