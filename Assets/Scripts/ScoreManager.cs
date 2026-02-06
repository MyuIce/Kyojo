using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int TotalScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        TotalScore += amount;
        Debug.Log($"Score Added: {amount}. Total Score: {TotalScore} / 100");
    }

    public void ResetScore()
    {
        TotalScore = 0;
    }
}
