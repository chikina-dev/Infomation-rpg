using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使うために必要
using System.Collections.Generic;
using System.Linq;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("ランダム配置するアイテム")]
    public GameObject[] randomItemPrefabs;

    [Header("部屋を識別するタグ")]
    public string roomTag = "RandomSpawnRoom";

    [Header("対象シーン")]
    [Tooltip("このリストにあるシーンがロードされた時にスポーン処理を試みます")]
    public List<string> targetSceneNames = new List<string> { "1F_Hallway" };

    private bool hasSpawned = false;

    // スクリプトが有効になった時に呼ばれる
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // スクリプトが無効になった時に呼ばれる
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンがロードされるたびに呼び出されるメソッド
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // すでに配置済みか、ロードされたシーンが対象リストに含まれて���なければ何もしない
        if (hasSpawned || !targetSceneNames.Contains(scene.name))
        {
            return;
        }

        // 対象のシーンがロードされたので、アイテムを配置する
        SpawnItems();
    }

    private void SpawnItems()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag(roomTag);

        if (rooms.Length < randomItemPrefabs.Length)
        {
            Debug.LogWarning("ランダムアイテムを配置する部屋の数が足りません。次のシーンロードで再試行します。");
            return;
        }

        List<GameObject> shuffledRooms = rooms.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < randomItemPrefabs.Length; i++)
        {
            GameObject targetRoom = shuffledRooms[i];
            RoomItemSpawnPoint spawnPoint = targetRoom.GetComponent<RoomItemSpawnPoint>();

            if (spawnPoint != null && spawnPoint.spawnPoints.Length > 0)
            {
                Transform itemSpawnTransform = spawnPoint.spawnPoints[Random.Range(0, spawnPoint.spawnPoints.Length)];
                Vector3 basePosition = itemSpawnTransform.position;
                Vector3 finalPosition = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, -1f);
                Instantiate(randomItemPrefabs[i], finalPosition, Quaternion.identity, targetRoom.transform);
            }
            else
            {
                Debug.LogWarning($"部屋 {targetRoom.name} に有効なRoomItemSpawnPointがありません。");
            }
        }

        // 処理が完了したのでフラグを立て、イベント購読も解除する
        hasSpawned = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("ランダムアイテムの配置が完了しました。");
    }
}
