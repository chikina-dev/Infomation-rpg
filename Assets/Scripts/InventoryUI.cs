using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InventoryUI : MonoBehaviour {
    public Image[] slots; // 0-4: 部品, 5: 設計図
    // public Sprite defaultIcon; // ← 不要になったので削除
    public Color collectedColor = Color.white;
    public Color notCollectedColor = new Color(1f, 1f, 1f, 0.5f);

    private ItemData[] partItems = new ItemData[5];
    private ItemData blueprintItem;

    void Start() {
        for (int i = 0; i < 5; i++) {
            string itemId = $"part{i + 1}";
            partItems[i] = Resources.Load<ItemData>($"Items/{itemId}");
            if (partItems[i] == null) {
                Debug.LogError($"InventoryUI: ItemData '{itemId}' が見つかりません！");
            }
        }
        blueprintItem = Resources.Load<ItemData>("Items/blueprint");
        if (blueprintItem == null) {
            Debug.LogError("InventoryUI: ItemData 'blueprint' が見つかりません！");
        }
        UpdateUI();
    }

    void Update() {
        UpdateUI();
    }

    void UpdateUI() {
        for (int i = 0; i < 5; i++) {
            UpdateSlot(slots[i], partItems[i]);
        }
        UpdateSlot(slots[5], blueprintItem);
    }

    void UpdateSlot(Image slot, ItemData item) {
        if (slot == null || item == null) return;

        if (GameManager.Instance.HasItem(item.itemId)) {
            slot.sprite = item.icon;
            slot.color = collectedColor;
        } else {
            slot.sprite = item.icon; // 未取得でもアイテム固有のアイコンを使用
            slot.color = notCollectedColor;
        }
    }
}
