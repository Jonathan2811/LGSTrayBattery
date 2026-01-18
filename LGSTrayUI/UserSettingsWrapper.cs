using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Specialized;

namespace LGSTrayUI
{
    public partial class UserSettingsWrapper : ObservableObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "")]
        public StringCollection SelectedDevices => Properties.Settings.Default.SelectedDevices;

        public bool NumericDisplay
        {
            get => Properties.Settings.Default.NumericDisplay;
            set
            {
                Properties.Settings.Default.NumericDisplay = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }

        public bool LowBatteryNotificationEnabled
        {
            get => Properties.Settings.Default.LowBatteryNotificationEnabled;
            set
            {
                Properties.Settings.Default.LowBatteryNotificationEnabled = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }

        public bool ChargingCompleteNotificationEnabled
        {
            get => Properties.Settings.Default.ChargingCompleteNotificationEnabled;
            set
            {
                Properties.Settings.Default.ChargingCompleteNotificationEnabled = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }

        public int NotificationThreshold
        {
            get => Properties.Settings.Default.NotificationThreshold;
            set
            {
                Properties.Settings.Default.NotificationThreshold = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }

        public int NotificationCooldownMinutes
        {
            get => Properties.Settings.Default.NotificationCooldownMinutes;
            set
            {
                Properties.Settings.Default.NotificationCooldownMinutes = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged();
            }
        }

        public void AddDevice(string deviceId)
        {
            if (Properties.Settings.Default.SelectedDevices.Contains(deviceId))
            {
                return;
            }

            Properties.Settings.Default.SelectedDevices.Add(deviceId);
            Properties.Settings.Default.Save();

            OnPropertyChanged(nameof(SelectedDevices));
        }

        public void RemoveDevice(string deviceId)
        {
            Properties.Settings.Default.SelectedDevices.Remove(deviceId);
            Properties.Settings.Default.Save();

            OnPropertyChanged(nameof(SelectedDevices));
        }
    }
}
