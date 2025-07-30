using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            return instance;
        }
    }

    public UIManager()
    {
        TypeDic.Add(UIType.FIXED, GameObject.Find("Canvas/Fixed"));
        TypeDic.Add(UIType.NORMAL, GameObject.Find("Canvas/Normal"));
        TypeDic.Add(UIType.TIP, GameObject.Find("Canvas/Tip"));
    }

    //存储所有面板  对应的UIBase类
    Dictionary<Type, UIBase> panelDic = new Dictionary<Type, UIBase>();
    //存储三个空节点 Fixed，normal，tip
    Dictionary<UIType, GameObject> TypeDic = new Dictionary<UIType, GameObject>();

    //打开面板
    public void Open<T>() where T : UIBase
    {

        Type type = typeof(T);

        //  new UIBase();
        UIBase uIBase = Activator.CreateInstance(type) as UIBase;

        GameObject prefab = Resources.Load<GameObject>(uIBase.path);


        GameObject panel = GameObject.Instantiate(prefab, TypeDic[uIBase.uiType].transform);

        if (uIBase.uiType == UIType.NORMAL)
        {
            //互斥处理
            //存储要从字典中删除的元素
            List<Type> removeList = new List<Type>();

            foreach (var item in panelDic)
            {
                if (item.Value.uiType == UIType.NORMAL)
                    removeList.Add(item.Key);
            }

            foreach (var item in removeList)
            {
                panelDic[item].Destroy();
                panelDic.Remove(item);
            }
        }


        //给UIBase的panel赋值    
        uIBase.Init(panel);

        panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        panel.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        panel.GetComponent<RectTransform>().localScale = Vector3.one;

        panelDic.Add(type, uIBase);
    }

    //关闭面板
    public void Close<T>() where T :UIBase
    {
        Type type = typeof(T);
        if (panelDic.ContainsKey(type))
        {
            panelDic[type].Destroy();
            panelDic.Remove(type);
        }
    }

    public T getUIPanel<T>() where T : UIBase
    {
        Type type = typeof(T);

        if (panelDic.ContainsKey(type))
        {
            return panelDic[type] as T;
        }
        return null;
    }
}
