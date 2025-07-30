using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ModelManager
{
    private static ModelManager instance;

    public static ModelManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ModelManager();
            return instance;
        }
    }

    //存储所有模块
    Dictionary<Type, ModelBase> ModelDic = new Dictionary<Type, ModelBase>();

    public T getModel<T>() where T : ModelBase
    {
        Type type = typeof(T);
        if (ModelDic.ContainsKey(type))
        {
            return ModelDic[type] as T;
        }
        else
        {
            ModelBase modelBase = Activator.CreateInstance(type) as ModelBase;

            //模块初始化
            modelBase.Init();

            ModelDic.Add(type, modelBase);

            return modelBase as T;
        }
    }
}

