using UnityEngine;
using System.Collections;

public class testmin : MonoBehaviour
{
    public Camera minicamera;
    public Transform player;
    public Transform miniplayerIcon;//С��ͼ����ͼ��
    public Transform maxplayerIcon;
    private float mapSize;//С��ͼ��orthographicSize��С
    public float Maxmapsize;//���ͼ��orthographicSize��С
    public float minSize;//С��ͼ��orthographicSize��Сֵ
    public float maxSize; //С��ͼ��orthographicSize���ֵ
    public GameObject maxmap;//���ͼ
    public GameObject minimap;//С��ͼ



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

    //�򿪴��ͼ
    public void OpenMaxmap()
    {
        maxmap.gameObject.SetActive(true);
        minimap.gameObject.SetActive(false);
        minicamera.orthographicSize = Maxmapsize;


    }

    //��С��ͼ
    public void OpenMinimap()
    {
        maxmap.gameObject.SetActive(false);
        minimap.gameObject.SetActive(true);
        minicamera.orthographicSize = mapSize;

    }
    //���ŵ�ͼ����
    public void ChangeMapSize(float value)
    {
        mapSize += value;
        mapSize = Mathf.Clamp(mapSize, minSize, maxSize);
        minicamera.orthographicSize = mapSize;
    }
}