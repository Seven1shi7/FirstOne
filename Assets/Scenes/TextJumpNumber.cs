using UnityEngine;

public class TextJumpNumber : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int jumpNumbernum = Random.Range(1, 101);//�����������
            bool crit = jumpNumbernum > 50;//�ж��Ƿ񱬻�
            JumpNumber.instance.ShowJumpNumber(gameObject, jumpNumbernum, crit);
        }
    }
}
