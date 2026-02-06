using UnityEngine;
  using UnityEngine.UI;
  using TMPro;
  using System.Collections;

  [DisallowMultipleComponent]
  public class AlertLevelEffects : MonoBehaviour
  {
      [Header("Sources")]
      [SerializeField] private TimerController timer;

      [Header("Alert Text Color")]
      [SerializeField] private TextMeshProUGUI alertLevelText;
      [SerializeField] private Color level1Color = Color.white;
      [SerializeField] private Color level2Color = new Color(1f, 0.85f, 0.2f);   // Yellow-ish
      [SerializeField] private Color level3Color = new Color(1f, 0.55f, 0.1f);  // Orange
      [SerializeField] private Color level4Color = new Color(1f, 0.2f, 0.2f);   // Red

      [Header("Vignette Overlay (UI Image)")]
      [SerializeField] private Image vignetteImage;           // 非Activeで開始OK
      [SerializeField] private Color level3VignetteColor = new Color(0f, 0f, 0f, 0.25f); // 黒っぽい弱ビネット
      [SerializeField] private Color level4VignetteColor = new Color(1f, 0f, 0f, 0.35f); // 赤ビネット

      [Header("Level 5 Flash Overlay")]
      [SerializeField] private Image flashOverlayImage;       // 非Activeで開始OK
      [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.45f); // 赤みの強い点滅
      [SerializeField] private float flashFrequencyHz = 2.0f; // 1秒あたり点滅回数
      [SerializeField] private float flashMinAlpha = 0.0f;
      [SerializeField] private float flashMaxAlpha = 0.6f;
      [SerializeField] private bool useUnscaledTime = true;

      private Coroutine _flashCo;

      private void Awake()
      {
          if (timer == null) timer = FindFirstObjectByType<TimerController>();
      }

      private void OnEnable()
      {
          if (timer != null)
          {
              timer.OnAlertLevelChanged += OnAlertLevelChanged;
              ApplyLevel(timer.CurrentAlertLevel);
          }
          else
          {
              ApplyLevel(1);
          }
      }

      private void OnDisable()
      {
          if (timer != null) timer.OnAlertLevelChanged -= OnAlertLevelChanged;
          StopFlash();
          // 念のため無効化
          if (vignetteImage != null) vignetteImage.gameObject.SetActive(false);
          if (flashOverlayImage != null) flashOverlayImage.gameObject.SetActive(false);
      }

      public void OnAlertLevelChanged(int prevLevel, int nextLevel)
      {
          ApplyLevel(nextLevel);
      }

      private void ApplyLevel(int level)
      {
          // Text color
          if (alertLevelText != null)
          {
              switch (level)
              {
                  case 1: alertLevelText.color = level1Color; break;
                  case 2: alertLevelText.color = level2Color; break;
                  case 3: alertLevelText.color = level3Color; break;
                  case 4: alertLevelText.color = level4Color; break;
                  case 5: alertLevelText.color = level4Color; break; // Lv5も赤系維持
                  default: alertLevelText.color = level1Color; break;
              }
          }

          // Vignette active/disable + color
          if (vignetteImage != null)
          {
              if (level <= 2)
              {
                  if (vignetteImage.gameObject.activeSelf)
                      vignetteImage.gameObject.SetActive(false);
              }
              else if (level == 3)
              {
                  if (!vignetteImage.gameObject.activeSelf)
                      vignetteImage.gameObject.SetActive(true);
                  vignetteImage.color = level3VignetteColor;
              }
              else // 4 or 5
              {
                  if (!vignetteImage.gameObject.activeSelf)
                      vignetteImage.gameObject.SetActive(true);
                  vignetteImage.color = level4VignetteColor;
              }
          }

          // 変更点：Lv4から点滅開始
          if (level >= 4)
          {
              if (flashOverlayImage != null && !flashOverlayImage.gameObject.activeSelf)
                  flashOverlayImage.gameObject.SetActive(true);
              StartFlash();
          }
          else
          {
              StopFlash(); // 内部で非Activeに戻す
          }
      }

      private void StartFlash()
      {
          if (flashOverlayImage == null) return;

          if (_flashCo != null) StopCoroutine(_flashCo);

          // 初期アルファを0にしてから開始
          var c0 = flashOverlayImage.color;
          flashOverlayImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

          _flashCo = StartCoroutine(FlashRoutine());
      }

      private void StopFlash()
      {
          if (_flashCo != null)
          {
              StopCoroutine(_flashCo);
              _flashCo = null;
          }
          if (flashOverlayImage != null)
          {
              var c = flashOverlayImage.color;
              c.a = 0f;
              flashOverlayImage.color = c;
              if (flashOverlayImage.gameObject.activeSelf)
                  flashOverlayImage.gameObject.SetActive(false);
          }
      }

      private IEnumerator FlashRoutine()
      {
          while (true)
          {
              float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
              float t = (useUnscaledTime ? Time.unscaledTime : Time.time) * flashFrequencyHz * Mathf.PI * 2f;

              float s = (Mathf.Sin(t) + 1f) * 0.5f; // 0..1
              float a = Mathf.Lerp(flashMinAlpha, flashMaxAlpha, s);

              var c = new Color(flashColor.r, flashColor.g, flashColor.b, a);
              flashOverlayImage.color = c;

              yield return null;
          }
      }
  }