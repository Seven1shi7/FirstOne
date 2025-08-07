using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [Header("生命值设置")]
    public Slider hpSlider;
    public float maxHealth = 100f;
    public float currentHealth;
    public Text healthText;
    public GameObject damageTextPrefab;
    public Material hurtMaterial;
    public float hurtFlashDuration = 0.1f;
    public GameObject deathEffect;
    public float destroyDelay = 2f;
    
    private Renderer playerRenderer;
    private Material originalMaterial;
    private bool isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // 初始化生命值
        currentHealth = maxHealth;
        
        // 设置Slider最大值
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
        
        // 获取渲染器组件
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
        }
        
        // 更新UI
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !isDead)
        {
            TakeDamage(10f, "敌人攻击");
        }
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
        if (currentHealth < 0) currentHealth = 0;
        
        // 显示伤害数字
        ShowDamageText(damage, damageType);
        
        // 受伤闪烁效果
        if (hurtMaterial != null)
        {
            StartCoroutine(HurtFlash());
        }
        
        // 更新UI
        UpdateHealthUI();
        
        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"玩家受到 {damage} 点{damageType}伤害，剩余生命值: {currentHealth}");
    }
    
    /// <summary>
    /// 显示伤害数字
    /// </summary>
    private void ShowDamageText(float damage, string damageType)
    {
        // 如果没有预制体，创建一个简单的伤害数字
        if (damageTextPrefab == null)
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
            damageText.fontSize = 24;
            damageText.fontStyle = FontStyle.Bold;
            damageText.alignment = TextAnchor.MiddleCenter;
            
            // 根据伤害类型设置颜色
            switch (damageType.ToLower())
            {
                case "敌人攻击":
                    damageText.color = Color.red;
                    break;
                default:
                    damageText.color = Color.white;
                    break;
            }
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(100, 30);
            
            // 销毁整个Canvas
            Destroy(canvasObj, 1f);
        }
        else
        {
            // 在玩家头顶生成伤害数字
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
                    case "敌人攻击":
                        damageText.color = Color.red;
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
    /// 受伤闪烁效果
    /// </summary>
    private IEnumerator HurtFlash()
    {
        if (playerRenderer != null)
        {
            // 切换到受伤材质
            playerRenderer.material = hurtMaterial;
            
            // 等待闪烁时间
            yield return new WaitForSeconds(hurtFlashDuration);
            
            // 恢复原材质
            playerRenderer.material = originalMaterial;
        }
    }
    
    /// <summary>
    /// 更新生命值UI
    /// </summary>
    private void UpdateHealthUI()
    {
        // 更新Slider
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth;
        }
        
        // 更新文本
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }
    
    /// <summary>
    /// 死亡处理
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("玩家死亡!");
        
        // 播放死亡特效
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // 延迟销毁
        Destroy(gameObject, destroyDelay);
    }
}
