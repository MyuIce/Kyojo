using UnityEngine;
  using UnityEngine.Events;

  [DisallowMultipleComponent]
  public class TimerController : MonoBehaviour
  {
      [System.Serializable]
      public class AlertLevelChangedEvent : UnityEvent<int, int> { }

      [SerializeField] private float totalDuration = 300f;
      [SerializeField] private bool autoStart = true;
      [SerializeField] private float[] levelStartTimes = new float[] { 0f, 60f, 120f, 210f, 270f };
      [SerializeField] private AlertLevelChangedEvent onAlertLevelChanged;

      public float TotalDuration => totalDuration;
      public float ElapsedSeconds { get; private set; }
      public float RemainingSeconds => Mathf.Max(0f, totalDuration - ElapsedSeconds);
      public int CurrentAlertLevel { get; private set; } = 1;
      public int LevelCount => levelStartTimes != null ? levelStartTimes.Length : 0;
      public bool IsRunning => _isRunning;

      public event System.Action<int, int> OnAlertLevelChanged;

      private bool _isRunning;

      private void Awake()
      {
          EnsureValidConfig();
          CurrentAlertLevel = ComputeLevel(ElapsedSeconds);
      }

      private void Start()
      {
          if (autoStart) _isRunning = true;
      }

      private void Update()
      {
          if (!_isRunning) return;

          ElapsedSeconds += Time.deltaTime;
          if (ElapsedSeconds >= totalDuration)
          {
              ElapsedSeconds = totalDuration;
              UpdateLevelIfNeeded();
              _isRunning = false;
              if (GameManager.Instance != null) GameManager.Instance.NotifyTimeUp();
              return;
          }

          UpdateLevelIfNeeded();
      }

      public void StartTimer() => _isRunning = true;

      public void StopTimer() => _isRunning = false;

      public void ResetTimer(float startElapsed = 0f)
      {
          ElapsedSeconds = Mathf.Clamp(startElapsed, 0f, totalDuration);
          _isRunning = false;
          CurrentAlertLevel = ComputeLevel(ElapsedSeconds);
      }

      private void UpdateLevelIfNeeded()
      {
          int next = ComputeLevel(ElapsedSeconds);
          if (next == CurrentAlertLevel) return;

          int prev = CurrentAlertLevel;
          CurrentAlertLevel = next;
          OnAlertLevelChanged?.Invoke(prev, next);
          onAlertLevelChanged?.Invoke(prev, next);
      }

      private int ComputeLevel(float elapsed)
      {
          if (levelStartTimes == null || levelStartTimes.Length == 0) return 1;

          int level = 1;
          for (int i = 0; i < levelStartTimes.Length; i++)
          {
              if (elapsed >= levelStartTimes[i]) level = i + 1;
              else break;
          }
          return level;
      }

      private void EnsureValidConfig()
      {
          if (levelStartTimes == null || levelStartTimes.Length == 0)
          {
              levelStartTimes = new float[] { 0f, 60f, 120f, 210f, 270f };
          }
          System.Array.Sort(levelStartTimes);
          if (levelStartTimes[0] > 0f) levelStartTimes[0] = 0f;
      }

  #if UNITY_EDITOR
      private void OnValidate()
      {
          EnsureValidConfig();
          totalDuration = Mathf.Max(1f, totalDuration);
          CurrentAlertLevel = ComputeLevel(Mathf.Clamp(ElapsedSeconds, 0f, totalDuration));
      }
  #endif
  }