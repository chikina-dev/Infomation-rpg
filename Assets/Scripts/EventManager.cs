using UnityEngine;
using System;

public class EventManager : MonoBehaviour {
    public static EventManager Instance { get; private set; }

    // ★ string から (string, Sprite) のタプルに変更
    public event Action<(string message, Sprite icon)> OnDialogue;

    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ★ 引数をタプルに変更
    public void PublishDialogue(string msg, Sprite icon = null) {
        OnDialogue?.Invoke((msg, icon));
    }
}