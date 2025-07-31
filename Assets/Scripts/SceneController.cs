using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fadeCanvasGroup.alpha = 0f;
    }

    // 加载场景（带淡入淡出效果）
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
    }

    // 异步加载场景
    public void LoadSceneAsync(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
    }

    // 重新加载当前场景
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;

        // 淡出
        yield return StartCoroutine(Fade(0f, 1f));

        // 加载新场景
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 淡入
        yield return StartCoroutine(Fade(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        isTransitioning = true;

        // 淡出
        yield return StartCoroutine(Fade(0f, 1f));

        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 淡入
        yield return StartCoroutine(Fade(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}