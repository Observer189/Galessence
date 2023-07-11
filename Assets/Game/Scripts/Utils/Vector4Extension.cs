using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector4Extension 
{
    public static Vector4 SetX(this Vector4 vector, float newValue)
    {
        vector.x = newValue;
        return vector;
    }
    
    public static Vector4 SetY(this Vector4 vector, float newValue)
    {
        vector.y = newValue;
        return vector;
    }
    
    public static Vector4 SetZ(this Vector4 vector, float newValue)
    {
        vector.z = newValue;
        return vector;
    }
    
    public static Vector4 SetW(this Vector4 vector, float newValue)
    {
        vector.w = newValue;
        return vector;
    }
}
