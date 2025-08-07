using UnityEngine;

/// <summary>
/// 技能范围可视化器 - 用于在运行时显示技能范围
/// </summary>
public class SkillRangeVisualizer : MonoBehaviour
{
    [Header("可视化设置")]
    public bool showInGame = true; // 是否在游戏中显示
    public bool showOnSkillCast = true; // 是否在释放技能时显示
    public float displayDuration = 2f; // 显示持续时间
    
    [Header("材质设置")]
    public Material circleMaterial; // 圆形材质
    public Material rectangleMaterial; // 矩形材质
    public Material sectorMaterial; // 扇形材质
    
    [Header("颜色设置")]
    public Color circleColor = Color.red;
    public Color rectangleColor = Color.blue;
    public Color sectorColor = Color.green;
    
    private GameObject circleRange;
    private GameObject rectangleRange;
    private GameObject sectorRange;
    private float displayTimer = 0f;
    private bool isDisplaying = false;
    
    private Move moveComponent;
    
    void Start()
    {
        moveComponent = GetComponent<Move>();
        if (moveComponent == null)
        {
            Debug.LogError("SkillRangeVisualizer需要Move组件！");
            return;
        }
        
        // 创建技能范围对象
        CreateSkillRangeObjects();
        
        // 初始隐藏
        HideAllRanges();
    }
    
    void Update()
    {
        // 更新显示计时器
        if (isDisplaying)
        {
            displayTimer -= Time.deltaTime;
            if (displayTimer <= 0)
            {
                HideAllRanges();
                isDisplaying = false;
            }
        }
        
        // 实时更新范围位置和大小
        if (showInGame && !isDisplaying)
        {
            UpdateRangePositions();
        }
    }
    
    /// <summary>
    /// 创建技能范围可视化对象
    /// </summary>
    private void CreateSkillRangeObjects()
    {
        // 创建圆形范围
        circleRange = CreateRangeObject("CircleRange", circleMaterial, circleColor);
        
        // 创建矩形范围
        rectangleRange = CreateRangeObject("RectangleRange", rectangleMaterial, rectangleColor);
        
        // 创建扇形范围
        sectorRange = CreateRangeObject("SectorRange", sectorMaterial, sectorColor);
    }
    
    /// <summary>
    /// 创建范围对象
    /// </summary>
    private GameObject CreateRangeObject(string name, Material material, Color color)
    {
        GameObject rangeObj = new GameObject(name);
        rangeObj.transform.SetParent(transform);
        rangeObj.transform.localPosition = Vector3.zero;
        
        // 添加MeshRenderer
        MeshRenderer renderer = rangeObj.AddComponent<MeshRenderer>();
        MeshFilter filter = rangeObj.AddComponent<MeshFilter>();
        
        // 设置材质
        if (material != null)
        {
            renderer.material = material;
        }
        else
        {
            // 创建默认材质
            Material defaultMat = new Material(Shader.Find("Standard"));
            defaultMat.color = new Color(color.r, color.g, color.b, 0.3f);
            defaultMat.SetFloat("_Mode", 3); // Transparent mode
            defaultMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            defaultMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            defaultMat.SetInt("_ZWrite", 0);
            defaultMat.DisableKeyword("_ALPHATEST_ON");
            defaultMat.EnableKeyword("_ALPHABLEND_ON");
            defaultMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            defaultMat.renderQueue = 3000;
            renderer.material = defaultMat;
        }
        
        return rangeObj;
    }
    
    /// <summary>
    /// 更新范围位置和大小
    /// </summary>
    private void UpdateRangePositions()
    {
        if (moveComponent == null) return;
        
        // 更新圆形范围
        UpdateCircleRange();
        
        // 更新矩形范围
        UpdateRectangleRange();
        
        // 更新扇形范围
        UpdateSectorRange();
    }
    
    /// <summary>
    /// 更新圆形范围
    /// </summary>
    private void UpdateCircleRange()
    {
        if (circleRange == null) return;
        
        MeshFilter filter = circleRange.GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.mesh = CreateCircleMesh(moveComponent.skillRadius);
        }
    }
    
    /// <summary>
    /// 更新矩形范围
    /// </summary>
    private void UpdateRectangleRange()
    {
        if (rectangleRange == null) return;
        
        MeshFilter filter = rectangleRange.GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.mesh = CreateRectangleMesh(moveComponent.skillWidth, moveComponent.skillLength);
        }
        
        // 更新旋转以匹配角色朝向
        rectangleRange.transform.rotation = transform.rotation;
        rectangleRange.transform.position = transform.position + transform.forward * (moveComponent.skillLength / 2);
    }
    
    /// <summary>
    /// 更新扇形范围
    /// </summary>
    private void UpdateSectorRange()
    {
        if (sectorRange == null) return;
        
        MeshFilter filter = sectorRange.GetComponent<MeshFilter>();
        if (filter != null)
        {
            filter.mesh = CreateSectorMesh(moveComponent.skillRadius, moveComponent.skillAngle);
        }
        
        // 更新旋转以匹配角色朝向
        sectorRange.transform.rotation = transform.rotation;
    }
    
    /// <summary>
    /// 创建圆形网格
    /// </summary>
    private Mesh CreateCircleMesh(float radius)
    {
        Mesh mesh = new Mesh();
        
        int segments = 32;
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        
        // 中心点
        vertices[0] = Vector3.zero;
        
        // 圆周上的点
        for (int i = 0; i < segments; i++)
        {
            float angle = (360f / segments) * i * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
        
        // 创建三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    /// <summary>
    /// 创建矩形网格
    /// </summary>
    private Mesh CreateRectangleMesh(float width, float length)
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-width / 2, 0, -length / 2);
        vertices[1] = new Vector3(width / 2, 0, -length / 2);
        vertices[2] = new Vector3(width / 2, 0, length / 2);
        vertices[3] = new Vector3(-width / 2, 0, length / 2);
        
        int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    /// <summary>
    /// 创建扇形网格
    /// </summary>
    private Mesh CreateSectorMesh(float radius, float angle)
    {
        Mesh mesh = new Mesh();
        
        int segments = 20;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        
        // 中心点
        vertices[0] = Vector3.zero;
        
        // 扇形边界点
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -angle / 2 + (angle / segments) * i;
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
            vertices[i + 1] = direction * radius;
        }
        
        // 创建三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    /// <summary>
    /// 显示技能范围
    /// </summary>
    public void ShowSkillRanges()
    {
        if (!showOnSkillCast) return;
        
        ShowAllRanges();
        displayTimer = displayDuration;
        isDisplaying = true;
    }
    
    /// <summary>
    /// 显示所有范围
    /// </summary>
    private void ShowAllRanges()
    {
        if (circleRange != null) circleRange.SetActive(true);
        if (rectangleRange != null) rectangleRange.SetActive(true);
        if (sectorRange != null) sectorRange.SetActive(true);
    }
    
    /// <summary>
    /// 隐藏所有范围
    /// </summary>
    private void HideAllRanges()
    {
        if (circleRange != null) circleRange.SetActive(false);
        if (rectangleRange != null) rectangleRange.SetActive(false);
        if (sectorRange != null) sectorRange.SetActive(false);
    }
    
    /// <summary>
    /// 切换显示状态
    /// </summary>
    public void ToggleDisplay()
    {
        showInGame = !showInGame;
        if (showInGame)
        {
            ShowAllRanges();
        }
        else
        {
            HideAllRanges();
        }
    }
} 