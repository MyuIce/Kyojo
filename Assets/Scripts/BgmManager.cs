using UnityEngine;
  using UnityEngine.SceneManagement;
  using System.Collections;

  [DisallowMultipleComponent]
  public class BgmManager : MonoBehaviour
  {
      public static BgmManager Instance { get; private set; }

      [Header("Scene Names")]
      [SerializeField] private string titleSceneName = "TitleScene";
      [SerializeField] private string gameMapSceneName = "GameMap";
      [SerializeField] private string gameClearSceneName = "GameClear";
      [SerializeField] private string gameOverSceneName = "GameOver";

      [Header("Audio Source")]
      [SerializeField] private AudioSource audioSource;
      [SerializeField] private float baseVolume = 1f;

      [Header("BGM Clips")]
      [SerializeField] private AudioClip titleBgm;
      [SerializeField] private AudioClip gameMapBgmNormal;   // Lv1-3
      [SerializeField] private AudioClip gameMapBgmLevel4;   // Lv4+
      [SerializeField] private AudioClip gameClearBgm;
      [SerializeField] private AudioClip gameOverBgm;

      [Header("Fade")]
      [SerializeField] private float fadeOutDuration = 1f;

      private Coroutine _fadeCo;
      private TimerController _timer;

      private void Awake()
      {
          if (Instance != null && Instance != this)
          {
              Destroy(gameObject);
              return;
          }
          Instance = this;
          DontDestroyOnLoad(gameObject);

          if (audioSource == null)
          {
              audioSource = GetComponent<AudioSource>();
              if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
          }
          audioSource.loop = true;
          audioSource.volume = baseVolume;
      }

      private void OnEnable()
      {
          SceneManager.sceneLoaded += OnSceneLoaded;
      }

      private void OnDisable()
      {
          SceneManager.sceneLoaded -= OnSceneLoaded;
          UnsubscribeTimer();
      }

      private void Start()
      {
          ApplySceneBgm(SceneManager.GetActiveScene().name);
      }

      private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
      {
          ApplySceneBgm(scene.name);
      }

      private void ApplySceneBgm(string sceneName)
      {
          UnsubscribeTimer();

          if (sceneName == titleSceneName)
          {
              SwitchBgm(titleBgm);
          }
          else if (sceneName == gameMapSceneName)
          {
              _timer = FindFirstObjectByType<TimerController>();
              if (_timer != null)
              {
                  _timer.OnAlertLevelChanged += OnAlertLevelChanged;
                  UpdateMapBgm(_timer.CurrentAlertLevel);
              }
              else
              {
                  SwitchBgm(gameMapBgmNormal);
              }
          }
          else if (sceneName == gameClearSceneName)
          {
              SwitchBgm(gameClearBgm);
          }
          else if (sceneName == gameOverSceneName)
          {
              SwitchBgm(gameOverBgm);
          }
      }

      private void OnAlertLevelChanged(int prevLevel, int nextLevel)
      {
          UpdateMapBgm(nextLevel);
      }

      private void UpdateMapBgm(int level)
      {
          if (level >= 4)
              SwitchBgm(gameMapBgmLevel4);
          else
              SwitchBgm(gameMapBgmNormal);
      }

      private void SwitchBgm(AudioClip nextClip)
      {
          if (audioSource == null) return;

          if (audioSource.clip == nextClip)
          {
              if (!audioSource.isPlaying && nextClip != null)
              {
                  audioSource.volume = baseVolume;
                  audioSource.Play();
              }
              return;
          }

          if (_fadeCo != null) StopCoroutine(_fadeCo);
          _fadeCo = StartCoroutine(FadeOutAndSwitch(nextClip));
      }

      private IEnumerator FadeOutAndSwitch(AudioClip nextClip)
      {
          if (fadeOutDuration > 0f && audioSource.isPlaying)
          {
              float startVol = audioSource.volume;
              float t = 0f;
              while (t < fadeOutDuration)
              {
                  t += Time.unscaledDeltaTime;
                  float a = Mathf.Clamp01(t / fadeOutDuration);
                  audioSource.volume = Mathf.Lerp(startVol, 0f, a);
                  yield return null;
              }
          }

          audioSource.Stop();
          audioSource.clip = nextClip;
          audioSource.volume = baseVolume;

          if (nextClip != null)
          {
              audioSource.Play();
          }

          _fadeCo = null;
      }

      private void UnsubscribeTimer()
      {
          if (_timer != null)
          {
              _timer.OnAlertLevelChanged -= OnAlertLevelChanged;
              _timer = null;
          }
      }
  }