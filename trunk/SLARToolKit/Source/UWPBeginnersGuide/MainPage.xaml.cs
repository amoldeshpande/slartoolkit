using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SLARToolKit;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Media3D;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPBeginnersGuide
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CaptureSourceMarkerDetector arDetector;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            
        }

        private async Task InitializeDetector()
        {
            //  Initialize the Detector
            arDetector = new CaptureSourceMarkerDetector();

            // Load the marker pattern. It has 16x16 segments and a width of 80 millimeters
            var marker = await Marker.LoadFromResource("ms-appx:///Marker_SLAR_16x16segments_80width.pat", 16, 16, 80);

            // The perspective projection has the near plane at 1 and the far plane at 4000
            await arDetector.Initialize(1, 4000, new List<Marker> { marker },
                Windows.Devices.Enumeration.Panel.Back,
                640,
                480,
                30,
                adaptive.IsChecked.Value);

            // Attach the AR detection event handler
            // The event is fired if at least one marker was detected
            arDetector.MarkersDetected += async (s, me) =>
            {
                // Change to UI thread in order to manipulate the text control's projection
               await Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                   () => 
                {
                    WriteableBitmap bm = new WriteableBitmap(640,480);
                    me.Buffer.AsBuffer().CopyTo(bm.PixelBuffer);
                    Viewport.Source = bm;
                    // Calculate the projection matrix
                    var dr = me.DetectionResults;
                    if (dr.HasResults)
                    {
                        // Center at origin of the TextBlock
                        var centerAtOrigin = Matrix3DFactory.CreateTranslation(-Txt.ActualWidth * 0.5, -Txt.ActualHeight * 0.5, 0);
                        // Swap the y-axis and scale down by half
                        var scale = Matrix3DFactory.CreateScale(0.5, -0.5, 0.5);
                        // Calculate the complete transformation matrix based on the first detection result
                        var world = centerAtOrigin * scale * dr[0].Transformation;

                        // Calculate the final transformation matrix by using the camera projection matrix 
                        var vp = Matrix3DFactory.CreateViewportTransformation(Viewport.ActualWidth, Viewport.ActualHeight);
                        var m = Matrix3DFactory.CreateViewportProjection(world, Matrix3D.Identity, arDetector.Projection, vp);

                        // Apply the final transformation matrix to the TextBox
                        Txt.Projection = new Matrix3DProjection { ProjectionMatrix = m };
                    }
                });
            };
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Request webcam access and start the capturing
                if (CaptureDeviceConfiguration.RequestDeviceAccess())
                {
                    arDetector.Start();
                    await InitializeDetector();
                }
            }
            catch
            {

            }
        }
    }
}
