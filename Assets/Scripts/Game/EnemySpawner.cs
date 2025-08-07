using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敌人生成器 - 用于测试伤害系统
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject enemyPrefab; // 敌人预制体
    public int maxEnemies = 5; // 最大敌人数
    public float spawnRadius = 10f; // 生成半径
    public float spawnInterval = 3f; // 生成间隔
    
    [Header("测试设置")]
    public KeyCode spawnKey = KeyCode.Space; // 手动生成按键
    public bool autoSpawn = true; // 自动生成
    
    private float spawnTimer = 0f;
    private int currentEnemyCount = 0;
    
    void Start()
    {
        // 如果没有预制体，创建一个简单的敌人
        if (enemyPrefab == null)
        {
            CreateDefaultEnemyPrefab();
        }
    }
    
    void Update()
    {
        // 手动生成
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnEnemy();
        }
        
        // 自动生成
        if (autoSpawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval && currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
        
        // 更新当前敌人数
        UpdateEnemyCount();
    }
    
    /// <summary>
    /// 生成敌人
    /// </summary>
    public void SpawnEnemy()
    {
        if (enemyPrefab == null || currentEnemyCount >= maxEnemies) return;
        
        // 随机生成位置
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        Vector3 spawnPosition = transform.position + randomDirection * spawnRadius;
        spawnPosition.y = 60; // 确保在地面上
        
        // 生成敌人
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.name = $"Enemy_{currentEnemyCount + 1}";
        
        currentEnemyCount++;
        
        Debug.Log($"生成了敌人: {enemy.name}");
    }
    
    /// <summary>
    /// 更新敌人数
    /// </summary>
    private void UpdateEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        currentEnemyCount = enemies.Length;
    }
    
    /// <summary>
    /// 创建默认敌人预制体
    /// </summary>
    private void CreateDefaultEnemyPrefab()
    {
        // 创建一个简单的立方体作为敌人
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.name = "DefaultEnemy";
        enemy.tag = "Enemy";
        
        // 添加Enemy组件
        Enemy enemyComponent = enemy.AddComponent<Enemy>();
        enemyComponent.maxHealth = 100f;
        
        // 添加血条UI
        CreateHealthUI(enemy);
        
        // 设置材质
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        
        // 保存为预制体
        enemyPrefab = enemy;
        
        Debug.Log("创建了默认敌人预制体");
    }
    
    /// <summary>
    /// 为敌人创建血条UI
    /// </summary>
    private void CreateHealthUI(GameObject enemy)
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("EnemyCanvas");
        canvasObj.transform.SetParent(enemy.transform);
        canvasObj.transform.localPosition = Vector3.up * 2f;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // 创建血条背景
        GameObject healthBarBg = new GameObject("HealthBarBg");
        healthBarBg.transform.SetParent(canvasObj.transform);
        healthBarBg.transform.localPosition = Vector3.zero;
        
        Image bgImage = healthBarBg.AddComponent<Image>();
        bgImage.color = Color.black;
        RectTransform bgRect = healthBarBg.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(100, 10);
        
        // 创建血条
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(healthBarBg.transform);
        healthBarObj.transform.localPosition = Vector3.zero;
        
        Slider healthBar = healthBarObj.AddComponent<Slider>();
        healthBar.fillRect = healthBarObj.GetComponent<RectTransform>();
        healthBar.minValue = 0f;
        healthBar.maxValue = 1f;
        healthBar.value = 1f;
        
        Image fillImage = healthBarObj.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRect = healthBarObj.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(100, 10);
        
        // 创建血量文本
        GameObject healthTextObj = new GameObject("HealthText");
        healthTextObj.transform.SetParent(canvasObj.transform);
        healthTextObj.transform.localPosition = Vector3.up * 15f;
        
        Text healthText = healthTextObj.AddComponent<Text>();
        healthText.text = "100/100";
        // 安全地设置字体
        SetFontSafely(healthText);
        healthText.fontSize = 12;
        healthText.color = Color.white;
        healthText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = healthTextObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 20);
        
        // 设置Enemy组件的UI引用
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.healthBar = healthBar;
            enemyComponent.healthText = healthText;
        }
    }
    
    /// <summary>
    /// 清除所有敌人
    /// </summary>
    public void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        currentEnemyCount = 0;
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