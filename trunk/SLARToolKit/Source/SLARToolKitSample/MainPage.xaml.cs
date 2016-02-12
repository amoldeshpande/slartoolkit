#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Sample using the Silverlight webcam API
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Linq;
using System.IO;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Text;

using SLARToolKit;

namespace SLARToolKitSample
{
   /// <summary>
   /// AR Sample using the Silverlight webcam API and SL Matrix3D projections
   /// </summary>
   public partial class MainPage : UserControl
   {
      private CaptureSource captureSource;
      private bool doAR;
      private float timedRotation;
      private Marker markerL, markerSlar;
      private Matrix3D projectionMatrix;

      public double Scale { get; set; }
      public double Rotate { get; set; }
      public CaptureSourceMarkerDetector ArDetector { get; set; }

      public MainPage()
      {
         InitializeComponent();
      }

      private void Initialize()
      {
         try
         {
            // Init variables
            timedRotation = 0;
            Scale = 0.4;
            Rotate = 0;

            // Init capture source
            captureSource = new CaptureSource();
            captureSource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
                        
            // Desired format is 320 x 240 (good tracking results and performance)
            captureSource.VideoCaptureDevice.DesiredFormat = new VideoFormat(PixelFormatType.Unknown, 320, 240, 30);

            // Init AR
            markerSlar = Marker.LoadFromResource("data/Marker_SLAR_16x16segments_80width.pat", 16, 16, 80.0, "SLAR");
            markerL = Marker.LoadFromResource("data/Marker_L_16x16segments_80width.pat", 16, 16, 80.0, "L");
            ArDetector = new CaptureSourceMarkerDetector(captureSource, 1, 4000, new List<Marker> { markerSlar, markerL });
            AttachAREvent();

            // Init Rest
            projectionMatrix = Matrix3DFactory.CreatePerspectiveFieldOfViewRH(0.7, ViewportContainer.Width / ViewportContainer.Height, 1, 4000);
            this.DataContext = this;

            // Start all
            SetARObject();
            RunUpdate();
         }
         catch (Exception ex)
         {
            var builder = new StringBuilder();
            foreach (var sf in captureSource.VideoCaptureDevice.SupportedFormats)
            {
               builder.AppendFormat("{0}: {1} x {2} @ {3} fps. Stride: {4}\r\n", sf.PixelFormat, sf.PixelWidth, sf.PixelHeight, sf.FramesPerSecond, sf.Stride);
            }
            MessageBox.Show("Error during initialization. Please make sure a default webcam was set.\r\nSupported formats:\r\n" + builder.ToString() + "\r\n\r\n" + ex.ToString(), "Error during init.", MessageBoxButton.OK);
            throw;
         }
      }

      private void AttachAREvent()
      {
         var dispatcher = Application.Current.RootVisual.Dispatcher;
         ArDetector.MarkersDetected += (s, e) =>
         {
            // Detected
            var detectedResults = e.DetectionResults;
            projectionMatrix = ArDetector.Projection;

            // Since this event is raised in background thread, change to UI thread
            dispatcher.BeginInvoke(() =>
            {
               // Hide it
               this.GrdARContent2.Visibility = System.Windows.Visibility.Collapsed;

               WriteableBitmap markerHightlightBmp = null;
               if (detectedResults.HasResults)
               {
                  // Apply transformation to SL controls
                  ApplyTransformations(detectedResults);

                  // Highlight detected markers
                  string txt = String.Empty;
                  markerHightlightBmp = new WriteableBitmap(e.BufferWidth, e.BufferHeight);
                  foreach (var r in detectedResults)
                  {
                     markerHightlightBmp.DrawQuad((int)r.Square.P1.X, (int)r.Square.P1.Y,
                                                    (int)r.Square.P2.X, (int)r.Square.P2.Y,
                                                    (int)r.Square.P3.X, (int)r.Square.P3.Y,
                                                    (int)r.Square.P4.X, (int)r.Square.P4.Y, Colors.Red);

                     txt += String.Format("{0}.Confidence = {1:0.00}   ", r.Marker.Name, r.Confidence);
                  }
                  Txt.Text = txt;
               }
               this.ViewportOverlay.Source = markerHightlightBmp;
            });
         };
      }

      private void ApplyTransformations(DetectionResults detectedResults)
      {
         // Find L marker in result and transform object
         var resultL = detectedResults.Where(r => r.Marker == markerL).FirstOrDefault();
         if (resultL == null)
         {
            resultL = detectedResults[0];
         }
         ApplyTransformation(GrdARContent1, resultL.Transformation);

         // Find SLAR marker in result
         if (detectedResults.Count > 1)
         {
            var resultSlar = detectedResults.Where(r => r.Marker == markerSlar).FirstOrDefault();
            if (resultSlar == null)
            {
               resultSlar = detectedResults[1];
            }
            ApplyTransformation(GrdARContent2, resultSlar.Transformation);
            this.GrdARContent2.Visibility = System.Windows.Visibility.Visible;
         }
      }

