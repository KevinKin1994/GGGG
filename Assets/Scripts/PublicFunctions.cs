using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PublicFunctions 
{
    public static int GetEnumLength<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    public static T GetValueFromDict<T,W>(Dictionary<W,string> dict,W key)
    {
        // 键不存在，返回默认值
        if (!dict.TryGetValue(key, out string strValue))
        {
            return default(T);
        }

        Type targetType = typeof(T);

        // 基础数字、bool、字符串转换分支
        if (targetType == typeof(int))
        {
            int.TryParse(strValue, out int res);
            return (T)Convert.ChangeType(res, targetType);
        }
        if (targetType == typeof(float))
        {
            float.TryParse(strValue, out float res);
            return (T)Convert.ChangeType(res, targetType);
        }
        if (targetType == typeof(bool))
        {
            bool.TryParse(strValue, out bool res);
            return (T)Convert.ChangeType(res, targetType);
        }
        if (targetType == typeof(string))
        {
            return (T)Convert.ChangeType(strValue, targetType);
        }

        // 其他类型直接返回默认
        return default(T);
    }
    
    /// <summary>
    /// 修改字典内字符串存储的基础类型数值，自动转换后写回字典
    /// </summary>
    /// <typeparam name="T">要操作的基础类型 int/float/bool/string</typeparam>
    /// <typeparam name="W">字典key类型</typeparam>
    /// <param name="dict">目标字典</param>
    /// <param name="key">目标键</param>
    /// <param name="modifyValue">修改用的值：数字为增量；bool/string为覆盖新值</param>
    public static void ChangeValueForDict<T, W>(Dictionary<T, string> dict, T key, W modifyValue)
    {
        // 键不存在直接退出，不新增键
        if (!dict.TryGetValue(key, out string strRaw))
            return;

        Type typeT = typeof(W);
        string newStrValue = strRaw;

        // 1. int 逻辑：原有数字 + 传入增量
        if (typeT == typeof(int))
        {
            if (int.TryParse(strRaw, out int originNum))
            {
                int addNum = Convert.ToInt32(modifyValue);
                int result = originNum + addNum;
                newStrValue = result.ToString();
            }
        }
        // 2. float 逻辑：原有小数 + 传入增量
        else if (typeT == typeof(float))
        {
            if (float.TryParse(strRaw, out float originNum))
            {
                float addNum = Convert.ToSingle(modifyValue);
                float result = originNum + addNum;
                newStrValue = result.ToString();
            }
        }
        // 3. bool 逻辑：直接覆盖为传入的bool值
        else if (typeT == typeof(bool))
        {
            bool newBool = Convert.ToBoolean(modifyValue);
            newStrValue = newBool.ToString();
        }
        // 4. string 逻辑：直接覆盖文本
        else if (typeT == typeof(string))
        {
            newStrValue = Convert.ToString(modifyValue);
        }

        // 核心：把处理完的新字符串写回字典，完成修改
        dict[key] = newStrValue;
    }
}
