using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPVideoCapture;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Linq;
using Windows.System.Display;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaStreamSinkTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        UWPVideoCaptureHelper capture;
        private DisplayRequest displayRequest = new DisplayRequest();
        private DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
        private DisplayOrientations displayOrientation = DisplayOrientations.Landscape;
        bool capturing = false;

        public MainPage()
        {
            this.InitializeComponent();
            displayOrientation = displayInformation.CurrentOrientation;
            displayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, Object args)
        {
            displayOrientation = sender.CurrentOrientation;
            if (capturing)
            {
                setPreviewRotation();
            }
        }

        private void setPreviewRotation()
        {
            // Calculate which way and how far to rotate the preview
            int rotationDegrees = ConvertDisplayOrientationToDegrees(displayOrientation);
            capture.Rotate(rotationDegrees);
        }
        /// <summary>
        /// Converts the given orientation of the app on the screen to the corresponding rotation in degrees
        /// </summary>
        /// <param name="orientation">The orientation of the app on the screen</param>
        /// <returns>An orientation in degrees</returns>
        private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                    return 90;
                case DisplayOrientations.LandscapeFlipped:
                    return 180;
                case DisplayOrientations.PortraitFlipped:
                    return 270;
                case DisplayOrientations.Landscape:
                default:
                    return 0;
            }
        }
        private async void StartButton_Click(Object sender, RoutedEventArgs e)
        {
            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;
            capture = new UWPVideoCaptureHelper();
            try
            {
                
                MediaCaptureInitializationSettings settings = await GetMediaCaptureSettingsAsync(Windows.Devices.Enumeration.Panel.Back, 
                                                                                                 (int)Preview.Width, 
                                                                                                 (int)Preview.Height, 
                                                                                                 30);
                await listener.BindEndpointAsync(new HostName("127.0.0.1"), "25");
                capturing = true;
                bool result = await capture.Start(settings,(int)Preview.Width,(int)Preview.Height, 25);
                System.Diagnostics.Debug.WriteLine("Capture start returned " + result);                
            }
            catch(System.Runtime.InteropServices.COMException cex)
            {
                var error = cex.HResult;
            }
        }
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            reader.ByteOrder = ByteOrder.LittleEndian; //WTF Microsoft ?
            try
            {
                while (true)
                {
                    capture.GetFrame();
                    // Read first 4 bytes (length of the subsequent string). 
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data. 
                        return;
                    }
                    int actualStringLength = reader.ReadInt32();
                    //System.Diagnostics.Debug.WriteLine("Expecting " + actualStringLength + " bytes from socket");
                    byte[] data = new byte[actualStringLength];
                    sizeFieldCount = await reader.LoadAsync((uint)actualStringLength);
                    reader.ReadBytes(data);
                    //System.Diagnostics.Debug.WriteLine("read " + sizeFieldCount + " bytes from socket");
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
                    {
                        WriteableBitmap bm = new WriteableBitmap((int)Preview.Width, (int)Preview.Height);
                        data.AsBuffer().CopyTo(bm.PixelBuffer);                        
                         Preview.Source = bm;                        
                    });
                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal 
                    // the text back to the UI thread. 
                    //NotifyUserFromAsyncThread(
                    //    String.Format("Received data: bytes {0} frame {1}", actualStringLength, ++frameCounter));
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail. 
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                NotifyUserFromAsyncThread(exception.ToString());
            }
        }
        private void NotifyUserFromAsyncThread(string strMessage)
        {
            var ignore = Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => StatusText.Text = strMessage);
        }
        private async Task<MediaCaptureInitializationSettings> GetMediaCaptureSettingsAsync(Windows.Devices.Enumeration.Panel panel, int width, int height, int frameRate)
        {
            string deviceId = string.Empty;

            // Finds all video capture devices
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            foreach (var device in devices)
            {
                // Check if the device on the requested panel supports Video Profile
                if (MediaCapture.IsVideoProfileSupported(device.Id) && device.EnclosureLocation.Panel == panel)
                {
                    // We've located a device that supports Video Profiles on expected panel
                    deviceId = device.Id;
                    break;
                }
            }
            MediaCaptureInitializationSettings mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = deviceId };
            IReadOnlyList<MediaCaptureVideoProfile> profiles = MediaCapture.FindAllVideoProfiles(deviceId);

            var match = (from profile in profiles
                         from desc in profile.SupportedRecordMediaDescription
                         where desc.Width == width && desc.Height == height && Math.Round(desc.FrameRate) == frameRate
                         select new { profile, desc }).FirstOrDefault();

            if (match != null)
            {
                mediaInitSettings.VideoProfile = match.profile;
                mediaInitSettings.RecordMediaDescription = match.desc;
            }
            else
            {
                // Could not locate rofile, use default video recording profile
                mediaInitSettings.VideoProfile = deviceId == String.Empty ? null : profiles[0];
            }
            return mediaInitSettings;
        }
    }
}
