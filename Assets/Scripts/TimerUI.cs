using UnityEngine;
using UnityEngine.UI;

// GameManagerの経過時間をUIテキストに表示する
[RequireComponent(typeof(Text))]
public class TimerUI : MonoBehaviour
{
    private Text timerText;

    private void Awake()
    {
        // Textコンポーネントをキャッシュする
        timerText = GetComponent<Text>();
    }

    private void Update()
    {
        // GameManagerから経過時間を取得
        float time = GameManager.Instance.ElapsedTime;

        // 分と秒を計算
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time * 100) % 100);

        // "00:00:00" の形式でテキストを更新
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}