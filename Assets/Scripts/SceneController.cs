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

    // ���س����������뵭��Ч����
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
    }

    // �첽���س���
    public void LoadSceneAsync(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
    }

    // ���¼��ص�ǰ����
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;

        // ����
        yield return StartCoroutine(Fade(0f, 1f));

        // �����³���
        yield return SceneManager.LoadSceneAsync(sceneName);

        // ����
        yield return StartCoroutine(Fade(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        isTransitioning = true;

        // ����
        yield return StartCoroutine(Fade(0f, 1f));

        // �첽���س���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // �ȴ������������
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ����
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