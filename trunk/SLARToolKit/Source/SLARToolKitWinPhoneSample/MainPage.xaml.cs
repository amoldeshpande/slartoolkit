#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Windows Phone sample.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2010-2011 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using SLARToolKit;

namespace SLARToolKitWinPhoneSample
{
   public partial class MainPage
   {
      PhotoCamera photoCamera;
      DispatcherTimer dispatcherTimer;
      bool isInitialized;
      bool isDetecting;
      GrayBufferMarkerDetector arDetector;
      byte[] buffer;
      Matrix3D scale;
      Matrix3D viewport;
      Matrix3D centerAtOrigin;
      Matrix3DProjection matrix3DProjection;

      // Constructor
      public MainPage()
      {
         InitializeComponent();
         OverlayRadioButtonChecked(null, null);
#if !DEBUG
         TxtDiag.Visibility = Visibility.Collapsed;
#endif
      }

      protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
      {
         base.OnNavigatedTo(e);

         // Delayed due to Camera init bug in WP71 SDK Beta 2
         // See http://forums.create.msdn.com/forums/p/85830/516843.aspx
         Dispatcher.BeginInvoke(() =>
                                {

                                   // Initialize the webcam
                                   photoCamera = new PhotoCamera();
                                   photoCamera.Initialized += PhotoCameraInitialized;
                                   CameraButtons.ShutterKeyHalfPressed += PhotoCameraButtonHalfPress;
                                   isInitialized = false;
                                   isDetecting = false;

                                   // Fill the Viewport Rectangle with the VideoBrush
                                   var vidBrush = new VideoBrush();
                                   vidBrush.SetSource(photoCamera);
                                   Viewport.Fill = vidBrush;

                                   // Start timer
                                   dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(50)};
                                   dispatcherTimer.Tick += (sender, e1) => Detect();
                                   dispatcherTimer.Start();
                                });
      }

      protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
      {
         // Release resources
         if (dispatcherTimer != null)
         {
            dispatcherTimer.Stop();
            dispatcherTimer = null;
         }
         if (photoCamera != null)
         {
            Viewport.Fill = null;

            photoCamera.Initialized -= PhotoCameraInitialized;
            CameraButtons.ShutterKeyHalfPressed -= PhotoCameraButtonHalfPress;
            photoCamera.Dispose();
         }

         base.OnNavigatedFrom(e);
      }

      private void Detect()
      {
         if (isDetecting || !isInitialized)
         {
            return;
         }

         isDetecting = true;
         var stopwatch = Stopwatch.StartNew();

         try
         {
            // Update buffer size
            var pixelWidth = (int)photoCamera.PreviewResolution.Width;
            var pixelHeight = (int)photoCamera.PreviewResolution.Height;
            if (buffer == null || buffer.Length != pixelWidth * pixelHeight)
            {
               buffer = new byte[pixelWidth * pixelHeight];

               // Create constant transformations instances
               // Center at origin of the 256x256 controls
               centerAtOrigin = Matrix3DFactory.CreateTranslation(-128, -128, 0);
               // Swap the y-axis and scale down by half
               scale = Matrix3DFactory.CreateScale(0.5, -0.5, 0.5);
               // Viewport transformation
               viewport = Matrix3DFactory.CreateViewportTransformation(pixelWidth, pixelHeight);
               matrix3DProjection = new Matrix3DProjection();
               Txt.Projection = matrix3DProjection;
               Img.Projection = matrix3DProjection;
            }

            // Grab snapshot
            photoCamera.GetPreviewBufferY(buffer);

            // Detect
            var dr = arDetector.DetectAllMarkers(buffer, pixelWidth, pixelHeight);

            // Draw the detected squares
            //bitmap.Clear();
            //ViewportOverlay.Source = bitmap;

            // Calculate the projection matrix
            if (dr.HasResults)
            {
               // Calculate the complete transformation matrix based on the first detection result
               var world = centerAtOrigin * scale * dr[0].Transformation;

               // Calculate the final transformation matrix by using the camera projection matrix 
               var m = Matrix3DFactory.CreateViewportProjection(world, Matrix3D.Identity, arDetector.Projection, viewport);

               // Apply the final transformation matrix to the TextBox
               matrix3DProjection.ProjectionMatrix = m;

               //// Draw the detected squares
               //foreach (var r in dr)
               //{
               //   bitmap.DrawQuad((int)r.Square.P1.X, (int)r.Square.P1.Y, (int)r.Square.P2.X, (int)r.Square.P2.Y, (int)r.Square.P3.X, (int)r.Square.P3.Y, (int)r.Square.P4.X, (int)r.Square.P4.Y, Colors.Red);
               //}
            }
         }
         finally
         {
            isDetecting = false;
            stopwatch.Stop();
            TxtDiag.Text = string.Format("{0} ms", stopwatch.ElapsedMilliseconds);
         }
      }

      void PhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
      {
         //  Initialize the Detector
         arDetector = new GrayBufferMarkerDetector();
         // Load the marker pattern. It has 16x16 segments and a width of 80 millimeters
         var marker = Marker.LoadFromResource("data/Marker_SLAR_16x16segments_80width.pat", 16, 16, 80);
         // The perspective projection has the near plane at 1 and the far plane at 4000
         arDetector.Initialize((int)photoCamera.PreviewResolution.Width, (int)photoCamera.PreviewResolution.Height, 1, 4000, marker);

         isInitialized = true;
      }

      void PhotoCameraButtonHalfPress(object sender, EventArgs e)
      {
         if (isInitialized)
         {
            photoCamera.Focus();
         }
      }

      private void OverlayRadioButtonChecked(object sender, RoutedEventArgs e)
      {
         if (Img == null || Txt == null)
         {
            return;
         }

         Img.Visibility = Visibility.Collapsed;
         Txt.Visibility = Visibility.Collapsed;

         if(RBImage.IsChecked.Value)
         {
            Img.Visibility = Visibility.Visible;
         }
         if (RBText.IsChecked.Value)
         {
            Txt.Visibility = Visibility.Visible;
         }
      }

      private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
      {
         CompositionGrid.RenderTransform = e.Orientation == PageOrientation.LandscapeRight ? new RotateTransform { Angle = 180 } : null;
      }
   }
}