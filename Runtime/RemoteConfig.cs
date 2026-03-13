using System;
using System.Collections.Generic;
using System.Reflection;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

namespace DBD.Firebase
{
    public sealed class RemoteConfig : MonoBehaviour
    {
        private event Action<FirebaseRemoteConfig> OnConfigUpdate;

        private int version = 1;

        public void Init(Dictionary<string, object> defaults, int version)
        {
            Debug.LogWarning($"remote config - FirebaseRemoteConfig Init");
            this.version = version;
            SetDefaultData(defaults);

            SetConfigSetting(FetchAndActivate);
        }

        public void SetConfigUpdateListener(Action<FirebaseRemoteConfig> OnConfigUpdate)
        {
            this.OnConfigUpdate = OnConfigUpdate;
        }

        private void FetchAndActivate()
        {
            FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWithOnMainThread(
                task =>
                {
                    Debug.LogWarning($"remote config - Fetch And Activate Data {task.Result} {task.Status}");
                    UpdateDate();
                    FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener +=
                        OnConfigUpdateListener;
                });
        }

        private void OnDestroy()
        {
            FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= OnConfigUpdateListener;
        }

        private void SetConfigSetting(Action OnSetConfigSettingsComplete)
        {
            var configSettings = new ConfigSettings
            {
                MinimumFetchIntervalInMilliseconds = 0
            };

            FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(configSettings)
                .ContinueWithOnMainThread(_ => { OnSetConfigSettingsComplete?.Invoke(); });
        }

        private void SetDefaultData(Dictionary<string, object> defaults)
        {
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                .ContinueWithOnMainThread(task =>
                {
                    Debug.LogWarning($"remote config - Set Default Data {task.Status}");
                    UpdateDate(true);
                });
        }

        private void OnConfigUpdateListener(object sender, ConfigUpdateEventArgs e)
        {
            if (e.Error != RemoteConfigError.None)
            {
                return;
            }

            FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task =>
                {
                    Debug.LogWarning($"remote config - Update Data {task.Result} {task.Status}");
                    if (task.Result)
                    {
                        UpdateDate();
                    }
                });
        }

        private void UpdateDate(bool isSetDefault = false)
        {
            Debug.LogWarning($"remote config - {isSetDefault}");
            // if (!isSetDefault)
            // {
            OnConfigUpdate?.Invoke(FirebaseRemoteConfig.DefaultInstance);
            // }
        }

        public T GetDateRemoteConfig<T>() where T : class, new()
        {
            var allValues = FirebaseRemoteConfig.DefaultInstance.AllValues;

            T t = new T();
            Type type = typeof(T);

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                string key = $"{field.Name}_v{version}";
                if (!allValues.TryGetValue(key, out var value))
                    continue;

                object parsedValue = ConvertValue(value, field.FieldType);

                if (parsedValue != null)
                {
                    field.SetValue(t, parsedValue);
                }
            }

            return t;
        }

        private object ConvertValue(ConfigValue value, Type targetType)
        {
            if (targetType == typeof(bool))
                return value.BooleanValue;

            if (targetType == typeof(int))
                return (int)value.LongValue;

            if (targetType == typeof(long))
                return value.LongValue;

            if (targetType == typeof(float))
                return (float)value.DoubleValue;

            if (targetType == typeof(double))
                return value.DoubleValue;

            if (targetType == typeof(string))
                return value.StringValue;

            return JsonConvert.DeserializeObject(value.StringValue, targetType);
        }
    }
}