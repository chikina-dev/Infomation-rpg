using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour {
    [TextArea]
    public string message;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        EventManager.Instance.PublishDialogue(message);
    }
}