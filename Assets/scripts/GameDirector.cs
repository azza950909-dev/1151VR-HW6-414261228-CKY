using UnityEngine;
using TMPro; // 若你用傳統 UI Text，可改用 using UnityEngine.UI; 並把 TMP_Text 改成 Text

public class GameDirector : MonoBehaviour
{
    [Header("時間與分數設定")]
    public float totalTime = 90f;
    public int appleScore = 30;
    public int stage1BombPenalty = 20;
    public int stage2BombPenalty = 50;

    [Header("UI 元件")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text statusText; // 選填：可放「時間到 / 遊戲結束」提示文字

    [Header("結束時是否停止場上掉落物移動")]
    public bool clearItemsOnGameOver = false;

    public float RemainingTime { get; private set; }
    public int Score { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Start()
    {
        RemainingTime = totalTime;
        Score = 0;
        IsGameOver = false;

        if (statusText != null)
            statusText.text = "";

        UpdateUI();
    }

    private void Update()
    {
        if (IsGameOver)
            return;

        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            GameOver();
        }

        UpdateUI();
    }

    public void OnCatchApple()
    {
        if (IsGameOver)
            return;

        Score += appleScore;
        UpdateUI();
    }

    public void OnCatchBomb()
    {
        if (IsGameOver)
            return;

        if (RemainingTime > 70f)
        {
            // 第一階段：扣 20 分
            Score -= stage1BombPenalty;
        }
        else if (RemainingTime > 20f)
        {
            // 第二階段：扣 50 分
            Score -= stage2BombPenalty;
        }
        else
        {
            // 第三階段：分數砍半（若為負數則往 0 靠近，或視需求保留）
            Score = Score / 2;
        }

        UpdateUI();
    }

    private void GameOver()
    {
        IsGameOver = true;
        UpdateUI();

        if (statusText != null)
            statusText.text = $"Game Over! Final Score: {Score}";

        if (clearItemsOnGameOver)
        {
            FallingItem[] items = FindObjectsOfType<FallingItem>();
            foreach (FallingItem item in items)
            {
                Destroy(item.gameObject);
            }
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {Score}";

        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(RemainingTime)}s";
    }
}