using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 加载主菜单场景
        //SceneController.Instance.LoadScene("MainMenu");

        // 加载游戏场景（异步）
        SceneController.Instance.LoadSceneAsync("GameScene");

        // 重新加载当前场景
        //SceneController.Instance.ReloadCurrentScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
