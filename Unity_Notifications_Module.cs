﻿#region Access
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
#endregion
namespace MMA.Unity_Notifications
{
    public static class Key
    {
        // public const string _   = KeyData._;
        public static string Initialize = "Unity_Notification_Initialize";
        public static string AddChannel = "Unity_Notification_AddChannel";
        public static string AddNotification = "Unity_Notification_AddNotification";
        public static string SendNotification = "Unity_Notification_SendNotification";
    }
    public static class Import
    {
        //public const string _ = _;
    }
    public sealed partial class Unity_Notifications_Module : Module
    {
        #region References
        //[Header("Applications")]
        //[SerializeField] public ApplicationBase interface_Unity_Notifications;
        private readonly Dictionary<string, AndroidNotificationChannel> dic_notificationChannel = new Dictionary<string, AndroidNotificationChannel>();
        private readonly Dictionary<string, AndroidNotification> dic_notification = new Dictionary<string, AndroidNotification>();

        #endregion
        #region Reactions ( On___ )
        // Contenedor de toda las reacciones del Unity_Notifications
        protected override void OnSubscription(bool condition)
        {
            // Initialize
            Middleware<bool>.Subscribe_Publish(condition, Key.Initialize, Initialize);

            // AddChannel
            Middleware<(string id, string title, string description, int importance)>.Subscribe_Publish(condition, Key.AddChannel, AddChannel);

            // AddNotification
            Middleware<(string id, string title, string text, DateTime delay, TimeSpan repeatInterval)>.Subscribe_Publish(condition, Key.AddNotification, AddNotification);

            // SendNotification
            Middleware<string>.Subscribe_Publish(condition, Key.SendNotification, SendNotification);
        }
        #endregion
        #region Methods
        // Contenedor de toda la logica del Unity_Notifications
        private bool Initialize() => AndroidNotificationCenter.Initialize();

        private void AddChannel((string id, string title, string description, int importance) data)
        {
            dic_notificationChannel.Add(data.id, new AndroidNotificationChannel(data.id, data.title, data.description, (Importance)data.importance));
            AndroidNotificationCenter.RegisterNotificationChannel(dic_notificationChannel[data.id]);
        }

        private void AddNotification((string id, string title, string text, DateTime delay, TimeSpan repeatInterval) data) => dic_notification.Add(data.id, new AndroidNotification(data.title, data.text, data.delay, data.repeatInterval));

        private void SendNotification(string id) => AndroidNotificationCenter.SendNotification(dic_notification[id], id);
        #endregion
        #region Request ( Coroutines )
        // Contenedor de toda la Esperas de corutinas del Unity_Notifications
        #endregion
        #region Task ( async )
        // Contenedor de toda la Esperas asincronas del Unity_Notifications
        #endregion
    }
}


/*
 
 
 if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        {
            // Replace the currently scheduled notification with a new notification.
            AndroidNotificationCenter.UpdateScheduledNotification(identifier, newNotification, channel);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
        {
            //Remove the notification from the status bar
            AndroidNotificationCenter.CancelNotification(identifier);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
        {
            AndroidNotificationCenter.SendNotification(newNotification, "channel_id");
        }
 
 */