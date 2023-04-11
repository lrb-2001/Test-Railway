using System;
using System.Reflection;

namespace FrameModel;

public static class EnumExpand
{
    /// <summary>
    /// 获取枚举字段的返回备注
    /// </summary>
    /// <returns></returns>
    public static string GetTypeName(this Enum value)
    {
        //首先我们获取type类型
        Type type = value.GetType();
        //这里直接存起来方便返回
        string strValue = value.ToString();
        //然后我们是获取字段上面的特性，所以这里是一个
        FieldInfo field = type.GetField(strValue);
        //IsDefined判断类型是不是有这个类型。
        //第二个属性：如果是真会根据继承链找是不是有这个类型
        if (field.IsDefined(typeof(RemarkAttribute), true))
        {
            //GetCustomAttribute获取类型的特性.（这个的时候会去获取之前[User("111")]的类，相当于new,这个就是构造函数）
            //第一个参数是类型
            //第二个:如果是真会看自己的祖先有没有这个类型
            RemarkAttribute attribute = (RemarkAttribute)field.GetCustomAttribute(typeof(RemarkAttribute), true);
            return attribute.GetRemarks();
        }
        else
        {
            return strValue;
        }
    }
}
