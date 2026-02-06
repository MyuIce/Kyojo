using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Clear,
        GameOver
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private bool autoFindPlayerByTag = true;
    [SerializeField] private string playerTag = "Player";

    [SerializeField] private UnityEvent onClear;
    [SerializeField] private UnityEvent onGameOver;

    public GameState State { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        if (playerCollider == null && autoFindPlayerByTag)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null)
            {
                playerCollider = go.GetComponent<Collider2D>();
            }
        }
    }

    public void NotifyTimeUp()
    {
        if (State != GameState.Playing) return;
        bool inSafe = IsPlayerInSafeZone();
        if (inSafe) SetState(GameState.Clear);
        else SetState(GameState.GameOver);
    }

    public void ForceClear()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Clear);
    }

    public void ForceGameOver()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.GameOver);
    }

    private void SetState(GameState newState)
    {
        if (State == newState) return;
        State = newState;

        if (State == GameState.Clear)
        {
            onClear?.Invoke();
        }
        else if (State == GameState.GameOver)
        {
            onGameOver?.Invoke();
        }
    }

    private bool IsPlayerInSafeZone()
    {
        if (playerCollider != null)
        {
            var results = new List<Collider2D>(8);
            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true
            };
            filter.SetLayerMask(Physics2D.AllLayers);

            int count = playerCollider.Overlap(filter, results);
            for (int i = 0; i < count; i++)
            {
                if (results[i] != null && results[i].CompareTag("SafeZone"))
                    return true;
            }

            Vector2 center = playerCollider.bounds.center;
            var hits = Physics2D.OverlapPointAll(center);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null && hits[i].CompareTag("SafeZone"))
                    return true;
            }
            return false;
        }
        else
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player == null) return false;

            Vector2 pos = player.transform.position;
            var hits = Physics2D.OverlapPointAll(pos);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != null && hits[i].CompareTag("SafeZone"))
                    return true;
            }
            return false;
        }
    }

}