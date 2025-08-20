using UnityEngine;
using System.Collections;

public class testmin : MonoBehaviour
{
    public Camera minicamera;
    public Transform player;
    public Transform miniplayerIcon;//小地图人物图标
    public Transform maxplayerIcon;
    private float mapSize;//小地图的orthographicSize大小
    public float Maxmapsize;//大地图的orthographicSize大小
    public float minSize;//小地图的orthographicSize最小值
    public float maxSize; //小地图的orthographicSize最大值
    public GameObject maxmap;//大地图
    public GameObject minimap;//小地图



    void Awake()
    {
        mapSize = minicamera.orthographicSize;

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



        minicamera.transform.position = new Vector3(player.position.x, minicamera.transform.position.y, player.position.z);
        miniplayerIcon.eulerAngles = new Vector3(0, 0, -player.eulerAngles.y);



    }

    //打开大地图
    public void OpenMaxmap()
    {
        maxmap.gameObject.SetActive(true);
        minimap.gameObject.SetActive(false);
        minicamera.orthographicSize = Maxmapsize;


    }

    //打开小地图
    public void OpenMinimap()
    {
        maxmap.gameObject.SetActive(false);
        minimap.gameObject.SetActive(true);
        minicamera.orthographicSize = mapSize;

    }
    //缩放地图方法
    public void ChangeMapSize(float value)
    {
        mapSize += value;
        mapSize = Mathf.Clamp(mapSize, minSize, maxSize);
        minicamera.orthographicSize = mapSize;
    }
}