using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Messaging;
using Firebase.RemoteConfig;
using UnityEngine;

[RequireComponent(typeof(RemoteConfig))]
public abstract class BaseFirebaseManager<INSTANCE, REMOTE_CONFIG_DATA> : MonoBehaviour
    where INSTANCE : MonoBehaviour
    where REMOTE_CONFIG_DATA : class, new()
{
    public static INSTANCE Instance { get; private set; }

    [SerializeField] private RemoteConfig remoteConfig;

    public REMOTE_CONFIG_DATA RemoteConfigData { get; private set; }
    public bool IsInitialized { get; private set; }

    public static Action<REMOTE_CONFIG_DATA> OnRemoteConfigUpdateData;

    protected virtual void Reset()
    {
        remoteConfig = GetComponent<RemoteConfig>();
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<INSTANCE>();

            CheckAndFixDependencies();

            Transform root = transform.root;
            if (root != transform)
            {
                DontDestroyOnLoad(root);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void OnDestroy()
    {
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    private void CheckAndFixDependencies()
    {
        IsInitialized = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                IsInitialized = true;

                Init();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void Init()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        remoteConfig.Init(GetRemoteConfigDefaultValue());
        remoteConfig.SetConfigUpdateListener(OnRemoteConfigUpdate);
    }

    protected virtual void OnRemoteConfigUpdate(FirebaseRemoteConfig firebaseRemoteConfig)
    {
        RemoteConfigData = remoteConfig.GetDateRemoteConfig<REMOTE_CONFIG_DATA>();
        OnRemoteConfigUpdateData?.Invoke(RemoteConfigData);
    }

    protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
    }

    protected virtual void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
    }

    protected virtual Dictionary<string, object> GetRemoteConfigDefaultValue()
    {
        return new Dictionary<string, object>();
    }

    public virtual void LogEvent(string name, params Parameter[] parameters)
    {
        if (!IsInitialized)
        {
            return;
        }

        FirebaseAnalytics.LogEvent(name, parameters);
    }

    /// <summary>
    /// gọi khi request consent thành công 
    /// </summary>
    public virtual void FirebaseAnalyticsConsent()
    {
        if (!IsInitialized) return;

        Dictionary<ConsentType, ConsentStatus> consentDict = new()
        {
            { ConsentType.AnalyticsStorage, ConsentStatus.Granted },
            { ConsentType.AdStorage, ConsentStatus.Granted },
            { ConsentType.AdUserData, ConsentStatus.Granted },
            { ConsentType.AdPersonalization, ConsentStatus.Granted }
        };
        FirebaseAnalytics.SetConsent(consentDict);
    }

    /// <summary>
    /// ko dùng cho Admob, vì admob tự động log 
    /// </summary>
    /// <param name="adPlatform"></param>
    /// <param name="adFormat"></param>
    /// <param name="revenue"></param>
    /// <param name="adPos"></param>
    public virtual void AdRevenue(string adPlatform, string adFormat, double revenue, string adPos)
    {
        if (!IsInitialized)
        {
            return;
        }

        var impressionParameters = new[]
        {
            new Parameter("ad_platform", adPlatform), // admob, applovin
            new Parameter("ad_format", adFormat), // open_app, interstitial...
            new Parameter("value", revenue),
            new Parameter("currency", "USD"),
            new Parameter("ad_position", adPos) // quang cao tai vi tri nao
        };

        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
}