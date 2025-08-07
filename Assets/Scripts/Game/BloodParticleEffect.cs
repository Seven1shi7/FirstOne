using UnityEngine;

/// <summary>
/// 血滴粒子效果 - 实现血滴飞溅的视觉效果
/// </summary>
public class BloodParticleEffect : MonoBehaviour
{
    [Header("粒子设置")]
    public int particleCount = 20; // 粒子数量
    public float particleSpeed = 5f; // 粒子速度
    public float particleLifeTime = 2f; // 粒子生命周期
    public float gravity = 9.8f; // 重力
    
    [Header("血滴设置")]
    public Color bloodColor = Color.red; // 血滴颜色
    public float bloodSize = 0.1f; // 血滴大小
    public Material bloodMaterial; // 血滴材质
    
    [Header("飞溅设置")]
    public float spreadAngle = 60f; // 飞溅角度
    public float minSpeed = 2f; // 最小速度
    public float maxSpeed = 8f; // 最大速度
    
    private GameObject[] bloodDrops;
    private Vector3[] velocities;
    private float[] lifeTimers;
    private bool isActive = false;
    
    void Start()
    {
        // 创建血滴材质（如果没有提供）
        if (bloodMaterial == null)
        {
            CreateBloodMaterial();
        }
        
        // 初始化血滴数组
        InitializeBloodDrops();
    }
    
    void Update()
    {
        if (!isActive) return;
        
        UpdateBloodDrops();
    }
    
    /// <summary>
    /// 创建血滴材质
    /// </summary>
    private void CreateBloodMaterial()
    {
        bloodMaterial = new Material(Shader.Find("Standard"));
        bloodMaterial.color = bloodColor;
        bloodMaterial.SetFloat("_Metallic", 0.1f);
        bloodMaterial.SetFloat("_Smoothness", 0.3f);
    }
    
    /// <summary>
    /// 初始化血滴
    /// </summary>
    private void InitializeBloodDrops()
    {
        bloodDrops = new GameObject[particleCount];
        velocities = new Vector3[particleCount];
        lifeTimers = new float[particleCount];
        
        for (int i = 0; i < particleCount; i++)
        {
            // 创建血滴球体
            GameObject bloodDrop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bloodDrop.name = $"BloodDrop_{i}";
            bloodDrop.transform.SetParent(transform);
            bloodDrop.transform.localScale = Vector3.one * bloodSize;
            
            // 设置材质
            Renderer renderer = bloodDrop.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = bloodMaterial;
            }
            
            // 禁用碰撞器
            Collider collider = bloodDrop.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            bloodDrops[i] = bloodDrop;
            bloodDrops[i].SetActive(false);
        }
    }
    
    /// <summary>
    /// 播放血滴效果
    /// </summary>
    /// <param name="position">血滴起始位置</param>
    /// <param name="direction">飞溅方向</param>
    public void PlayBloodEffect(Vector3 position, Vector3 direction = default)
    {
        if (direction == default)
        {
            direction = Vector3.up; // 默认向上飞溅
        }
        
        // 重置所有血滴
        for (int i = 0; i < particleCount; i++)
        {
            bloodDrops[i].SetActive(true);
            bloodDrops[i].transform.position = position;
            
            // 计算随机速度
            Vector3 randomDirection = GetRandomDirection(direction);
            float speed = Random.Range(minSpeed, maxSpeed);
            velocities[i] = randomDirection * speed;
            
            lifeTimers[i] = 0f;
        }
        
        isActive = true;
    }
    
    /// <summary>
    /// 获取随机方向
    /// </summary>
    private Vector3 GetRandomDirection(Vector3 baseDirection)
    {
        // 在基础方向周围随机化
        Vector3 randomDir = baseDirection + Random.insideUnitSphere * 0.5f;
        randomDir.Normalize();
        
        // 应用飞溅角度
        float angle = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
        return Quaternion.AngleAxis(angle, Random.onUnitSphere) * randomDir;
    }
    
    /// <summary>
    /// 更新血滴
    /// </summary>
    private void UpdateBloodDrops()
    {
        bool allDead = true;
        
        for (int i = 0; i < particleCount; i++)
        {
            if (!bloodDrops[i].activeSelf) continue;
            
            lifeTimers[i] += Time.deltaTime;
            
            // 应用重力
            velocities[i] += Vector3.down * gravity * Time.deltaTime;
            
            // 更新位置
            bloodDrops[i].transform.position += velocities[i] * Time.deltaTime;
            
            // 更新透明度
            float alpha = 1f - (lifeTimers[i] / particleLifeTime);
            UpdateBloodDropAlpha(bloodDrops[i], alpha);
            
            // 检查生命周期
            if (lifeTimers[i] >= particleLifeTime)
            {
                bloodDrops[i].SetActive(false);
            }
            else
            {
                allDead = false;
            }
        }
        
        if (allDead)
        {
            isActive = false;
        }
    }
    
    /// <summary>
    /// 更新血滴透明度
    /// </summary>
    private void UpdateBloodDropAlpha(GameObject bloodDrop, float alpha)
    {
        Renderer renderer = bloodDrop.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }
    
    /// <summary>
    /// 停止血滴效果
    /// </summary>
    public void StopBloodEffect()
    {
        isActive = false;
        for (int i = 0; i < particleCount; i++)
        {
            bloodDrops[i].SetActive(false);
        }
    }
    
    /// <summary>
    /// 设置血滴颜色
    /// </summary>
    public void SetBloodColor(Color color)
    {
        bloodColor = color;
        if (bloodMaterial != null)
        {
            bloodMaterial.color = color;
        }
    }
    
    /// <summary>
    /// 设置血滴大小
    /// </summary>
    public void SetBloodSize(float size)
    {
        bloodSize = size;
        for (int i = 0; i < particleCount; i++)
        {
            if (bloodDrops[i] != null)
            {
                bloodDrops[i].transform.localScale = Vector3.one * bloodSize;
            }
        }
    }
} 