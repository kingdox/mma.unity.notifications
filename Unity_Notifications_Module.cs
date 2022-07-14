#region Access
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
        public static string SetChannel = "Unity_Notification_SetChannel";
        public static string SetNotification = "Unity_Notification_SetNotification";
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
        private readonly Dictionary<string, int> dic_notificationSheduled = new Dictionary<string, int>();

        #endregion
        #region Reactions ( On___ )
        // Contenedor de toda las reacciones del Unity_Notifications
        protected override void OnSubscription(bool condition)
        {
            // Initialize
            Middleware<bool>.Subscribe_Publish(condition, Key.Initialize, Initialize);

            // AddChannel
            Middleware<(string id, string title, string description, int importance)>.Subscribe_Publish(condition, Key.SetChannel, SetChannel);

            // AddNotification
            Middleware<(string id, string title, string text, DateTime delay)>.Subscribe_Publish(condition, Key.SetNotification, SetNotification);
            Middleware<(string id, string title, string text, DateTime delay, TimeSpan repeatInterval)>.Subscribe_Publish(condition, Key.SetNotification, SetNotification);

            // SendNotification
            Middleware<string>.Subscribe_Publish(condition, Key.SendNotification, SendNotification);
        }
        #endregion
        #region Methods
        // Contenedor de toda la logica del Unity_Notifications
        private bool Initialize()
        {
            return AndroidNotificationCenter.Initialize();
        }

        private void SetChannel((string id, string title, string description, int importance) data)
        {
            if (dic_notificationChannel.ContainsKey(data.id)){
                dic_notificationChannel[data.id] = new AndroidNotificationChannel(data.id, data.title, data.description, (Importance)data.importance);
            }
            else
            {
                dic_notificationChannel.Add(data.id, new AndroidNotificationChannel(data.id, data.title, data.description, (Importance)data.importance));
            }
            //AndroidNotificationCenter.DeleteNotificationChannel TODO
            AndroidNotificationCenter.RegisterNotificationChannel(dic_notificationChannel[data.id]);
        }

        private void SetNotification((string id, string title, string text, DateTime delay) data)
        {
            //Debug.Log($"{nameof(SetNotification)} => {data}");
            if (dic_notification.ContainsKey(data.id)) dic_notification[data.id] = new AndroidNotification(data.title, data.text, data.delay);
            else dic_notification.Add(data.id, new AndroidNotification(data.title, data.text, data.delay));
        }
        private void SetNotification((string id, string title, string text, DateTime delay, TimeSpan repeatInterval) data)
        {
            if (dic_notification.ContainsKey(data.id)) dic_notification[data.id] = new AndroidNotification(data.title, data.text, data.delay, data.repeatInterval);
            else dic_notification.Add(data.id, new AndroidNotification(data.title, data.text, data.delay, data.repeatInterval));
        }

        private void SendNotification(string id)
        {
            if (dic_notificationSheduled.ContainsKey(id))
            {
                if (AndroidNotificationCenter.CheckScheduledNotificationStatus(dic_notificationSheduled[id]) == NotificationStatus.Scheduled)
                {
                    // Replace the currently scheduled notification with a new notification.
                    AndroidNotificationCenter.UpdateScheduledNotification(dic_notificationSheduled[id], dic_notification[id], id);
                }
                else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(dic_notificationSheduled[id]) == NotificationStatus.Delivered)
                {
                    //Remove the notification from the status bar
                    AndroidNotificationCenter.CancelNotification(dic_notificationSheduled[id]);
                }
                else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(dic_notificationSheduled[id]) == NotificationStatus.Unknown)
                {
                    AndroidNotificationCenter.SendNotification(dic_notification[id], id);
                }

            } else {
                dic_notificationSheduled.Add(id, AndroidNotificationCenter.SendNotification(dic_notification[id], id));
            }
        }
#endregion
#region Request ( Coroutines )
        // Contenedor de toda la Esperas de corutinas del Unity_Notifications
#endregion
#region Task ( async )
        // Contenedor de toda la Esperas asincronas del Unity_Notifications
#endregion
    }
}

