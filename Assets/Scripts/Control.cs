using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // �������˵�����
        //SceneController.Instance.LoadScene("MainMenu");

        // ������Ϸ�������첽��
        SceneController.Instance.LoadSceneAsync("GameScene");

        // ���¼��ص�ǰ����
        //SceneController.Instance.ReloadCurrentScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
