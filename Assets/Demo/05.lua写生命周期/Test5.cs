
using System;
using UnityEngine;
using XLua;

public class Test5 : MonoBehaviour
{ 
    static LuaEnv luaEnv = new LuaEnv();
    
    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;
    void Awake()
    {
        scriptEnv = luaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();
        
        
        scriptEnv.Set("self", this);
        
        luaEnv.DoString(MyLoader("05test.lua"), "05test.lua", scriptEnv);
        
        Action luaAwake = scriptEnv.Get<Action>("awake");
        luaAwake?.Invoke();
        
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
    }
    
    byte[] MyLoader( string filName)
    {
        return Resources.Load<TextAsset>(filName).bytes;
    }


    private void Start()
    {
        luaStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        luaUpdate?.Invoke();
    }

    private void OnDestroy()
    {
        luaOnDestroy?.Invoke();
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        
    }
}
