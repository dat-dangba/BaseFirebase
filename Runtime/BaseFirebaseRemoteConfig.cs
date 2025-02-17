using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public abstract class BaseFirebaseRemoteConfig : MonoBehaviour
{
    public virtual void Init()
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(GetDefaultConfig())
          .ContinueWithOnMainThread(task =>
          {
              SetData();
          });
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += OnConfigUpdateListener;
    }

    private void OnDestroy()
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= OnConfigUpdateListener;
    }

    private void OnConfigUpdateListener(object sender, ConfigUpdateEventArgs e)
    {
        if (e.Error != RemoteConfigError.None)
        {
            return;
        }
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
          .ContinueWithOnMainThread(task =>
          {
              SetData();
          });
    }

    protected virtual void SetData()
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig firebaseRemoteConfig =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
        UpdateData(firebaseRemoteConfig);
    }

    protected abstract Dictionary<string, object> GetDefaultConfig();

    protected abstract void UpdateData(Firebase.RemoteConfig.FirebaseRemoteConfig firebaseRemoteConfig);
}
