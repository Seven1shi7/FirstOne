using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JumpNumber : MonoBehaviour
{
    public GameObject Number; // ����Ԥ����
    private Camera mainCamera;//�������
    //����ģʽ
    public static JumpNumber instance;
    private void Awake()
    {
        // ����ģʽ�ļ�ʵ�֣�ȷ��JumpNumber�ڳ�����ΪΨһʵ��
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // �Զ���ȡ�������
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    //����Ŀ�����壬�������֣��Ƿ񱩻�
    public void ShowJumpNumber(GameObject behitGameObject, float number, bool crit)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }
        // ʹ��Ŀ�������λ��
        Vector3 worldPosition = behitGameObject.transform.position;
        //��x������ƫ��һ��
        worldPosition.x += Random.Range(-1f, 1f);
        //��������ת��Ļ����
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        //ʵ����Ԥ����
        GameObject numberInstance = Instantiate(Number, screenPosition, behitGameObject.transform.rotation);
        numberInstance.transform.position = screenPosition;
        //���һ�����λ��
        GameObject Canvas = GameObject.Find("Canvas");
        numberInstance.gameObject.transform.SetParent(Canvas.transform);
        //�������͵�numberת��Ϊ����
        number = (int)number;
        // ����Ϊ��㣬��ֹ��һ���ﵲס����
        numberInstance.transform.SetAsLastSibling();
        numberInstance.GetComponent<Text>().text = number.ToString();
        //�����Ƿ񱩻�
        Color color = Color.white;
        //���������С
        Number.GetComponent<Text>().fontSize = 25;
        if (crit) 
        {
            //����������ɫ
            color = Color.red;
            //���������С
            Number.GetComponent<Text>().fontSize = 50;
        }
        numberInstance.GetComponent<Text>().color = color;
        // ʹ�� DOTween ��number�����ƶ���Y=0�ƶ���y=800,Ȼ������
        //����һ��������Χ//OnComplete()�Ƕ�����ɺ�Ļص�����
        int jumpfloat = Random.Range(0, 100);
        numberInstance.transform.DOMoveY(numberInstance.transform.position.y + 100, 0.5f).OnComplete(() => Destroy(numberInstance));
    }
}
