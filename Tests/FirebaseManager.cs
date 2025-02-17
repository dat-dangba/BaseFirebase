using System.Collections;
using System.Collections.Generic;
using Teo.AutoReference;
using UnityEngine;

public class FirebaseManager : BaseFirebaseManager<FirebaseManager>
{
    [SerializeField, Get]
    private FirebaseRemoteConfig firebaseRemoteConfig;
}
