using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

/// <summary>
/// C#资源管理系统
/// 提供内存管理、对象池、资源加载和释放功能
/// </summary>
public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ResourceManager");
                _instance = go.AddComponent<ResourceManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // 对象池字典
    private Dictionary<string, ObjectPool> _objectPools = new Dictionary<string, ObjectPool>();

    // 资源缓存字典
    private Dictionary<string, UnityEngine.Object> _resourceCache = new Dictionary<string, UnityEngine.Object>();

    // 异步加载任务列表
    private List<AsyncOperation> _asyncOperations = new List<AsyncOperation>();

    // 内存监控
    private float _lastMemoryCheck = 0f;
    private const float MEMORY_CHECK_INTERVAL = 30f; // 30秒检查一次内存

    // 调试模式
    public bool DebugMode = false;

    #region 对象池系统

    /// <summary>
    /// 对象池类
    /// </summary>
    public class ObjectPool
    {
        public string Name { get; private set; }
        public GameObject Prefab { get; private set; }
        public int PoolSize { get; private set; }
        public Queue<GameObject> AvailableObjects { get; private set; }
        public List<GameObject> ActiveObjects { get; private set; }

        public ObjectPool(string name, GameObject prefab, int poolSize)
        {
            Name = name;
            Prefab = prefab;
            PoolSize = poolSize;
            AvailableObjects = new Queue<GameObject>();
            ActiveObjects = new List<GameObject>();

            // 预创建对象
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                AvailableObjects.Enqueue(obj);
            }
        }

        public GameObject GetObject()
        {
            GameObject obj = null;

            if (AvailableObjects.Count > 0)
            {
                obj = AvailableObjects.Dequeue();
            }
            else
            {
                // 池中没有可用对象，创建新的
                obj = Instantiate(Prefab);
            }

            obj.SetActive(true);
            ActiveObjects.Add(obj);
            return obj;
        }

        public void ReturnObject(GameObject obj)
        {
            if (obj != null && ActiveObjects.Contains(obj))
            {
                obj.SetActive(false);
                ActiveObjects.Remove(obj);

                // 如果池已满，销毁对象
                if (AvailableObjects.Count >= PoolSize)
                {
                    Destroy(obj);
                }
                else
                {
                    AvailableObjects.Enqueue(obj);
                }
            }
        }

        public void Clear()
        {
            // 销毁所有对象
            foreach (var obj in AvailableObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }

            foreach (var obj in ActiveObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }

            AvailableObjects.Clear();
            ActiveObjects.Clear();
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    public void CreateObjectPool(string poolName, GameObject prefab, int poolSize)
    {
        if (_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning($"ResourceManager: 对象池 '{poolName}' 已存在");
            return;
        }

        _objectPools[poolName] = new ObjectPool(poolName, prefab, poolSize);

        if (DebugMode)
            Debug.Log($"ResourceManager: 创建对象池 '{poolName}' 大小: {poolSize}");
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    public GameObject GetFromPool(string poolName)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject();
        }

        Debug.LogError($"ResourceManager: 对象池 '{poolName}' 不存在");
        return null;
    }

    /// <summary>
    /// 将对象返回池中
    /// </summary>
    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ReturnObject(obj);
        }
        else
        {
            Debug.LogError($"ResourceManager: 对象池 '{poolName}' 不存在");
        }
    }

    /// <summary>
    /// 销毁对象池
    /// </summary>
    public void DestroyObjectPool(string poolName)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.Clear();
            _objectPools.Remove(poolName);

            if (DebugMode)
                Debug.Log($"ResourceManager: 销毁对象池 '{poolName}'");
        }
    }

    #endregion

    #region 资源加载系统

    /// <summary>
    /// 同步加载资源
    /// </summary>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        // 检查缓存
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object cached))
        {
            return cached as T;
        }

        // 加载资源
        T resource = Resources.Load<T>(path);
        if (resource != null)
        {
            _resourceCache[path] = resource;

            if (DebugMode)
                Debug.Log($"ResourceManager: 加载资源 '{path}'");
        }
        else
        {
            Debug.LogError($"ResourceManager: 无法加载资源 '{path}'");
        }

        return resource;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public IEnumerator LoadResourceAsync<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        // 检查缓存
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object cached))
        {
            callback?.Invoke(cached as T);
            yield break;
        }

        // 异步加载资源
        ResourceRequest request = Resources.LoadAsync<T>(path);
        _asyncOperations.Add(request);

        yield return request;

        _asyncOperations.Remove(request);

        if (request.asset != null)
        {
            _resourceCache[path] = request.asset;
            callback?.Invoke(request.asset as T);

            if (DebugMode)
                Debug.Log($"ResourceManager: 异步加载资源 '{path}'");
        }
        else
        {
            Debug.LogError($"ResourceManager: 无法异步加载资源 '{path}'");
            callback?.Invoke(null);
        }
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    public void UnloadResource(string path)
    {
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object resource))
        {
            _resourceCache.Remove(path);
            Resources.UnloadAsset(resource);

            if (DebugMode)
                Debug.Log($"ResourceManager: 卸载资源 '{path}'");
        }
    }

    /// <summary>
    /// 卸载所有资源
    /// </summary>
    public void UnloadAllResources()
    {
        foreach (var resource in _resourceCache.Values)
        {
            if (resource != null)
                Resources.UnloadAsset(resource);
        }

        _resourceCache.Clear();

        if (DebugMode)
            Debug.Log("ResourceManager: 卸载所有资源");
    }

    #endregion

    #region 内存管理

    /// <summary>
    /// 获取当前内存使用情况
    /// </summary>
    public MemoryInfo GetMemoryInfo()
    {
        return new MemoryInfo
        {
            TotalMemory = GC.GetTotalMemory(false),
            ManagedMemory = Profiler.GetTotalAllocatedMemoryLong(),
            UnusedMemory = Profiler.GetTotalUnusedReservedMemoryLong(),
            SystemMemory = SystemInfo.systemMemorySize * 1024 * 1024, // 转换为字节
            AvailableMemory = SystemInfo.systemMemorySize * 1024 * 1024 - Profiler.GetTotalAllocatedMemoryLong()
        };
    }

    /// <summary>
    /// 强制垃圾回收
    /// </summary>
    public void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (DebugMode)
            Debug.Log("ResourceManager: 执行垃圾回收");
    }

    /// <summary>
    /// 检查内存使用情况
    /// </summary>
    private void CheckMemoryUsage()
    {
        if (Time.time - _lastMemoryCheck >= MEMORY_CHECK_INTERVAL)
        {
            _lastMemoryCheck = Time.time;

            var memoryInfo = GetMemoryInfo();
            float memoryUsagePercent = (float)memoryInfo.ManagedMemory / memoryInfo.SystemMemory * 100f;

            if (DebugMode)
            {
                Debug.Log($"ResourceManager: 内存使用情况 - " +
                         $"管理内存: {memoryInfo.ManagedMemory / 1024 / 1024:F1}MB, " +
                         $"总内存: {memoryInfo.TotalMemory / 1024 / 1024:F1}MB, " +
                         $"使用率: {memoryUsagePercent:F1}%");
            }

            // 如果内存使用率超过80%，执行垃圾回收
            if (memoryUsagePercent > 80f)
            {
                Debug.LogWarning("ResourceManager: 内存使用率过高，执行垃圾回收");
                ForceGarbageCollection();
            }
        }
    }

    #endregion

    #region 场景管理

    /// <summary>
    /// 异步加载场景
    /// </summary>
    public IEnumerator LoadSceneAsync(string sceneName, Action<float> progressCallback = null, Action completeCallback = null)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        _asyncOperations.Add(operation);

        while (!operation.isDone)
        {
            progressCallback?.Invoke(operation.progress);
            yield return null;
        }

        _asyncOperations.Remove(operation);
        completeCallback?.Invoke();

        if (DebugMode)
            Debug.Log($"ResourceManager: 场景 '{sceneName}' 加载完成");
    }

    /// <summary>
    /// 卸载场景
    /// </summary>
    public IEnumerator UnloadSceneAsync(string sceneName, Action completeCallback = null)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
        _asyncOperations.Add(operation);

        while (!operation.isDone)
        {
            yield return null;
        }

        _asyncOperations.Remove(operation);
        completeCallback?.Invoke();

        if (DebugMode)
            Debug.Log($"ResourceManager: 场景 '{sceneName}' 卸载完成");
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 获取对象池信息
    /// </summary>
    public Dictionary<string, PoolInfo> GetPoolInfo()
    {
        var poolInfo = new Dictionary<string, PoolInfo>();

        foreach (var kvp in _objectPools)
        {
            var pool = kvp.Value;
            poolInfo[kvp.Key] = new PoolInfo
            {
                Name = pool.Name,
                PoolSize = pool.PoolSize,
                AvailableCount = pool.AvailableObjects.Count,
                ActiveCount = pool.ActiveObjects.Count
            };
        }

        return poolInfo;
    }

    /// <summary>
    /// 清理所有资源
    /// </summary>
    public void CleanupAll()
    {
        // 清理对象池
        foreach (var pool in _objectPools.Values)
        {
            pool.Clear();
        }
        _objectPools.Clear();

        // 清理资源缓存
        UnloadAllResources();

        // 清理异步操作
        _asyncOperations.Clear();

        // 强制垃圾回收
        ForceGarbageCollection();

        if (DebugMode)
            Debug.Log("ResourceManager: 清理所有资源完成");
    }

    #endregion

    #region Unity生命周期

    private void Update()
    {
        CheckMemoryUsage();
    }

    private void OnDestroy()
    {
        CleanupAll();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 应用暂停时执行垃圾回收
            ForceGarbageCollection();
        }
    }

    #endregion
}

/// <summary>
/// 内存信息结构
/// </summary>
public struct MemoryInfo
{
    public long TotalMemory;      // 总内存
    public long ManagedMemory;    // 托管内存
    public long UnusedMemory;     // 未使用内存
    public long SystemMemory;     // 系统内存
    public long AvailableMemory;  // 可用内存
}

/// <summary>
/// 对象池信息结构
/// </summary>
public struct PoolInfo
{
    public string Name;
    public int PoolSize;
    public int AvailableCount;
    public int ActiveCount;
}