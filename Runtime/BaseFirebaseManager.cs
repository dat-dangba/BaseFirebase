using Firebase;
using Teo.AutoReference;
using UnityEngine;

public class BaseFirebaseManager<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
{
    public bool IsInitialized;
    [Space(20)]
    [SerializeField, Get]
    private FirebaseMessaging firebaseMessaging;
    [SerializeField, Get]
    private FirebaseTracking firebaseTracking;

    public FirebaseMessaging FirebaseMessaging => firebaseMessaging;
    public FirebaseTracking FirebaseTracking => firebaseTracking;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singleton = new(typeof(T).Name);
                    instance = singleton.AddComponent<T>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Transform root = transform.root;
            if (root != transform)
            {
                DontDestroyOnLoad(root);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
            CheckAndFixDependencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CheckAndFixDependencies()
    {
        IsInitialized = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //FirebaseApp app = FirebaseApp.DefaultInstance;

                IsInitialized = true;

                Initialized();
            }
            else
            {
                Debug.LogError(string.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    protected virtual void Initialized()
    {
        firebaseMessaging.Init();
        firebaseTracking.Init();
    }
}