      private void ApplyTransformation(FrameworkElement element, Matrix3D baseTransformation)
      {
         // Create additional transformations
         var centerImageAtOrigin = Matrix3DFactory.CreateTranslation(-element.ActualWidth * 0.5, -element.ActualHeight * 0.5, 0);
         var invertYAxis = Matrix3DFactory.CreateScale(1, -1, 1);
         var viewport = Matrix3DFactory.CreateViewportTransformation(ViewportContainer.ActualWidth, ViewportContainer.ActualHeight);
         var rotate = Matrix3DFactory.CreateRotationZ(MathHelper.ToRadians(Rotate));
         var scale = Matrix3DFactory.CreateScale(Scale);

         // Compose transform
         var m = Matrix3D.Identity;
         m = m * centerImageAtOrigin;
         m = m * invertYAxis;
         m = m * rotate;
         m = m * scale;
         m = m * baseTransformation;
         m = Matrix3DFactory.CreateViewportProjection(m, Matrix3D.Identity, projectionMatrix, viewport);

         // Apply transform
         element.Projection = new Matrix3DProjection { ProjectionMatrix = m };
      }

      private void RunUpdate()
      {
         doAR = false;
         CompositionTarget.Rendering += (s, ea) =>
         {
            if (!doAR)
            {
               // Rotate other AR object
               var trans = Matrix3DFactory.CreateRotationZ(timedRotation);
               trans = trans * Matrix3DFactory.CreateTranslation(0, 0, -400);
               ApplyTransformation(ImgLogo, trans);
            }
            timedRotation += 0.01f;
         };
      }

      private void StartStopCapture()
      {
         try
         {
            // Start capturing
            if (captureSource.State != CaptureState.Started)
            {
               captureSource.Stop();

               // Create video brush and fill the rectangle with it
               VideoBrush vidBrush = new VideoBrush();
               vidBrush.Stretch = Stretch.Uniform;
               vidBrush.SetSource(captureSource);
               WebcamVideo.Fill = vidBrush;

               // Ask user for permission
               if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
               {
                  captureSource.Start();
               }
               this.BtnCapture.Content = "Stop Fun";

               // Start AR
               this.CanvasARContent.Visibility = System.Windows.Visibility.Visible;
               this.CanvasLogo.Visibility = System.Windows.Visibility.Collapsed;
               doAR = true;
            }
            else
            {
               // Stop AR
               doAR = false;
               this.CanvasARContent.Visibility = System.Windows.Visibility.Collapsed;
               this.CanvasLogo.Visibility = System.Windows.Visibility.Visible;

               // Stop capturing
               captureSource.Stop();
               this.BtnCapture.Content = "Start Fun";
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error using webcam", MessageBoxButton.OK);
            throw;
         }
      }

      private void SetARObject()
      {
         if (this.Img != null && this.TxtBox != null && this.Video != null && this.Img2 != null)
         {
            this.Img.Visibility     = System.Windows.Visibility.Collapsed;
            this.Img2.Visibility    = System.Windows.Visibility.Collapsed;
            this.TxtBox.Visibility  = System.Windows.Visibility.Collapsed;
            this.Video.Visibility   = System.Windows.Visibility.Collapsed;
            this.Video.Pause();
         }

         if (Combo != null && Combo.SelectedIndex != -1)
         {
            switch (Combo.SelectedIndex)
            {
               case 0:
                  this.Img.Visibility = System.Windows.Visibility.Visible;
                  this.Img2.Visibility = System.Windows.Visibility.Visible;
                  break;
               case 1:
                  this.TxtBox.Visibility = System.Windows.Visibility.Visible;
                  break;
               case 2:
                  this.Video.Visibility = System.Windows.Visibility.Visible;
                  this.Video.Play();
                  break;
               default:
                  break;
            }
         }
      }

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         Initialize();
      }

      private void BtnCapture_Click(object sender, RoutedEventArgs e)
      {
         StartStopCapture();
      }

      private void ChkFlip_Checked(object sender, RoutedEventArgs e)
      {
         this.ViewportContainer.RenderTransform = new ScaleTransform { ScaleX = -1, ScaleY = 1 };
      }

      private void ChkFlip_Unchecked(object sender, RoutedEventArgs e)
      {
         this.ViewportContainer.RenderTransform = new MatrixTransform { Matrix = System.Windows.Media.Matrix.Identity };
      }

      private void ChkFullscreen_Checked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Content.IsFullScreen = true;
      }

      private void ChkFullscreen_Unchecked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Content.IsFullScreen = false;
      }

      private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         SetARObject();
      }

      private void Video_MediaEnded(object sender, RoutedEventArgs e)
      {
         // Loop
         this.Video.Position = new TimeSpan();
         this.Video.Play();
      }
   }
}