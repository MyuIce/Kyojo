using UnityEngine;

[DisallowMultipleComponent]
public class OneWayGateController : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private TimerController timer;
    [SerializeField] private int activateLevel = 4;

    [Header("Gate Colliders (4つまで想定)")]
    [SerializeField] private Collider2D[] gateColliders = new Collider2D[4]; // インスペクタで4つ割り当て

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float dirEpsilon = 0.0001f; // 左移動判定の閾値

    private Transform _player;
    private Vector3 _prevPos;
    private bool _active;

    private void Awake()
    {
        if (timer == null) timer = FindFirstObjectByType<TimerController>();
        // 初期は開放（isTrigger=true）
        SetTriggers(true);
    }

    private void Start()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go != null) _player = go.transform;
        if (_player != null) _prevPos = _player.position;

        _active = (timer != null && timer.CurrentAlertLevel >= activateLevel);
        if (!_active) SetTriggers(true);
    }

    private void OnEnable()
    {
        if (timer != null) timer.OnAlertLevelChanged += OnAlertLevelChanged;
    }

    private void OnDisable()
    {
        if (timer != null) timer.OnAlertLevelChanged -= OnAlertLevelChanged;
        SetTriggers(true);
    }

    public void OnAlertLevelChanged(int prevLevel, int nextLevel)
    {
        _active = (nextLevel >= activateLevel);
        if (!_active) SetTriggers(true);
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        if (!_active)
        {
            SetTriggers(true);
            _prevPos = _player.position;
            return;
        }

        Vector3 curr = _player.position;
        bool movingLeft = (curr.x < _prevPos.x - dirEpsilon);

        // レベル4以上 かつ 左に動いている間だけ閉じる（isTrigger=false）
        SetTriggers(!movingLeft);

        _prevPos = curr;
    }

    private void SetTriggers(bool value)
    {
        if (gateColliders == null) return;
        for (int i = 0; i < gateColliders.Length; i++)
        {
            var col = gateColliders[i];
            if (col == null) continue;

            // CompositeCollider2D も考慮
            var comp = col as CompositeCollider2D;
            if (comp != null)
            {
                if (comp.isTrigger != value) comp.isTrigger = value;
            }
            else
            {
                if (col.isTrigger != value) col.isTrigger = value;
            }
        }
    }
}

  