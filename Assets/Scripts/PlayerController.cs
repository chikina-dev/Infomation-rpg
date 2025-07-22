using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastMovement; // 最後の移動方向を保存

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        // 入力取得: 矢印キー + WASD
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // アニメーションパラメータ更新
        bool isMoving = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);
        
        if (isMoving) {
            // 移動中は現在の方向を使用
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            lastMovement = movement;
        } else if (lastMovement != Vector2.zero) {
            // 停止時は最後の移動方向を保持
            animator.SetFloat("MoveX", lastMovement.x);
            animator.SetFloat("MoveY", lastMovement.y);
        }
    }

    void FixedUpdate() {
        // 移動処理（正規化で対角線移動の速度を統一）
        if (movement.sqrMagnitude > 0.01f) {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}