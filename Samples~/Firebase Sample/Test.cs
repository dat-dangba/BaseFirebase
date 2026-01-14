using Newtonsoft.Json;
using UnityEngine;

namespace DBD.Firebase.Sample
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