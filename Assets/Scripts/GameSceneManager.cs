using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class GameSceneLoader : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string sceneName;                 // インスペクタで指定

    [Header("Options")]
    [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
    [SerializeField] private float delaySeconds = 0f;         // 0なら即時
    [SerializeField] private bool useUnscaledTime = true;     // ポーズ中でも待機
    [SerializeField] private bool resetTimeScaleOnLoad = true;

    // インスペクタで指定した sceneName を使用
    public void Load()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameSceneLoader: sceneName が未設定です。", this);
            return;
        }
        StartCoroutine(LoadRoutine(sceneName));
    }

    // 文字列引数で指定して呼ぶことも可能
    public void LoadByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("GameSceneLoader: 引数 name が空です。", this);
            return;
        }
        StartCoroutine(LoadRoutine(name));
    }

    // 現在のシーンを再読み込み
    public void ReloadCurrent()
    {
        StartCoroutine(LoadRoutine(SceneManager.GetActiveScene().name));
    }

    // アプリ終了（エディタでは再生停止）
    public void QuitGame()
    {
        if (delaySeconds > 0f)
        {
            StartCoroutine(QuitRoutine());
        }
        else
        {
            DoQuit();
        }
    }

    private IEnumerator LoadRoutine(string name)
    {
        if (delaySeconds > 0f)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(delaySeconds);
            else yield return new WaitForSeconds(delaySeconds);
        }

        if (resetTimeScaleOnLoad) Time.timeScale = 1f;
        SceneManager.LoadScene(name, loadMode);
    }

    private IEnumerator QuitRoutine()
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(delaySeconds);
        else yield return new WaitForSeconds(delaySeconds);
        DoQuit();
    }

    private void DoQuit()
    {
        if (resetTimeScaleOnLoad) Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}