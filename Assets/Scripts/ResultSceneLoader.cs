using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DisallowMultipleComponent]
public class ResultSceneLoader : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string clearSceneName;
    [SerializeField] private string gameOverSceneName;

    [Header("Options")]
    [SerializeField] private float delaySeconds = 0f;       // 遷移前の待機（不要なら0）
    [SerializeField] private bool useUnscaledTime = true;   // ポーズ中でも待機したい場合はtrue
    [SerializeField] private bool resetTimeScaleOnLoad = true;

    public void OnClear()
    {
        if (!string.IsNullOrEmpty(clearSceneName))
            StartCoroutine(LoadRoutine(clearSceneName));
        else
            Debug.LogWarning("ResultSceneLoader: clearSceneName が未設定です。", this);
    }

    public void OnGameOver()
    {
        if (!string.IsNullOrEmpty(gameOverSceneName))
            StartCoroutine(LoadRoutine(gameOverSceneName));
        else
            Debug.LogWarning("ResultSceneLoader: gameOverSceneName が未設定です。", this);
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        if (delaySeconds > 0f)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(delaySeconds);
            else yield return new WaitForSeconds(delaySeconds);
        }

        if (resetTimeScaleOnLoad) Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

}