using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class test3 : MonoBehaviour
{
    private LuaEnv _luaEnv = new LuaEnv();
    

    private void Start()
    {

        _luaEnv.AddLoader(MyLoader);//设置读取文件方法
    }

    public void UpDataLua()
    {
        // string str = Resources.Load<TextAsset>("03test.lua").text;
        //
        // _luaEnv.DoString(str);
        
        _luaEnv.DoString("require '03test.lua'");
    }
    
    byte[] MyLoader(ref string filName)
    {
        return Resources.Load<TextAsset>(filName).bytes;
    }

    private void OnDisable()
    {
        _luaEnv.Dispose();
    }
}
