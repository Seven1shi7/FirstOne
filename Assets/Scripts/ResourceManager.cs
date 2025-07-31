using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

/// <summary>
/// C#��Դ����ϵͳ
/// �ṩ�ڴ��������ء���Դ���غ��ͷŹ���
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

    // ������ֵ�
    private Dictionary<string, ObjectPool> _objectPools = new Dictionary<string, ObjectPool>();

    // ��Դ�����ֵ�
    private Dictionary<string, UnityEngine.Object> _resourceCache = new Dictionary<string, UnityEngine.Object>();

    // �첽���������б�
    private List<AsyncOperation> _asyncOperations = new List<AsyncOperation>();

    // �ڴ���
    private float _lastMemoryCheck = 0f;
    private const float MEMORY_CHECK_INTERVAL = 30f; // 30����һ���ڴ�

    // ����ģʽ
    public bool DebugMode = false;

    #region �����ϵͳ

    /// <summary>
    /// �������
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

            // Ԥ��������
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
                // ����û�п��ö��󣬴����µ�
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

                // ��������������ٶ���
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
            // �������ж���
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
    /// ���������
    /// </summary>
    public void CreateObjectPool(string poolName, GameObject prefab, int poolSize)
    {
        if (_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning($"ResourceManager: ����� '{poolName}' �Ѵ���");
            return;
        }

        _objectPools[poolName] = new ObjectPool(poolName, prefab, poolSize);

        if (DebugMode)
            Debug.Log($"ResourceManager: ��������� '{poolName}' ��С: {poolSize}");
    }

    /// <summary>
    /// �Ӷ���ػ�ȡ����
    /// </summary>
    public GameObject GetFromPool(string poolName)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject();
        }

        Debug.LogError($"ResourceManager: ����� '{poolName}' ������");
        return null;
    }

    /// <summary>
    /// �����󷵻س���
    /// </summary>
    public void ReturnToPool(string poolName, GameObject obj)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ReturnObject(obj);
        }
        else
        {
            Debug.LogError($"ResourceManager: ����� '{poolName}' ������");
        }
    }

    /// <summary>
    /// ���ٶ����
    /// </summary>
    public void DestroyObjectPool(string poolName)
    {
        if (_objectPools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.Clear();
            _objectPools.Remove(poolName);

            if (DebugMode)
                Debug.Log($"ResourceManager: ���ٶ���� '{poolName}'");
        }
    }

    #endregion

    #region ��Դ����ϵͳ

    /// <summary>
    /// ͬ��������Դ
    /// </summary>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        // ��黺��
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object cached))
        {
            return cached as T;
        }

        // ������Դ
        T resource = Resources.Load<T>(path);
        if (resource != null)
        {
            _resourceCache[path] = resource;

            if (DebugMode)
                Debug.Log($"ResourceManager: ������Դ '{path}'");
        }
        else
        {
            Debug.LogError($"ResourceManager: �޷�������Դ '{path}'");
        }

        return resource;
    }

    /// <summary>
    /// �첽������Դ
    /// </summary>
    public IEnumerator LoadResourceAsync<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        // ��黺��
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object cached))
        {
            callback?.Invoke(cached as T);
            yield break;
        }

        // �첽������Դ
        ResourceRequest request = Resources.LoadAsync<T>(path);
        _asyncOperations.Add(request);

        yield return request;

        _asyncOperations.Remove(request);

        if (request.asset != null)
        {
            _resourceCache[path] = request.asset;
            callback?.Invoke(request.asset as T);

            if (DebugMode)
                Debug.Log($"ResourceManager: �첽������Դ '{path}'");
        }
        else
        {
            Debug.LogError($"ResourceManager: �޷��첽������Դ '{path}'");
            callback?.Invoke(null);
        }
    }

    /// <summary>
    /// ж����Դ
    /// </summary>
    public void UnloadResource(string path)
    {
        if (_resourceCache.TryGetValue(path, out UnityEngine.Object resource))
        {
            _resourceCache.Remove(path);
            Resources.UnloadAsset(resource);

            if (DebugMode)
                Debug.Log($"ResourceManager: ж����Դ '{path}'");
        }
    }

    /// <summary>
    /// ж��������Դ
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
            Debug.Log("ResourceManager: ж��������Դ");
    }

    #endregion

    #region �ڴ����

    /// <summary>
    /// ��ȡ��ǰ�ڴ�ʹ�����
    /// </summary>
    public MemoryInfo GetMemoryInfo()
    {
        return new MemoryInfo
        {
            TotalMemory = GC.GetTotalMemory(false),
            ManagedMemory = Profiler.GetTotalAllocatedMemoryLong(),
            UnusedMemory = Profiler.GetTotalUnusedReservedMemoryLong(),
            SystemMemory = SystemInfo.systemMemorySize * 1024 * 1024, // ת��Ϊ�ֽ�
            AvailableMemory = SystemInfo.systemMemorySize * 1024 * 1024 - Profiler.GetTotalAllocatedMemoryLong()
        };
    }

    /// <summary>
    /// ǿ����������
    /// </summary>
    public void ForceGarbageCollection()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (DebugMode)
            Debug.Log("ResourceManager: ִ����������");
    }

    /// <summary>
    /// ����ڴ�ʹ�����
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
                Debug.Log($"ResourceManager: �ڴ�ʹ����� - " +
                         $"�����ڴ�: {memoryInfo.ManagedMemory / 1024 / 1024:F1}MB, " +
                         $"���ڴ�: {memoryInfo.TotalMemory / 1024 / 1024:F1}MB, " +
                         $"ʹ����: {memoryUsagePercent:F1}%");
            }

            // ����ڴ�ʹ���ʳ���80%��ִ����������
            if (memoryUsagePercent > 80f)
            {
                Debug.LogWarning("ResourceManager: �ڴ�ʹ���ʹ��ߣ�ִ����������");
                ForceGarbageCollection();
            }
        }
    }

    #endregion

    #region ��������

    /// <summary>
    /// �첽���س���
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
            Debug.Log($"ResourceManager: ���� '{sceneName}' �������");
    }

    /// <summary>
    /// ж�س���
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
            Debug.Log($"ResourceManager: ���� '{sceneName}' ж�����");
    }

    #endregion

    #region ���߷���

    /// <summary>
    /// ��ȡ�������Ϣ
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
    /// ����������Դ
    /// </summary>
    public void CleanupAll()
    {
        // ��������
        foreach (var pool in _objectPools.Values)
        {
            pool.Clear();
        }
        _objectPools.Clear();

        // ������Դ����
        UnloadAllResources();

        // �����첽����
        _asyncOperations.Clear();

        // ǿ����������
        ForceGarbageCollection();

        if (DebugMode)
            Debug.Log("ResourceManager: ����������Դ���");
    }

    #endregion

    #region Unity��������

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
            // Ӧ����ͣʱִ����������
            ForceGarbageCollection();
        }
    }

    #endregion
}

/// <summary>
/// �ڴ���Ϣ�ṹ
/// </summary>
public struct MemoryInfo
{
    public long TotalMemory;      // ���ڴ�
    public long ManagedMemory;    // �й��ڴ�
    public long UnusedMemory;     // δʹ���ڴ�
    public long SystemMemory;     // ϵͳ�ڴ�
    public long AvailableMemory;  // �����ڴ�
}

/// <summary>
/// �������Ϣ�ṹ
/// </summary>
public struct PoolInfo
{
    public string Name;
    public int PoolSize;
    public int AvailableCount;
    public int ActiveCount;
}