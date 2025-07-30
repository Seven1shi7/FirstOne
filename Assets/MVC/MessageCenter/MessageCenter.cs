using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//消息中心
public class MessageCenter
{
    private static MessageCenter instance;

    public static MessageCenter Instance
    {
        get
        {
            if (instance == null)
                instance = new MessageCenter();
            return instance;
        }
    }


    //存储消息中心里面的方法
    Dictionary<MessageType, Action<ModelBase>> MessageDic = new Dictionary<MessageType, Action<ModelBase>>();


    //存储/注册 消息
    public void RegisterMessage(MessageType messageType,Action<ModelBase> action)
    {
        if (MessageDic.ContainsKey(messageType))
        {
            MessageDic[messageType] += action;
        }
        else
        {
            MessageDic.Add(messageType, action);
        }
    }

    //删除消息
    public void RemoveMessage(MessageType messageType,Action<ModelBase> action)
    {
        if (MessageDic.ContainsKey(messageType))
        {
            MessageDic[messageType] -= action;

            if (MessageDic[messageType] == null)
            {
                MessageDic.Remove(messageType);
            }
        }
    }

    //调用/发送 消息
    public void SendMessage(MessageType messageType,ModelBase modelBase)
    {
        if (MessageDic.ContainsKey(messageType))
        {
            MessageDic[messageType](modelBase);
        }
    }



}

public enum MessageType
{
    GoldChange, //金币修改消息
}

