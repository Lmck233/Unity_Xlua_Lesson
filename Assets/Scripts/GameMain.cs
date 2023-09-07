using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

[Hotfix()]
public class GameMain : MonoBehaviour
{
    private void Start()
    {
    }

    public void Test(string str)
    {
        Debug.Log(str);
    }
    
    
    
}









#region 调用泛型

public  class Lesson12
{
    public interface ITest
    {
        
    }
    
    public class testFather
    {
        
    }
    
    public class testChild : testFather,ITest
    {
        
    }
    
    public void testfun1<T>(T a,T b) where T: testFather
    {
        Debug.Log("有参数有约束的泛型方法");        
    }
    
    public void testfun2<T>(T a) 
    {
        Debug.Log("有参数 没约束的泛型方法");        
    }
    
    public void testfun3<T>() where T: testFather
    {
        Debug.Log("有约束 没参数的泛型方法");        
    }
    
    public void testfun4<T>(T a) where T: ITest
    {
        Debug.Log("有约束 有参数 约束是接口 的泛型方法");        
    }
    
}

#endregion



#region 系统类型加特性

public static class Lesson10
{
    [CSharpCallLua]
    public static List<Type> CsharpCallLuaList = new List<Type>()
    {
        typeof(UnityAction<float>)
    };
    
    
    [LuaCallCSharp()]   
    public static List<Type> LuaCallCsharpList = new List<Type>()
    {
        typeof(GameObject),
        typeof(Rigidbody)
    };
    
}

#endregion


#region 判空

[LuaCallCSharp()]
public static class Lesson9
{
    public static bool IsNull(this object obj)
    {
        return obj == null;
    }
}

#endregion


public class Lesson8
{
    public int[,] array = new int[2, 3]
    {
        { 1, 2, 3, },
        { 4, 5, 6, }
    };
}


public class Lesson7
{
    //委托
    public UnityAction del;
    public event UnityAction eventAction;

    public void DoEvent()
    {
        eventAction?.Invoke();
    }

    public void ClearEvent()
    {
        eventAction = null;
    }
}


public class Lesson6
{
    public int Calc()
    {
        return 100;
    }

    public int Calc(int a, int b)
    {
        return a + b;
    }

    public int Calc(int a)
    {
        return a;
    }

    public float Calc(float a)
    {
        return a;
    }
}


public class Lesson5
{
    public int RefFun(int a, ref int b, ref int c, int d)
    {
        b = a + d;
        c = a - d;
        return 100;
    }

    public int OutFun(int a, out int b, out int c, int d)
    {
        b = a;
        c = d;
        return 200;
    }

    public int RefAndOutFun(int a, ref int b, out int c)
    {
        b = a * 10;
        c = a * 20;
        return 300;
    }
}


[LuaCallCSharp()]
public static class Tool
{
    public static void Move(this Lesson4 obj)
    {
        Debug.Log("move");
    }
}


public class Lesson4
{
    public string name = "牛爷爷";

    public void Speak(string str)
    {
        Debug.Log(str);
    }

    public static void Eat()
    {
        Debug.Log("吃东西");
    }
}


public class luaToList
{
    public int[] array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, };
    public List<int> List = new List<int>();
    public Dictionary<int, string> dic = new Dictionary<int, string>();
}


public enum MyEnum
{
    a1 = 1,
    a2 = 2,
    a3 = 3
}


public class Test1
{
    public void log(string str)
    {
        Debug.Log(str);
    }
}

namespace test
{
    public class Test1
    {
        public void log(string str)
        {
            Debug.Log(str);
        }
    }
}