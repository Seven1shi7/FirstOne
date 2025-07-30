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

    //�洢�������  ��Ӧ��UIBase��
    Dictionary<Type, UIBase> panelDic = new Dictionary<Type, UIBase>();
    //�洢�����սڵ� Fixed��normal��tip
    Dictionary<UIType, GameObject> TypeDic = new Dictionary<UIType, GameObject>();

    //�����
    public void Open<T>() where T : UIBase
    {

        Type type = typeof(T);

        //  new UIBase();
        UIBase uIBase = Activator.CreateInstance(type) as UIBase;

        GameObject prefab = Resources.Load<GameObject>(uIBase.path);


        GameObject panel = GameObject.Instantiate(prefab, TypeDic[uIBase.uiType].transform);

        if (uIBase.uiType == UIType.NORMAL)
        {
            //���⴦��
            //�洢Ҫ���ֵ���ɾ����Ԫ��
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


        //��UIBase��panel��ֵ    
        uIBase.Init(panel);

        panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        panel.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        panel.GetComponent<RectTransform>().localScale = Vector3.one;

        panelDic.Add(type, uIBase);
    }

    //�ر����
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
