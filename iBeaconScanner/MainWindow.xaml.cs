using System;
using System.Windows;

using Windows.Devices.Bluetooth.Advertisement;

namespace iBeaconScanner
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private BluetoothLEAdvertisementWatcher advWatcher;

        public MainWindow()
        {
            InitializeComponent();

            this.advWatcher = new BluetoothLEAdvertisementWatcher();

            // https://blogs.msdn.microsoft.com/shozoa/2016/02/28/windows-10-bluetooth/
            // インターバルがゼロのままだと、CPU負荷が高くなりますので、適切な間隔(SDK サンプルでは 1秒)に指定しないと、アプリの動作に支障をきたすことになります。
            this.advWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(1000);

            // rssi >= -60のときスキャンする
            //this.advWatcher.SignalStrengthFilter.InRangeThresholdInDBm = -60;

            this.advWatcher.Received += this.Watcher_Received;

            // スキャン開始
            this.advWatcher.Start();
        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await this.Dispatcher.InvokeAsync(() => {

                // http://sonic.blue/it/605
                // Windows10デバイスでiBeaconの全データを取得する方法
                iBeacon bcon = new iBeacon(args);
                if (bcon.UUID != null) {
                    // iBeacon
                    DateTimeOffset timestamp = args.Timestamp;
                    string retBeaconData;
                    retBeaconData = "{";
                    retBeaconData += string.Format("vendor:'{0}',", bcon.iBeaconVendor);
                    retBeaconData += string.Format("uuid:'{0}',", bcon.UUID);
                    retBeaconData += string.Format("major:{0},", bcon.Major.ToString("D"));
                    retBeaconData += string.Format("minor:{0},", bcon.Minor.ToString("D"));
                    retBeaconData += string.Format("measuredPower:{0},", bcon.MeasuredPower.ToString("D"));
                    retBeaconData += string.Format("rssi:{0},", bcon.Rssi.ToString("D"));
                    retBeaconData += string.Format("accuracy:{0},", bcon.Accuracy.ToString("F6"));
                    retBeaconData += string.Format("proximity:'{0}',", bcon.Proximity);
                    retBeaconData += string.Format("BluetoothAddress:'{0}',", bcon.BluetoothAddress);
                    retBeaconData += string.Format("RawSignalStrengthInDBm:{0}", bcon.RawSignalStrengthInDBm);
                    retBeaconData += "}";

                    this.textBox.Text = this.textBox.Text + timestamp.ToString("HH\\:mm\\:ss\\.fff") + ":" + retBeaconData + "\r\n";
                }

            });
        }

    }
}
