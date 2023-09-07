
using UnityEngine;
using XLua;
using YooAsset;

public class LuaMgr 
{
    private static LuaMgr instance;//用private修饰，然后用get方法访问
    
    public static LuaMgr Instance
    {
        get 
        {
            if (instance == null) //空的话就创建一个，在真正需要实例的时候创建
            {
                instance = new LuaMgr();		
            }
            return instance;
        }
    }
    
    
    
    private LuaEnv _luaEnv;



    public LuaTable Global
    {
        get
        {
            return _luaEnv.Global;
        }
    }
    
    
    
    public void Init()
    {
        if (_luaEnv!=null)
        {
            return;
        }

        _luaEnv = new LuaEnv();
        _luaEnv.AddLoader(MyLoader);
    }

    private byte[] MyLoader(ref string filepath)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        RawFileOperationHandle handle = package.LoadRawFileSync(filepath);
        return handle.GetRawFileData();
    }



    
    public void  Dostring(string str)
    {
        if (_luaEnv ==null)
        {
            Debug.Log("未初始化");
            return;
        }
        _luaEnv.DoString($"require '{str}'");
    }
    

    public void Tick()
    {
        if (_luaEnv ==null)
        {
            Debug.Log("未初始化");
            return;
        }
        _luaEnv.Tick();
    }

    public void Dispose()
    {
        if (_luaEnv ==null)
        {
            Debug.Log("未初始化");
            return;
        }
        _luaEnv.Dispose();
        _luaEnv = null;
    }
}
