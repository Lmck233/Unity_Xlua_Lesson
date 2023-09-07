using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;
using YooNeed;

public class HotRoot : MonoBehaviour
{
    public EPlayMode Mode;

    public string HostURL;
    public string Version;
    private string packageVersion;
    private string newpackageVersion;//获得的版本号

    public string GamePlayScene;

    [Header("进度条")]
    //界面
    public Text text;

    public Slider slider;
    [Header("确认下载界面")] public Image Image;
    public Text Text2;
    public Button DownButton;

    //用于异步取消
    private CancellationTokenSource token;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private async void Start()
    {
        token = new CancellationTokenSource();
        UIStart();


        //1 初始化
        await InitializeYooAsset();
        //2 获取资源版本
        newpackageVersion = await UpdatePackageVersion();
        //3 更新资源清单
        await UpdatePackageManifest(newpackageVersion);
        //4 资源包下载 
        await Download();
        //5 开始游戏
        await StartGame(GamePlayScene);
    }

    async void UIStart()
    {
        text.text = "连接服务器中.....";
        await SliderChangF();
        
    }


    /// <summary>
    /// YooAsset初始化
    /// </summary>
    async UniTask InitializeYooAsset()
    {
        // 初始化资源系统
        YooAssets.Initialize();

        // 创建默认的资源包
        string packageName = "DefaultPackage";
        var package = YooAssets.TryGetPackage(packageName);
        
        if (package == null)
        {
            package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);
            
            
        }

        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;

        if (Mode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
            await package.InitializeAsync(createParameters).Task;
        }

        // 单机运行模式
        if (Mode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            createParameters.DecryptionServices = new GameDecryptionServices();
            await package.InitializeAsync(createParameters).Task;
        }

