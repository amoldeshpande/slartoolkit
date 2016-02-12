using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPVideoCapture;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SLARToolKit
{
    internal class UWPMediaSink
    {
        private CaptureSourceMarkerDetector detector;
        private long frameCounter;
        private UWPVideoCaptureHelper uwpCapture;
        private StreamSocketListener listener = new StreamSocketListener();
        
        /// <summary>
        /// If true, the detection will be called multi-threaded.
        /// </summary>
        public bool IsMultithreaded { get { return true; } }

        /// <summary>
        /// Initializes a new DetectorVideoSink.
        /// </summary>
        /// <param name="detector">The CaptureSourceMarkerDetector to use.</param>
        public UWPMediaSink(CaptureSourceMarkerDetector detector)
        {
            this.detector = detector;
            listener.ConnectionReceived += Listener_ConnectionReceived;
        }

        public async Task<bool> Start(Panel panel, int width,int height, int frameRate)
        {
            try
            {                
                MediaCaptureInitializationSettings mediaInitSettings = await getMediaCaptureSettingsAsync(panel, width, height, frameRate);
                uwpCapture = new UWPVideoCaptureHelper();
                int port = await getLocalPort();
                if(port == 0)
                {
                    return false;
                }
               return  await uwpCapture.Start(mediaInitSettings, width, height, port);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Could not find device " + ex.ToString());
                return false;
            }
        }

        private async Task<int> getLocalPort()
        {
            int tries = 0;
            int port = 25;
            Random rand = new Random((int)DateTime.Now.Ticks);
            while (tries < 10)
            {
                try
                {
                    tries++;
                    await listener.BindEndpointAsync(new HostName("127.0.0.1"),port.ToString());
                    return port;
                }
                catch
                {
                    port = rand.Next(1, 65535);
                }
            }
            return 0;
        }

        private    async Task<MediaCaptureInitializationSettings> getMediaCaptureSettingsAsync(Panel panel, int width, int height, int frameRate)
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
            MediaCaptureInitializationSettings mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = deviceId,
                                                                                                            StreamingCaptureMode = StreamingCaptureMode.Video };
            if (deviceId != String.Empty)
            {
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
                    // Could not locate rofile, use default video recording profile (or none if device not found)
                    mediaInitSettings.VideoProfile =  profiles[0];
                }
            }            
            return mediaInitSettings;
        }
        /// <summary>
        /// Invoked when a video device starts capturing video data.
        /// </summary>
        protected void OnCaptureStarted()
        {
            frameCounter = 0;
            detector.Start();
        }

        /// <summary>
        /// Invoked when a video device stops capturing video data.
        /// </summary>
        protected  void OnCaptureStopped()
        {
        }
        
        /// <summary>
        /// Invoked when a video device captures a complete video sample / frame.
        /// </summary>
        /// <param name="sampleData">A byte stream containing video data, to be interpreted per the relevant video format information.</param>
        protected void OnSample(byte[] sampleData)
        {            
            ExecuteDetection(sampleData, frameCounter);            
            frameCounter++;
        }

        /// <summary>
        /// Executes the detection
        /// </summary>
        /// <param name="sampleData">The sample data form the webcam.</param>
        /// <param name="frameNumber">The current frame number.</param>
        private void ExecuteDetection(byte[] sampleData, long frameNumber)
        {
            //// Copy buffer to get rid of the negative stride
            //int h = vidFormat.PixelHeight;
            //int s = vidFormat.Stride;
            //int offset, sp;
            //byte[] buffer = new byte[vidFormat.PixelWidth * h << 2];
            //if (s < 0)
            //{
            //    for (int y = 0; y < h; y++)
            //    {
            //        sp = -s;
            //        offset = sp * (h - (y + 1));
            //        Buffer.BlockCopy(sampleData, offset, buffer, sp * y, sp);
            //    }
            //}
            //else
            //{
            //    for (int y = 0; y < h; y++)
            //    {
            //        offset = s * y;
            //        Buffer.BlockCopy(sampleData, offset, buffer, offset, s);
            //    }
            //}

            //// Call detector
            //detector.DetectAllMarkers(buffer, frameNumber);
            detector.DetectAllMarkers(sampleData);
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            OnCaptureStarted();
            DataReader reader = new DataReader(args.Socket.InputStream);
            reader.ByteOrder = ByteOrder.LittleEndian; //WTF Microsoft ?
            try
            {
                while (true)
                {
                    uwpCapture.GetFrame();
                    // Read first 4 bytes (length of the subsequent data). 
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        OnCaptureStopped();
                        return;
                    }
                    int actualLength = reader.ReadInt32();
                    byte[] data = new byte[actualLength];
                    sizeFieldCount = await reader.LoadAsync((uint)actualLength);
                    reader.ReadBytes(data);
                    OnSample(data);
                }
            }
            catch 
            {
                OnCaptureStopped();
            }
        }

    }
}
