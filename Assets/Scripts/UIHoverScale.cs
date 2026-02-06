using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.08f;      // ホバー時の倍率
    [SerializeField] private float duration = 0.10f;        // 変化にかける時間（秒）
    [SerializeField] private bool useUnscaledTime = true;   // Time.timeScale=0でも動かす（タイトル画面向け）

    [Header("Options")]
    [SerializeField] private bool affectThisTransformOnly = true;
    [SerializeField] private RectTransform target;          // nullなら自動で自分のRectTransform

    private RectTransform _rt;
    private Vector3 _baseScale;
    private Coroutine _co;

    private void Awake()
    {
        _rt = target != null ? target : GetComponent<RectTransform>();
        if (_rt == null)
        {
            Debug.LogError($"UIHoverScale: RectTransformが見つかりません ({name})");
            enabled = false;
            return;
        }
        _baseScale = _rt.localScale;
    }

    private void OnEnable()
    {
        // 再有効化時にスケールがズレないように
        if (_rt != null) _rt.localScale = _baseScale;
    }

    private void OnDisable()
    {
        if (_co != null) StopCoroutine(_co);
        _co = null;
        if (_rt != null) _rt.localScale = _baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(_baseScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(_baseScale);
    }

    private void StartScale(Vector3 to)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(ScaleRoutine(to));
    }

    private System.Collections.IEnumerator ScaleRoutine(Vector3 to)
    {
        Vector3 from = _rt.localScale;
        float t = 0f;

        // duration=0 対応
        if (duration <= 0f)
        {
            _rt.localScale = to;
            yield break;
        }

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);
            // なめらか補間（EaseInOutっぽく）
            a = a * a * (3f - 2f * a);

            _rt.localScale = Vector3.LerpUnclamped(from, to, a);
            yield return null;
        }

        _rt.localScale = to;
        _co = null;
    }

    /// <summary>
    /// レイアウト変更などで通常スケールを更新したいときに呼ぶ（任意）
    /// </summary>
    public void RebindBaseScale()
    {
        if (_rt == null) return;
        _baseScale = _rt.localScale;
    }
}
