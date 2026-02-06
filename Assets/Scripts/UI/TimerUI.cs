using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class TimerUI : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private TimerController timer;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI alertLevelText;

    private int _lastShownSeconds = -1;
    private int _lastShownLevel = -1;

    private void Awake()
    {
        if (timer == null)
        {
            timer = FindFirstObjectByType<TimerController>();
        }
    }

    private void OnEnable()
    {
        if (timer != null)
        {
            timer.OnAlertLevelChanged += OnAlertLevelChanged;
        }

        // 初期表示
        ForceRefresh();
    }

    private void OnDisable()
    {
        if (timer != null)
        {
            timer.OnAlertLevelChanged -= OnAlertLevelChanged;
        }
    }

    private void Update()
    {
        if (timer == null) return;

        int seconds = Mathf.Max(0, Mathf.FloorToInt(timer.RemainingSeconds));
        if (seconds != _lastShownSeconds)
        {
            _lastShownSeconds = seconds;
            if (timeText != null)
            {
                timeText.text = FormatTime(seconds);
            }
        }

        int level = timer.CurrentAlertLevel;
        if (level != _lastShownLevel)
        {
            _lastShownLevel = level;
            if (alertLevelText != null)
            {
                alertLevelText.text = $"Lv.{level}";
            }
        }
    }

    public void OnAlertLevelChanged(int prevLevel, int nextLevel)
    {
        _lastShownLevel = -1; // 次フレームで必ず更新
        if (alertLevelText != null)
        {
            alertLevelText.text = $"Lv.{nextLevel}";
        }
    }

    private void ForceRefresh()
    {
        if (timer == null) return;

        _lastShownSeconds = -1;
        _lastShownLevel = -1;

        // 即時反映
        if (timeText != null)
        {
            int seconds = Mathf.Max(0, Mathf.FloorToInt(timer.RemainingSeconds));
            timeText.text = FormatTime(seconds);
            _lastShownSeconds = seconds;
        }
        if (alertLevelText != null)
        {
            alertLevelText.text = $"Lv.{timer.CurrentAlertLevel}";
            _lastShownLevel = timer.CurrentAlertLevel;
        }
    }

    private static string FormatTime(int totalSeconds)
    {
        int m = totalSeconds / 60;
        int s = totalSeconds % 60;
        return $"{m:00}:{s:00}";
    }

}