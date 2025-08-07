using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敌人脚本 - 包含生命值、受伤、死亡等功能
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("生命值设置")]
    public float maxHealth = 100f; // 最大生命值
    public float currentHealth; // 当前生命值
    
    [Header("UI显示")]
    public Slider healthBar; // 血条
    public Text healthText; // 血量文本
    public GameObject damageTextPrefab; // 伤害数字预制体
    
    [Header("视觉效果")]
    public Material normalMaterial; // 正常材质
    public Material hurtMaterial; // 受伤材质
    public float hurtFlashDuration = 0.1f; // 受伤闪烁时间
    
    [Header("死亡设置")]
    public GameObject deathEffect; // 死亡特效
    public float destroyDelay = 2f; // 销毁延迟
    
    private Renderer enemyRenderer;
    private Material originalMaterial;
    private bool isDead = false;
    
    void Start()
    {
        // 初始化生命值
        currentHealth = maxHealth;
        
        // 获取渲染器组件
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalMaterial = enemyRenderer.material;
        }
        
        // 设置标签
        gameObject.tag = "Enemy";
        
        // 更新UI
        UpdateHealthUI();
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="damageType">伤害类型</param>
    public void TakeDamage(float damage, string damageType = "普通")
    {
        if (isDead) return;
        
        // 减少生命值
        currentHealth -= damage;
        
        // 显示伤害数字
        ShowDamageText(damage, damageType);
        
        // 受伤闪烁效果
        StartCoroutine(HurtFlash());
        
        // 更新UI
        UpdateHealthUI();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"{gameObject.name} 受到 {damage} 点伤害，剩余生命值: {currentHealth}");
    }
    
    /// <summary>
    /// 显示伤害数字
    /// </summary>
    private void ShowDamageText(float damage, string damageType)
    {
        // 如果没有预制体，创建一个简单的伤害数字
        if (damageTextPrefab == null)
        {
            CreateSimpleDamageText(damage, damageType);
        }
        else
        {
            // 在敌人头顶生成伤害数字
            Vector3 spawnPosition = transform.position + Vector3.up * 2f;
            GameObject damageTextObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity);
            
            // 设置伤害文本
            Text damageText = damageTextObj.GetComponent<Text>();
            if (damageText != null)
            {
                damageText.text = $"-{damage}";
                
                // 根据伤害类型设置颜色
                switch (damageType.ToLower())
                {
                    case "圆形":
                        damageText.color = Color.red;
                        break;
                    case "矩形":
                        damageText.color = Color.blue;
                        break;
                    case "扇形":
                        damageText.color = Color.green;
                        break;
                    default:
                        damageText.color = Color.white;
                        break;
                }
            }
            
            // 销毁伤害数字
            Destroy(damageTextObj, 1f);
        }
    }
    
    /// <summary>
    /// 创建简单的伤害数字（当没有预制体时）
    /// </summary>
    private void CreateSimpleDamageText(float damage, string damageType)
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("DamageCanvas");
        canvasObj.transform.position = transform.position + Vector3.up * 2f;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        // 创建伤害文本
        GameObject textObj = new GameObject("DamageText");
        textObj.transform.SetParent(canvasObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        
        Text damageText = textObj.AddComponent<Text>();
        damageText.text = $"-{damage}";
        // 安全地设置字体
        SetFontSafely(damageText);
        damageText.fontSize = 24;
        damageText.fontStyle = FontStyle.Bold;
        damageText.alignment = TextAnchor.MiddleCenter;
        
        // 根据伤害类型设置颜色
        switch (damageType.ToLower())
        {
            case "圆形":
                damageText.color = Color.red;
                break;
            case "矩形":
                damageText.color = Color.blue;
                break;
            case "扇形":
                damageText.color = Color.green;
                break;
            default:
                damageText.color = Color.white;
                break;
        }
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        
        // 添加DamageText组件来处理动画
        DamageText damageTextComponent = textObj.AddComponent<DamageText>();
        
        // 销毁整个Canvas
        Destroy(canvasObj, 1f);
    }
    
    /// <summary>
    /// 受伤闪烁效果
    /// </summary>
    private System.Collections.IEnumerator HurtFlash()
    {
        if (enemyRenderer != null && hurtMaterial != null)
        {
            // 切换到受伤材质
            enemyRenderer.material = hurtMaterial;
            
            // 等待闪烁时间
            yield return new WaitForSeconds(hurtFlashDuration);
            
            // 恢复原材质
            enemyRenderer.material = originalMaterial;
        }
    }
    
    /// <summary>
    /// 更新生命值UI
    /// </summary>
    public void UpdateHealthUI()
    {
        // 更新血条
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
        
        // 更新血量文本
        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
        }
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log($"{gameObject.name} 死亡了！");
        
        // 播放死亡特效
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
        
        // 禁用碰撞器
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        
        // 播放死亡动画（如果有）
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // 延迟销毁
        Destroy(gameObject, destroyDelay);
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    /// <param name="healAmount">治疗量</param>
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthUI();
        
        Debug.Log($"{gameObject.name} 恢复了 {healAmount} 点生命值，当前生命值: {currentHealth}");
    }
    
    /// <summary>
    /// 获取生命值百分比
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    /// <summary>
    /// 检查是否存活
    /// </summary>
    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
    
    /// <summary>
    /// 安全地设置字体
    /// </summary>
    private void SetFontSafely(Text textComponent)
    {
        // 不设置字体，使用Unity默认字体
        // 这样可以避免字体错误，同时保持功能正常
    }
} 