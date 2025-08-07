using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 伤害数字显示脚本
/// </summary>
public class DamageText : MonoBehaviour
{
    [Header("动画设置")]
    public float moveSpeed = 1f; // 向上移动速度
    public float fadeSpeed = 1f; // 淡出速度
    public float lifeTime = 1f; // 生命周期
    
    private Text damageText;
    private float timer = 0f;
    private Vector3 startPosition;
    
    void Start()
    {
        damageText = GetComponent<Text>();
        if (damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }
        
        startPosition = transform.position;
        
        // 确保文本面向摄像机
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // 向上移动
        transform.position = startPosition + Vector3.up * (moveSpeed * timer);
        
        // 淡出效果
        if (damageText != null)
        {
            Color textColor = damageText.color;
            textColor.a = Mathf.Lerp(1f, 0f, timer / lifeTime);
            damageText.color = textColor;
        }
        
        // 确保始终面向摄像机
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
        
        // 销毁
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 设置伤害文本
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="color">文本颜色</param>
    public void SetDamage(float damage, Color color)
    {
        if (damageText != null)
        {
            damageText.text = $"-{damage:F0}";
            damageText.color = color;
        }
    }
} 