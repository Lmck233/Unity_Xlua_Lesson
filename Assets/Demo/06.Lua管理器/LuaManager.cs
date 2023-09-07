using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XLua;

[Hotfix()]
public class LuaManager : MonoBehaviour
{
    void Start()
    {

        // AAA();
        LuaMgr.Instance.Init();
        
        LuaMgr.Instance.Dostring("luaMain.lua");

        // AAA();
        // BBB("sasas");
        //
        //
        // Hotfixtest test = new Hotfixtest();
        //
        // test.Speak("aaaaaa");

        //StartCoroutine(testCoroutine());

        
        // Hotfixtest test = new Hotfixtest();
        //
        // test.myevent?.Invoke();

        // test.Age = 199;
        // Debug.Log(test.Age);
        //
        // test[1] = 5;
        // Debug.Log(test[1]);

        // HotfixTest<string> sss = new HotfixTest<string>();
        // sss.Test("ssasa");
        //
        // HotfixTest<int> sss1 = new HotfixTest<int>();
        // sss1.Test(11111);
        
    }

    IEnumerator testCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("携程打印一次");
        }
    }
    

    private void Update()
    {
        
    }

    public void AAA()
    {
        Debug.Log("c# 原生");
    }
    
    public void BBB(string aaa)
    {
        Debug.Log(aaa);
    }
    
}


[Hotfix]
public class HotfixTest<T>
{
    public void Test(T t)
    {
        Debug.Log(t);
    }
}


[Hotfix()]
public class Hotfixtest
{
    public  Action myevent;
    
    
    public int[] array = new[] { 1, 2, 3 };

    //属性
    public int Age
    {
        get
        {
            
            return 0;
        }
        
        set
        {
            
            Debug.Log(value);
        }
    }
    
    //索引
    public int this[int index]
    {
        get
        {
            if (index >=array.Length|| index<0 )
            {
                Debug.Log("索引不正确");
                return 0;
            }
            
            return array[index];
        }
        set
        {
            if (index >=array.Length|| index<0 )
            {
                Debug.Log("索引不正确");
                return;
            }
            array[index] = value;
        }
    }
    
    
    public Hotfixtest()
    {
        Debug.Log("c#原生构造函数");
    }

    public void Speak(string str)
    {
        Debug.Log(str); 
    }
}