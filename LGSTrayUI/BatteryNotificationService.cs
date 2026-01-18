using LGSTrayPrimitives;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Concurrent;

namespace LGSTrayUI
{
    public class BatteryNotificationService
    {
        private readonly UserSettingsWrapper _userSettings;
        private readonly ConcurrentDictionary<string, DateTimeOffset> _lastLowBatteryNotificationTimes = new();
        private readonly ConcurrentDictionary<string, PowerSupplyStatus> _previousPowerStatus = new();

        public BatteryNotificationService(UserSettingsWrapper userSettings)
        {
            _userSettings = userSettings;
        }

        public void CheckAndNotify(string deviceId, string deviceName, double batteryPercentage, PowerSupplyStatus powerStatus)
        {
            if (batteryPercentage < 0)
            {
                return;
            }

            // Check for low battery notification
            CheckLowBatteryNotification(deviceId, deviceName, batteryPercentage);

            // Check for charging complete notification
            CheckChargingCompleteNotification(deviceId, deviceName, batteryPercentage, powerStatus);

            // Update previous power status
            _previousPowerStatus[deviceId] = powerStatus;
        }

        private void CheckLowBatteryNotification(string deviceId, string deviceName, double batteryPercentage)
        {
            if (!_userSettings.LowBatteryNotificationEnabled)
            {
                return;
            }

            int threshold = _userSettings.NotificationThreshold;

            if (batteryPercentage > threshold)
            {
                return;
            }

            if (!ShouldNotifyLowBattery(deviceId))
            {
                return;
            }

            SendLowBatteryNotification(deviceId, deviceName, batteryPercentage);
        }

        private void CheckChargingCompleteNotification(string deviceId, string deviceName, double batteryPercentage, PowerSupplyStatus currentStatus)
        {
            if (!_userSettings.ChargingCompleteNotificationEnabled)
            {
                return;
            }

            // Get previous status, default to unknown if not tracked yet
            if (!_previousPowerStatus.TryGetValue(deviceId, out var previousStatus))
            {
                return; // First update, don't notify
            }

            // Notify when transitioning from charging to full or not charging (with high battery)
            bool wasCharging = previousStatus == PowerSupplyStatus.POWER_SUPPLY_STATUS_CHARGING;
            bool isNowFull = currentStatus == PowerSupplyStatus.POWER_SUPPLY_STATUS_FULL;
            bool stoppedCharging = currentStatus == PowerSupplyStatus.POWER_SUPPLY_STATUS_NOT_CHARGING ||
                                   currentStatus == PowerSupplyStatus.POWER_SUPPLY_STATUS_DISCHARGING;

            if (wasCharging && (isNowFull || (stoppedCharging && batteryPercentage >= 95)))
            {
                SendChargingCompleteNotification(deviceName, batteryPercentage);
            }
        }

        private bool ShouldNotifyLowBattery(string deviceId)
        {
            var now = DateTimeOffset.Now;
            var cooldown = TimeSpan.FromMinutes(_userSettings.NotificationCooldownMinutes);

            if (_lastLowBatteryNotificationTimes.TryGetValue(deviceId, out var lastTime))
            {
                if (now - lastTime < cooldown)
                {
                    return false;
                }
            }

            return true;
        }

        private void SendLowBatteryNotification(string deviceId, string deviceName, double batteryPercentage)
        {
            _lastLowBatteryNotificationTimes[deviceId] = DateTimeOffset.Now;

            try
            {
                new ToastContentBuilder()
                    .AddText("Low Battery Warning")
                    .AddText($"{deviceName} battery is at {batteryPercentage:F0}%")
                    .Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        private void SendChargingCompleteNotification(string deviceName, double batteryPercentage)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText("Charging Complete")
                    .AddText($"{deviceName} is fully charged ({batteryPercentage:F0}%)")
                    .Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification: {ex.Message}");
            }
        }

        public void ClearNotificationHistory(string deviceId)
        {
            _lastLowBatteryNotificationTimes.TryRemove(deviceId, out _);
            _previousPowerStatus.TryRemove(deviceId, out _);
        }

        public void ClearAllNotificationHistory()
        {
            _lastLowBatteryNotificationTimes.Clear();
            _previousPowerStatus.Clear();
        }
    }
}