        // 联机运行模式
        if (Mode == EPlayMode.HostPlayMode)
        {
            string defaultHostServer = HostURL;
            string fallbackHostServer = HostURL;
            var createParameters = new HostPlayModeParameters();
            createParameters.DecryptionServices = new GameDecryptionServices();
            createParameters.BuildinQueryServices = new GameQueryServices();
            createParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
            createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

            initializationOperation = package.InitializeAsync(createParameters);
            await initializationOperation.Task;

            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
                packageVersion = package.GetPackageVersion();
                Debug.Log($"当前版本号为 : {packageVersion}");
            }
            else
            {
                Debug.LogWarning($"{initializationOperation.Error}");
            }
        }

        // WebGL运行模式
        if (Mode == EPlayMode.WebPlayMode)
        {
            string defaultHostServer = HostURL;
            string fallbackHostServer = HostURL;
            var createParameters = new WebPlayModeParameters();
            createParameters.DecryptionServices = new GameDecryptionServices();
            createParameters.BuildinQueryServices = new GameQueryServices();
            createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            initializationOperation = package.InitializeAsync(createParameters);
            await initializationOperation.Task;

            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
                packageVersion = package.GetPackageVersion();
                Debug.Log($"当前版本号为 : {packageVersion}");
                await SliderChangF();
            }
            else
            {
                Debug.LogWarning($"{initializationOperation.Error}");
            }
        }
    }


    /// <summary>
    /// 获取资源版本
    /// </summary>
    /// <returns></returns>
    private async UniTask<string> UpdatePackageVersion()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageVersionAsync();
        await operation.Task;
        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            newpackageVersion = operation.PackageVersion;
            Debug.Log($"获取到的版本号为 : {newpackageVersion}");

            text.text = "获取资源版本中";
            await SliderChangF();
            return newpackageVersion;
        }
        else
        {
            //更新失败
            Debug.LogWarning("获取版本号失败，请检查服务器连接");
            return null;
        }
    }

    /// <summary>
    /// 获取资源清单
    /// </summary>
    /// <param name="newVersion">最新版本号</param>
    private async UniTask UpdatePackageManifest(string newVersion)
    {
        if (String.IsNullOrEmpty(newVersion))
        {
            return;
        }

        // 更新成功后自动保存版本号，作为下次初始化的版本。
        // 也可以通过operation.SavePackageVersion()方法保存。
        //bool savePackageVersion = true;
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageManifestAsync(newVersion, true);
        await operation.Task;
        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            Debug.Log("更新资源清单成功");
            text.text = "更新资源清单成功";
            await SliderChangF();
        }
        else
        {
            //更新失败
            Debug.LogError("获取清单失败，请检查服务器连接:"+operation.Error);
        }
    }


    /// <summary>
    /// 启动下载器
    /// </summary>
    private async UniTask Download()
    {
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var package = YooAssets.GetPackage("DefaultPackage");
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要下载的资源");
            return;
        }

        Image.gameObject.SetActive(true);

        //需要下载的文件总数和总大小
        int totalDownloadCount = downloader.TotalDownloadCount;
        long totalDownloadBytes = downloader.TotalDownloadBytes;

        Text2.text = $"版本号{packageVersion}—{newpackageVersion} \n" +
                     $"需要下载文件个数：{totalDownloadCount}，下载总大小{Math.Round((double)totalDownloadBytes / (1024 * 1024), 2)} Mb";


        //注册回调方法
        downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
        downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
        downloader.OnDownloadOverCallback = OnDownloadOverFunction;
        downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

        try
        {
            await DownButton.OnClickAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("取消等待按钮"+e);
        }
        


        Image.gameObject.SetActive(false);
        //开启下载
        downloader.BeginDownload();
        await downloader.Task;

        //检测下载结果
        if (downloader.Status == EOperationStatus.Succeed)
        {
            Debug.Log("下载成功");
        }
        else
        {
            Debug.Log("下载失败");
        }
    }

    /// <summary>
    /// 开始进入游戏场景
    /// </summary>
    /// <param name="sceneName"></param>
    async UniTask StartGame(string sceneName)
    {
        var sceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single;
        bool suspendLoad = false;
        SceneOperationHandle handle = YooAssets.LoadSceneAsync(sceneName, sceneMode, suspendLoad);
        await handle.Task;
        Debug.Log($"Scene name is {sceneName}");
    }


    /// <summary>
    /// 开始下载文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="sizeBytes"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnStartDownloadFileFunction(string fileName, long sizeBytes)
    {
        Debug.LogWarningFormat("开始下载：文件名：{0}，文件大小{1}", fileName, sizeBytes);
    }


    /// <summary>
    /// 下载完成
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadOverFunction(bool isSucceed)
    {
        Debug.LogWarning("下载 " + (isSucceed ? "成功" : "失败"));
    }

    /// <summary>
    /// 更新中
    /// </summary>
    /// <param name="totalDownloadCount"></param>
    /// <param name="currentDownloadCount"></param>
    /// <param name="totalDownloadBytes"></param>
    /// <param name="currentDownloadBytes"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadProgressUpdateFunction(int totalDownloadCount, int currentDownloadCount,
        long totalDownloadBytes, long currentDownloadBytes)
    {
        Debug.LogWarningFormat("正在下载中：已下载{1}/{0}，已下载大小{3}/{2}", totalDownloadCount, currentDownloadCount,
            totalDownloadBytes, currentDownloadBytes);

        double currentDownloadMegabytes = Math.Round((double)currentDownloadBytes / (1024 * 1024), 2);
        double totalDownloadMegabytes = Math.Round((double)totalDownloadBytes / (1024 * 1024), 2);

        text.text =
            $"正在下载中：已下载{currentDownloadCount}/{totalDownloadCount}，已下载大小{currentDownloadMegabytes}/{totalDownloadMegabytes} Mb";
        slider.value = (float)currentDownloadBytes / totalDownloadBytes * 100;
    }


    /// <summary>
    /// 下载出错
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="error"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadErrorFunction(string fileName, string error)
    {
        Debug.LogErrorFormat("下载出错：文件名{0}，错误信息{1}", fileName, error);
    }


    async UniTask SliderChangF()
    {
        int count = 0;
        slider.value = 0;

        while (count != 100)
        {
            slider.value = count++;
            await UniTask.Yield();
        }

        //slider.value = 0;
    }


    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        string hostServerIP = HostURL;
        string appVersion = Version;

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
		if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/{appVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/{appVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/CDN/WebGL/{appVersion}";
		else
			return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }


    private void OnDestroy()
    {
        YooAssets.Destroy();
    }
}