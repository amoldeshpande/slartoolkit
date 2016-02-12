#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Assembly Infos
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-02-24 00:35:56 +0100 (Mi, 24 Feb 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKitSample/Properties/AssemblyInfo.cs $
//   Id:                $Id: AssemblyInfo.cs 48548 2010-02-23 23:35:56Z unknown $
//
//
//   Copyright (c) 2009-2011 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SLARToolKit;

namespace SLARToolKitBalderSampleSL5
{
   public partial class MainPage
   {
      CaptureSource captureSource;
      BitmapMarkerDetector arDetector;
      bool isInit;

      Marker slarMarker;
      Marker lMarker;

      public MainPage()
      {
         InitializeComponent();
         DataContext = this;
      }

      private void UserControlLoaded(object sender, RoutedEventArgs e)
      {
         // Initialize the webcam
         captureSource = new CaptureSource {VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice()};

         // Desired format is 640 x 480 (good tracking results and performance)
         captureSource.VideoCaptureDevice.DesiredFormat = new VideoFormat(PixelFormatType.Unknown, 640, 480, 60);
         captureSource.CaptureImageCompleted += CaptureSourceCaptureImageCompleted;

         // Fill the Viewport Rectangle with the VideoBrush
         var vidBrush = new VideoBrush();
         vidBrush.SetSource(captureSource);
         Viewport.Fill = vidBrush;

         //  Conctruct the Detector
         arDetector = new BitmapMarkerDetector { Threshold = 200, JitteringThreshold = 1 };

         // Load the marker patterns. It has 16x16 segments and a width of 80 millimeters
         slarMarker = Marker.LoadFromResource("data/Marker_SLAR_16x16segments_80width.pat", 16, 16, 80);

         // Capture or transform periodically
         CompositionTarget.Rendering += (s, e2) =>
                                        {
                                           if (captureSource.State == CaptureState.Started)
                                           {
                                              captureSource.CaptureImageAsync();
                                           }
                                           else
                                           {
                                              Game.SetWorldMatrix(Balder.Math.Matrix.Identity);
                                           }
                                           if (Game.ParticleSystem.Particles != null && Game.ParticleSystem.Particles.Count > 0)
                                           {
                                           }
                                        };
      }

      private void CaptureSourceCaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
      {
         var writeableBitmap = e.Result;

         // Perform AR
         DetectMarkers(writeableBitmap);
      }

      private void DetectMarkers(WriteableBitmap bmp)
      {
         // Init. here because the captureSource.VideoCaptureDevice.DesiredFormat getter is not reliable and throws an Exception
         if (!isInit)
         {
            // Init AR
            arDetector.Initialize(bmp.PixelWidth, bmp.PixelHeight, Game.Camera.Near, Game.Camera.Far, new List<Marker> {slarMarker});
            isInit = true;
         }

         // Detect
         var dr = arDetector.DetectAllMarkers(bmp);

         // Reused for marker highlighting
         bmp.Clear();
         ViewportOverlay.Source = bmp;

         if (dr.HasResults)
         {
            var transformation = Balder.Math.Matrix.CreateTranslation(0, -5, 0) * Balder.Math.Matrix.CreateRotationX(90) * dr[0].Transformation.ToBalder();
            Game.SetWorldMatrix(transformation);

            // Highlight detected markers
            var txt = String.Empty;
            foreach (var r in dr)
            {
               bmp.DrawQuad((int)r.Square.P1.X, (int)r.Square.P1.Y, (int)r.Square.P2.X, (int)r.Square.P2.Y, (int)r.Square.P3.X, (int)r.Square.P3.Y, (int)r.Square.P4.X, (int)r.Square.P4.Y, Colors.Red);
               txt += String.Format("{0}.Confidence = {1:0.00}   ", r.Marker.Name, r.Confidence);
            }
         }
      }

      private void BtnCaptureClick(object sender, RoutedEventArgs e)
      {
         // Request webcam access and start the capturing
         if (CaptureDeviceConfiguration.RequestDeviceAccess())
         {
            captureSource.Start();
            Camera.Position.Y = 0;
         }
      }

      private void ChkFlipChecked(object sender, RoutedEventArgs e)
      {
         ViewportContainer.RenderTransform = new ScaleTransform { ScaleX = -1, ScaleY = 1 };
      }

      private void ChkFlipUnchecked(object sender, RoutedEventArgs e)
      {
         ViewportContainer.RenderTransform = new MatrixTransform { Matrix = Matrix.Identity };
      }

      private void ChkFullscreenChecked(object sender, RoutedEventArgs e)
      {
         Application.Current.Host.Content.IsFullScreen = true;
      }

      private void ChkFullscreenUnchecked(object sender, RoutedEventArgs e)
      {
         Application.Current.Host.Content.IsFullScreen = false;
      }
   }
}
