using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour {
    [Header("UIパーツ")]
    public GameObject dialoguePanel; // 背景パネル
    public Text dialogueText;        // テキスト
    public Image dialogueIcon;       // ★アイコン表示用のImage

    [Header("設定")]
    public float displayDuration = 3f;

    private Coroutine hideRoutine;

    private void Awake() {
        if (dialoguePanel == null || dialogueText == null || dialogueIcon == null) {
            Debug.LogError("DialogueUI: UIパーツがInspectorで全て設定されていません！");
            return;
        }
        // ★ イベントの型を (string, Sprite) に合わせる
        EventManager.Instance.OnDialogue += ShowDialogue;
        dialoguePanel.SetActive(false);
    }

    private void OnDestroy() {
        if (EventManager.Instance != null)
            EventManager.Instance.OnDialogue -= ShowDialogue;
    }

    // ★ 引数をタプルに変更
    private void ShowDialogue((string message, Sprite icon) data) {
        if (hideRoutine != null) {
            StopCoroutine(hideRoutine);
        }

        dialogueText.text = data.message;

        // アイコンが渡されたかで表示を切り替える
        if (data.icon != null) {
            dialogueIcon.sprite = data.icon;
            dialogueIcon.gameObject.SetActive(true);
        } else {
            dialogueIcon.gameObject.SetActive(false);
        }

        dialoguePanel.SetActive(true);
        hideRoutine = StartCoroutine(HideAfter());
    }

    private IEnumerator HideAfter() {
        yield return new WaitForSecondsRealtime(displayDuration);
        dialoguePanel.SetActive(false);
    }

    // ゲーム開始時に呼ばれるリセット用メソッド
    public void HideImmediate() {
        if (hideRoutine != null) {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }
        if (dialoguePanel != null) {
            dialoguePanel.SetActive(false);
        }
    }
}