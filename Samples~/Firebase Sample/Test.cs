using System;
using Newtonsoft.Json;
using UnityEngine;

namespace FirebaseSample
{
    public class Test : MonoBehaviour
    {
        private void OnEnable()
        {
            FirebaseManager.OnRemoteConfigUpdateData += OnRemoteConfigUpdateData;
        }

        private void OnDisable()
        {
            FirebaseManager.OnRemoteConfigUpdateData -= OnRemoteConfigUpdateData;
        }

        private void OnRemoteConfigUpdateData(RemoteConfigData obj)
        {
            Debug.Log($"datdb - RemoteConfigData {JsonConvert.SerializeObject(obj)}");
        }
    }
}