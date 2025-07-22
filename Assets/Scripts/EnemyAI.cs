using UnityEngine;
using System.Collections;

public enum EnemyState { Patrolling, Chasing, Returning }

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("巡回設定")]
    public Transform[] patrolPoints; // 巡回する地点のリスト
    public float patrolSpeed = 2f;   // 巡回時の速度

    [Header("追跡設定")]
    public float chaseSpeed = 4f;    // 追跡時の速度
    public float detectionRadius = 5f; // プレイヤーを検知する半径
    public float loseSightRadius = 8f; // プレイヤーを見失う半径

    private Rigidbody2D rb;
    private Animator animator; // Animatorへの参照
    private Transform playerTransform;
    private Vector3 lastKnownPlayerPosition;
    private int currentPatrolIndex = 0;
    private EnemyState currentState = EnemyState.Patrolling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Animatorを取得
    }

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
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrollingState(distanceToPlayer);
                break;
            case EnemyState.Chasing:
                HandleChasingState(distanceToPlayer);
                break;
            case EnemyState.Returning:
                HandleReturningState();
                break;
        }
    }

    private void FixedUpdate()
    {
        // 状態に応じた移動処理
        switch (currentState)
        {
            case EnemyState.Patrolling:
                MoveTowards(patrolPoints[currentPatrolIndex].position, patrolSpeed);
                break;
            case EnemyState.Chasing:
                MoveTowards(playerTransform.position, chaseSpeed);
                break;
            case EnemyState.Returning:
                MoveTowards(lastKnownPlayerPosition, patrolSpeed);
                break;
        }
    }

    private void HandlePatrollingState(float distanceToPlayer)
    {
        // プレイヤーが索敵範囲に入ったら追跡モードに切り替え
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Chasing;
            Debug.Log("プレイヤー発見！追跡開始。");
        }
        // 巡回地点に近づいたら、次の巡回地点を目指す
        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void HandleChasingState(float distanceToPlayer)
    {
        // プレイヤーが索敵範囲から出たら、元の場所に戻るモードに切り替え
        if (distanceToPlayer > loseSightRadius)
        {
            lastKnownPlayerPosition = transform.position; // 最後にいた場所を記憶
            currentState = EnemyState.Returning;
            Debug.Log("プレイヤーを見失った。元の場所に戻る。");
        }
    }

    private void HandleReturningState()
    {
        // プレイヤーが再度索敵範囲に入ったら、再び追跡モードに
        if (Vector2.Distance(transform.position, playerTransform.position) <= detectionRadius)
        {
            currentState = EnemyState.Chasing;
            Debug.Log("プレイヤー再発見！追跡再開。");
            return;
        }
        // 最後にいた場所に戻ったら、巡回モードに復帰
        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < 0.5f)
        {
            currentState = EnemyState.Patrolling;
            Debug.Log("元の場所に戻った。巡回再開。");
        }
    }

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector2 direction = (target - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // Animatorが設定されていればパラメータを更新
        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
    }

    // プレイヤーに接触した時の処理
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ★ 衝突した相手の情報をログに出す
        Debug.Log($"[EnemyAI] OnCollisionEnter2D: '{collision.gameObject.name}' と衝突しました。");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに捕まった！");
            this.enabled = false; // 敵のAIを停止
            GameManager.Instance.RespawnPlayer();
        }
    }

    // Gizmoで索敵範囲を視覚的に表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseSightRadius);
    }
}