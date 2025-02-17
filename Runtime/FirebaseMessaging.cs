using System.Collections;
using UnityEngine;
using Firebase.Messaging;
using Unity.Notifications;
using System;
using Unity.Notifications.Android;
/*
 * Import Unity.Notications.Android
 * 
 * Window -> Package Manager
 * Search: Mobile Notifications
 * 
 * -----------
 * Setting
 * 
 * Project setting -> Mobile Notification 
 * Tick Use Custom Activity
 * Custom Activity name: com.google.firebase.MessagingUnityPlayerActivity
 */

public class FirebaseMessaging : MonoBehaviour
{
    private readonly string CHANNEL_ID = "channel_id";

    public void Init()
    {
        InitChannel();
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    private void InitChannel()
    {
        var group = new AndroidNotificationChannelGroup()
        {
            Id = "Main",
            Name = "Main notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannelGroup(group);
        var channel = new AndroidNotificationChannel()
        {
            Id = CHANNEL_ID,
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
            Group = "Main",  // must be same as Id of previously registered group

        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void OnDestroy()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {

    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (e == null
            || e.Message == null
            || e.Message.Notification == null
            || e.Message.Notification.Title == null
            || e.Message.Notification.Body == null)
        {
            return;
        }

        //GameNotification gameNotification = GameNotificationsManager.Instance.CreateNotification();
        //gameNotification.Title = e.Message.Notification.Title;
        //gameNotification.Body = e.Message.Notification.Body;
        //GameNotificationsManager.Instance.ScheduleNotification(gameNotification, DateTime.Now.AddSeconds(30));

        //GameNotificationsManager.Instance.ScheduleNotification(
        //    e.Message.Notification.Title,
        //    e.Message.Notification.Body,
        //    DateTime.Now.AddSeconds(30)
        //    );

        ShowNotification(e.Message.Notification.Title,
            e.Message.Notification.Body,
            DateTime.Now.AddMinutes(0.5f));
    }

    public void RequestNotificationPermission()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            StartCoroutine(RequestPermission());
        }
    }

    private IEnumerator RequestPermission()
    {
        yield return new WaitForSeconds(1f);

        var request = NotificationCenter.RequestPermission();
        if (request.Status == NotificationsPermissionStatus.RequestPending)
            yield return request;
    }

    public void ShowNotification(string title, string body, DateTime time, bool autoCancel = true)
    {
        AndroidNotification notification = new()
        {
            Title = title,
            Text = body,
            FireTime = time,
            ShouldAutoCancel = true,
            SmallIcon = "ic_stat_ic_notification"
        };

        AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
    }
}
