using UnityEngine;

public class TextJumpNumber : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int jumpNumbernum = Random.Range(1, 101);//生成随机数字
            bool crit = jumpNumbernum > 50;//判断是否爆击
            JumpNumber.instance.ShowJumpNumber(gameObject, jumpNumbernum, crit);
        }
    }
}
