using UnityEngine;

// 指定した「場所」に、指定した「アイテム」を配置するだけのシンプルなコンポーネント
public class FixedItemSpawner : MonoBehaviour
{
    [Header("配置するアイテム")]
    [Tooltip("ここにPart1などのアイテムPrefabを設定")]
    public GameObject itemPrefab;

    [Header("配置先の情報")]
    [Tooltip("このTransformの位置にアイテムを配置する")]
    public Transform spawnPoint;

    void Start()
    {
        // 設定が正しいかチェック
        if (itemPrefab == null)
        {
            Debug.LogError("配置するアイテム(itemPrefab)が設定されていません！", this.gameObject);
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("配置先の場所(spawnPoint)が設定されていません！", this.gameObject);
            return;
        }

        // 指定された座標を元に、最終的な出現位置を計算
        Vector3 basePosition = spawnPoint.position;
        Vector3 finalPosition = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, -1f);

        // アイテムを生成し、このオブジェクト（部屋）の子にする
        Instantiate(itemPrefab, finalPosition, Quaternion.identity, this.transform);
    }
}