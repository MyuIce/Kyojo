using UnityEngine;
using TMPro;

public class ResultScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string prefix = "合計スコア";

    void Start()
    {
        if (scoreText != null && ScoreManager.Instance != null)
        {
            scoreText.text = $"{prefix} {ScoreManager.Instance.TotalScore} / 100";
        }
    }
}
