using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class test1 : MonoBehaviour
{
    void Start()
    {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world')");
        luaenv.Dispose();
        
    }
}
