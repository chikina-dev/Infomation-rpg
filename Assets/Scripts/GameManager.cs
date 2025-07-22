using UnityEngine;
using System.Collections; // コルーチンを使うために必要
using System.Collections.Generic;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要

public enum GameState {
    Boot,
    Playing,
    Paused,
    Cleared
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private GameState _state = GameState.Boot;
    public GameState State {
        get { return _state; }
        private set {
            if (_state != value) {
                Debug.Log($"[GameManager] Stateが {_state} から {value} に変更されました。");
                _state = value;
            }
        }
    }

    private float elapsedTime = 0f;
    public float ElapsedTime => elapsedTime;

    private HashSet<string> inventory = new(); // 全アイテム
    private HashSet<string> collectedParts = new(); // 部品専用
    private HashSet<string> unlockedBlueprints = new(); // 設計図専用

    [Header("ゲーム設定")]
    [Tooltip("最初にロードするゲームプレイシーン")]
    public string initialGameSceneName = "1F_Hallway";

    public IReadOnlyCollection<string> Inventory => inventory;
    public int CollectedPartCount => collectedParts.Count;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ★ イベント購読の開始
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ★ イベント購読の解除
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ★ シーンがロードされた時に呼ばれる
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Bootstrapシーンに戻ってきたら、次のゲームのために全てをリセットする
        if (scene.name == "Bootstrap")
        {
            Debug.Log("タイトルに戻ったため、永続オブジェクトを破棄します。");
            
            // 他の永続オブジェクトも破棄する
            if (EventManager.Instance != null)
            {
                Destroy(EventManager.Instance.gameObject);
            }
            
            // 最後に自分自身を破棄する
            Destroy(gameObject);
        }
    }

    private void Start() {
        // 自分がロードされたら、初期ゲームシーンのロードを開始する
        StartCoroutine(LoadInitialScene());
    }

    private IEnumerator LoadInitialScene()
    {
        // まずGameManagerの各種リセット処理を実行
        StartGame();

        // 次に初期シーンを追加でロード
        yield return SceneManager.LoadSceneAsync(initialGameSceneName, LoadSceneMode.Additive);

        // これでゲームの準備が完全に整った
        Debug.Log("ゲームの準備が完了しました。タイマーが開始します。");
    }

    private void Update() {
        if (State == GameState.Playing) {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        Debug.Log("[GameManager] StartGame() が呼び出されました。");
        State = GameState.Playing;
        elapsedTime = 0f;
        inventory.Clear();
        collectedParts.Clear();
        unlockedBlueprints.Clear();
        
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueUI?.HideImmediate();
    }

    public void ClearGame() {
        State = GameState.Cleared;
    }
// ... (以降のメソッドは変更なし)

    public void AddItem(string itemId) {
        if (!inventory.Add(itemId)) return;

        // ★デバッグログ変更
        Debug.Log($"[GameManager] AddItem: \"{itemId}\" をインベントリに追加しました。現在のインベントリ内容: [{string.Join(", ", inventory)}]");

        var item = Resources.Load<ItemData>($"Items/{itemId}");
        if (item == null) {
            Debug.LogWarning($"ItemData が見つかりません: {itemId}");
            return;
        }

        switch (item.itemType) {
            case ItemType.Part:
                AddPart(itemId);
                break;
            case ItemType.Blueprint:
                UnlockBlueprint(itemId);
                break;
            default:
                break;
        }
    }

    private void AddPart(string partId) {
        if (!collectedParts.Add(partId)) return;

        Debug.Log($"部品取得: {partId}");

        if (collectedParts.Count >= 2) {
            Debug.Log("必要な部品を集めた！");
            // TODO: イベントやシーケンス処理をここに
        }
    }

    public void UnlockBlueprint(string blueprintId) {
        if (unlockedBlueprints.Add(blueprintId)) {
            Debug.Log($"設計図アンロック: {blueprintId}");
        }
    }

    public void MarkPartFound(string partId) {
        AddPart(partId);
    }

    public bool HasItem(string itemId) => inventory.Contains(itemId);
    public bool HasPart(string partId) => collectedParts.Contains(partId);

    public bool HasAllParts()
    {
        return collectedParts.Count >= 5;
    }

    public bool HasBlueprint()
    {
        return unlockedBlueprints.Count > 0;
    }

    // ★ プレイヤーをリスポーンさせるメソッド
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        Debug.Log("プレイヤーが捕まりました。1Fの廊下に戻ります。");

        // （任意）プレイヤーの操作を一時的に不能にする
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }

        // 現在のシーン（OnDemandSalon）を取得
        Scene currentScene = SceneManager.GetActiveScene();
        // OnDemandSalonが複数シーンある場合の対応
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "OnDemandSalon") {
                currentScene = scene;
                break;
            }
        }


        // 廊下シーンを追加でロー���
        yield return SceneManager.LoadSceneAsync("1F_Hallway", LoadSceneMode.Additive);

        // プレイヤーを廊下の初期位置に移動させる
        if (player != null)
        {
            GameObject startPoint = GameObject.Find("Player_Start"); // 廊下の初期位置オブジェクト
            if (startPoint != null)
            {
                player.transform.position = new Vector3(startPoint.transform.position.x, startPoint.transform.position.y, -1f);
            }
            else
            {
                Debug.LogWarning("1F_Hallwayに 'Player_Start' オブジェクトが見つかりません！");
            }
        }

        // OnDemandSalonシーンをアンロード
        if (currentScene.IsValid() && currentScene.name != "1F_Hallway")
        {
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        // プレイヤーの操作を再度有効にする
        if (player != null)
        {
            player.enabled = true;
        }
    }
}
