using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JumpNumber : MonoBehaviour
{
    public GameObject Number; // 跳字预制体
    private Camera mainCamera;//主摄像机
    //单例模式
    public static JumpNumber instance;
    private void Awake()
    {
        // 单例模式的简单实现，确保JumpNumber在场景中为唯一实例
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // 自动获取主摄像机
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }
    //传入目标物体，跳字数字，是否暴击
    public void ShowJumpNumber(GameObject behitGameObject, float number, bool crit)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }
        // 使用目标物体的位置
        Vector3 worldPosition = behitGameObject.transform.position;
        //将x轴左右偏移一点
        worldPosition.x += Random.Range(-1f, 1f);
        //世界坐标转屏幕坐标
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        //实例化预制体
        GameObject numberInstance = Instantiate(Number, screenPosition, behitGameObject.transform.rotation);
        numberInstance.transform.position = screenPosition;
        //查找画布的位置
        GameObject Canvas = GameObject.Find("Canvas");
        numberInstance.gameObject.transform.SetParent(Canvas.transform);
        //将浮点型的number转换为整型
        number = (int)number;
        // 设置为最顶层，防止玩家或怪物挡住跳字
        numberInstance.transform.SetAsLastSibling();
        numberInstance.GetComponent<Text>().text = number.ToString();
        //区分是否暴击
        Color color = Color.white;
        //设置字体大小
        Number.GetComponent<Text>().fontSize = 25;
        if (crit) 
        {
            //暴击字体颜色
            color = Color.red;
            //设置字体大小
            Number.GetComponent<Text>().fontSize = 50;
        }
        numberInstance.GetComponent<Text>().color = color;
        // 使用 DOTween 让number进行移动从Y=0移动到y=800,然后销毁
        //设置一个浮动范围//OnComplete()是动画完成后的回调函数
        int jumpfloat = Random.Range(0, 100);
        numberInstance.transform.DOMoveY(numberInstance.transform.position.y + 100, 0.5f).OnComplete(() => Destroy(numberInstance));
    }
}
