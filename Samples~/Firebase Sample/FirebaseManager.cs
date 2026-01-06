using System.Collections.Generic;
using Firebase.Messaging;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

namespace FirebaseSample
{
    public class FirebaseManager : BaseFirebaseManager<FirebaseManager, RemoteConfigData>
    {
        protected override Dictionary<string, object> GetRemoteConfigDefaultValue()
        {
            return new Dictionary<string, object>()
            {
                {
                    "test", "1"
                },
                {
                    "test_model", JsonConvert.SerializeObject(new TestModel
                    {
                        x = 1, y = 1, z = 1
                    })
                },
                {
                    "list", JsonConvert.SerializeObject(new List<int>()
                    {
                        1, 2, 3
                    })
                },
                {
                    "list_test_model", JsonConvert.SerializeObject(new List<TestModel>()
                    {
                        new() { x = 2, y = 2, z = 2 },
                        new() { x = 3, y = 3, z = 3 },
                    })
                }
            };
        }

        protected override void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(sender, e);
            // show noti
        }

        protected override void OnRemoteConfigUpdate(FirebaseRemoteConfig firebaseRemoteConfig)
        {
            base.OnRemoteConfigUpdate(firebaseRemoteConfig);
        }
    }
}