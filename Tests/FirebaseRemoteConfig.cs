using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public class FirebaseRemoteConfig : BaseFirebaseRemoteConfig
{
    protected override Dictionary<string, object> GetDefaultConfig()
    {
        return null;
    }

    protected override void UpdateData(Firebase.RemoteConfig.FirebaseRemoteConfig firebaseRemoteConfig)
    {

    }
}
