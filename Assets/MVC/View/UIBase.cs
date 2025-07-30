using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIBase
{
    //面板预制体路径
    public string path;
    //面板类型
    public UIType uiType;

    //存储面板实例化出来的预制体
    public GameObject panel;

    public UIBase(string path,UIType uIType)
    {
        this.path = path;
        this.uiType = uIType;
    }

    //初始化
    public virtual void Init(GameObject go)
    {
        this.panel = go;
    }


    public void Destroy()
    {
        GameObject.Destroy(panel);
    }
}

public enum UIType
{
    FIXED,
    NORMAL,
    TIP,
}

