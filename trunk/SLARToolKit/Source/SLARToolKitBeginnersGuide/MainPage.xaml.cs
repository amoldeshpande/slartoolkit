#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Beginner's Guide.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using SLARToolKit;
using System.Windows.Media.Media3D;

namespace SLARToolKitBeginnersGuide
{
   /// <summary>
   /// MainPage of the Beginner's Guide
   /// </summary>
   public partial class MainPage : UserControl
   {
      CaptureSource captureSource;
      CaptureSourceMarkerDetector arDetector;

      public MainPage()
      {
         InitializeComponent();
      }

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         // Initialize the webcam
         captureSource = new CaptureSource();
         captureSource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

         // Fill the Viewport Rectangle with the VideoBrush
         var vidBrush = new VideoBrush();
         vidBrush.SetSource(captureSource);
         Viewport.Fill = vidBrush;
      }

      private void InitializeDetector()
      {
         //  Initialize the Detector
         arDetector = new CaptureSourceMarkerDetector();
         
         // Load the marker pattern. It has 16x16 segments and a width of 80 millimeters
         var marker = Marker.LoadFromResource("Marker_SLAR_16x16segments_80width.pat", 16, 16, 80);
         
         // The perspective projection has the near plane at 1 and the far plane at 4000
         arDetector.Initialize(captureSource, 1, 4000, new List<Marker> { marker }, adaptive.IsChecked.Value);
         
         // Attach the AR detection event handler
         // The event is fired if at least one marker was detected
         arDetector.MarkersDetected += (s, me) =>
         {
            // Change to UI thread in order to manipulate the text control's projection
            Dispatcher.BeginInvoke(() =>
            {
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


      private void Button_Click(object sender, RoutedEventArgs e)
      {
         // Request webcam access and start the capturing
         if (CaptureDeviceConfiguration.RequestDeviceAccess())
         {
            captureSource.Start();
            InitializeDetector();
         }
      }
   }
}
