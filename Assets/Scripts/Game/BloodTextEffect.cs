using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 血滴文本效果 - 实现伤害数字的飘血动画
/// </summary>
public class BloodTextEffect : MonoBehaviour
{
    [Header("动画设置")]
    public float moveSpeed = 2f; // 移动速度
    public float fadeSpeed = 1f; // 淡出速度
    public float lifeTime = 2f; // 生命周期
    public float bounceHeight = 1f; // 弹跳高度
    public float bounceSpeed = 3f; // 弹跳速度

    [Header("随机效果")]
    public float randomMoveRange = 0.5f; // 随机移动范围
    public float randomRotationRange = 15f; // 随机旋转范围

    [Header("颜色渐变")]
    public Color startColor = Color.red; // 起始颜色
    public Color endColor = new Color(1f, 0.5f, 0f); // 橙色
    public bool useColorGradient = true; // 是否使用颜色渐变

    private Text damageText;
    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Vector3 randomDirection;
    private float timer = 0f;
    private float bounceTimer = 0f;
    private bool isInitialized = false;

    void Start()
    {
        damageText = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        if (damageText == null)
        {
            Debug.LogError("BloodTextEffect需要Text组件！");
            return;
        }

        InitializeEffect();
    }

    void Update()
    {
        if (!isInitialized) return;

        timer += Time.deltaTime;
        bounceTimer += Time.deltaTime * bounceSpeed;

        // 计算新位置
        Vector3 newPosition = CalculateNewPosition();
        rectTransform.position = newPosition;

        // 始终面向摄像机
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }

        // 更新颜色
        UpdateColor();

        // 更新透明度
        UpdateAlpha();

        // 检查生命周期
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEffect()
    {
        startPosition = rectTransform.position;

        // 随机方向
        randomDirection = new Vector3(
            Random.Range(-randomMoveRange, randomMoveRange),
            Random.Range(0.5f, 1f),
            Random.Range(-randomMoveRange, randomMoveRange)
        );

        // 随机旋转
        float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
        rectTransform.rotation = Quaternion.Euler(0, 0, randomRotation);

        // 设置初始颜色
        if (useColorGradient)
        {
            damageText.color = startColor;
        }

        isInitialized = true;
    }

    private Vector3 CalculateNewPosition()
    {
        Vector3 baseMovement = randomDirection * moveSpeed * timer;
        float bounceOffset = Mathf.Sin(bounceTimer) * bounceHeight * (1f - timer / lifeTime);
        float gravityOffset = -0.5f * timer * timer;
        return startPosition + baseMovement + Vector3.up * (bounceOffset + gravityOffset);
    }

    private void UpdateColor()
    {
        if (!useColorGradient) return;
        float progress = timer / lifeTime;
        damageText.color = Color.Lerp(startColor, endColor, progress);
    }

    private void UpdateAlpha()
    {
        float alpha = 1f - (timer / lifeTime);
        Color currentColor = damageText.color;
        currentColor.a = alpha;
        damageText.color = currentColor;
    }

    /// <summary>
    /// 设置伤害文本
    /// </summary>
    public void SetDamageText(float damage, string damageType)
    {
        if (damageText == null) return;
        damageText.text = $"-{damage}";

        // 根据伤害类型设置颜色
        switch (damageType.ToLower())
        {
            case "圆形":
                startColor = Color.red;
                endColor = new Color(1f, 0.5f, 0f); // 橙色
                break;
            case "矩形":
                startColor = Color.blue;
                endColor = Color.cyan;
                break;
            case "扇形":
                startColor = Color.green;
                endColor = Color.yellow;
                break;
            case "暴击":
                startColor = Color.magenta;
                endColor = Color.red;
                damageText.text = $"暴击! -{damage}";
                damageText.fontSize = (int)(damageText.fontSize * 1.5f);
                moveSpeed *= 1.5f;
                bounceHeight *= 1.5f;
                break;
            default:
                startColor = Color.white;
                endColor = Color.gray;
                break;
        }
    }
}
