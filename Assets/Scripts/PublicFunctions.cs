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
}
