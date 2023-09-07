using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using XLua;

public class test2 : MonoBehaviour
{
    private LuaEnv _luaEnv;

    private void Start()
    {
        _luaEnv = new LuaEnv();
        
        _luaEnv.AddLoader(MyLoader);//设置读取文件方法
        
        
        //直接读取文件
        // string str = Resources.Load<TextAsset>("02hello.lua").text;
        // _luaEnv.DoString(str);

        _luaEnv.DoString("require '02hello.lua'");
        
        _luaEnv.Dispose();
    }

    byte[] MyLoader(ref string filName)
    {
        return Resources.Load<TextAsset>(filName).bytes;
    }
    
    
}
