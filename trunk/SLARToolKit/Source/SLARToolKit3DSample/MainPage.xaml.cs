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
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SLARToolKit;
using SolarWind;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace SLARToolKit3DSample
{
   public partial class MainPage
   {
      const float MinConfidence = 0.1f;
      static readonly Microsoft.Xna.Framework.Color TransparentColor = new Microsoft.Xna.Framework.Color(0, 0, 0, 0);

      CaptureSource captureSource;
      BitmapMarkerDetector arDetector;
      bool isInit;

      readonly Camera camera;
      readonly double rotationSpeed;
      TimeSpan totalElapsedTime;
      bool contentLoaded;
      Marker slarMarker;
      Marker lMarker;

      public Earth Earth { get; set; }
      public Sun Sun { get; set; }

      public MainPage()
      {
         InitializeComponent();

         totalElapsedTime = TimeSpan.FromSeconds(0.0);
         rotationSpeed = 2.0;

         // Create camera and models
         camera = new Camera((float)ds.Width / (float)ds.Height);
         Earth = new Earth();
         Sun = new Sun { IsVisible = false, Transform = Matrix.CreateTranslation(-4, 0, 0) };

         DataContext = this;
      }

      void DrawingSurfaceLoaded(object sender, RoutedEventArgs e)
      {
         try
         {
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware)
            {
               if  (GraphicsDeviceManager.Current.RenderModeReason == RenderModeReason.SecurityBlocked)
               {
                  throw new SecurityException();
               }
            }

            if (!contentLoaded)
            {
               Earth.LoadContent();
               Sun.LoadContent();
               contentLoaded = true;
            }
         }
         catch (Exception)
         {
            MessageBox.Show(@"Could not initialize the 3D rendering. Please ensure 3D rendering is allowed.

1. Right click on your Silverlight plugin.
2. Go to the permissions tab.
3. Find the domain that hosts the XAP file.
4. Mark the XAP-Domain and click Allow.");
         }
      }

      void DrawingSurfaceDraw(object sender, DrawEventArgs e)
      {
         try
         {
            var scaledDeltaSeconds = e.DeltaTime.TotalSeconds * rotationSpeed;
            totalElapsedTime += TimeSpan.FromSeconds(scaledDeltaSeconds);

            GraphicsDeviceManager.Current.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, TransparentColor, 1.0f, 0);

            Sun.Draw(GraphicsDeviceManager.Current.GraphicsDevice, totalElapsedTime, camera);
            Earth.LightPosition = Sun.Position;
            Earth.Draw(GraphicsDeviceManager.Current.GraphicsDevice, totalElapsedTime, camera);

            e.InvalidateSurface();
         }
         catch (Exception)
         {
            MessageBox.Show(@"Could not initialize the 3D rendering. Please ensure 3D rendering is allowed.

1. Right click on your Silverlight plugin.
2. Go to the permissions tab.
3. Find the domain that hosts the XAP file.
4. Mark the XAP-Domain and click Allow.");
         }
      }

      /// <summary>
      /// Update the camera when the control changes size
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void DrawingSurfaceSizeChanged(object sender, SizeChangedEventArgs e)
      {
         camera.AspectRatio = (float)ds.ActualWidth / (float)ds.ActualHeight;
         Sun.ScreenSize = new Vector2((float)ds.ActualWidth, (float)ds.ActualHeight);
      }

      private void UserControlLoaded(object sender, RoutedEventArgs e)
      {
         // Initialize the webcam
         captureSource = new CaptureSource();
         captureSource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

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
         lMarker = Marker.LoadFromResource("data/Marker_L_16x16segments_80width.pat", 16, 16, 80);
      }

      private void CaptureSourceCaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
      {
         var writeableBitmap = e.Result;

         if (Sun != null)
         {
            if (ChkGlass.IsChecked.Value)
            {
               // Set reflection map dynamically using the webcam snapsot
               var tex = new Texture2D(GraphicsDeviceManager.Current.GraphicsDevice, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, false, SurfaceFormat.Color);
               writeableBitmap.CopyTo(tex);
               Sun.ReflectionTexture = tex;
            }
            else
            {
               Sun.ReflectionTexture = null;
            }
         }

         // Perform AR
         DetectMarkers(writeableBitmap);
      }

      private void InitializeDetector(int width, int height)
      {
         // Init AR
         arDetector.Initialize(width, height, camera.Near, camera.Far, new List<Marker> { slarMarker, lMarker });
         camera.AspectRatio = (float)width / height;
         Sun.ScreenSize = new Vector2(width, height);
      }

      private void DetectMarkers(WriteableBitmap bmp)
      {
         // Init. here because the captureSource.VideoCaptureDevice.DesiredFormat getter is not reliable and throws an Exception
         if (!isInit)
         {
            InitializeDetector(bmp.PixelWidth, bmp.PixelHeight);
            isInit = true;
         }

         // Detect
         var dr = arDetector.DetectAllMarkers(bmp);

         // Reused for marker highlighting
         bmp.Clear();
         ViewportOverlay.Source = bmp;

         if (dr.HasResults)
         {
            // Set camera
            camera.ProjectionTransform = arDetector.Projection.ToXnaMatrix();

            // Transform Sun
            var sunResult = dr.FirstOrDefault(d => d.Marker == lMarker);
            if (sunResult != null && sunResult.Confidence > MinConfidence)
            {
               Transform(Sun, sunResult.Transformation, TxtSun);
            }
            else
            {
               TxtSun.Visibility = Visibility.Collapsed;
               Sun.IsVisible = false;
            }

            // Transform Earth
            var earthResult = dr.FirstOrDefault(d => d.Marker == slarMarker);
            if (sunResult == null && earthResult == null)
            {
               earthResult = dr[0];
            }
            if (earthResult != null && earthResult.Confidence > MinConfidence)
            {
               Transform(Earth, earthResult.Transformation, TxtEarth);
            }
            else
            {
               TxtEarth.Visibility = Visibility.Collapsed;
               Earth.IsVisible = false;
            }

            //// Highlight detected markers
            //var txt = String.Empty;
            //foreach (var r in dr)
            //{
            //   bmp.DrawQuad((int)r.Square.P1.X, (int)r.Square.P1.Y, (int)r.Square.P2.X, (int)r.Square.P2.Y, (int)r.Square.P3.X, (int)r.Square.P3.Y, (int)r.Square.P4.X, (int)r.Square.P4.Y, Colors.Red);
            //   txt += String.Format("{0}.Confidence = {1:0.00}   ", r.Marker.Name, r.Confidence);
            //}
            //Txt.Text = txt;
         }
      }

      private void Transform(AstroObject astroObject, Matrix3D baseTransformation, FrameworkElement txt)
      {
         // Transform Model
         if (astroObject == null)
         {
            return;
         }
         var m = Matrix.Identity;
         m *= Matrix.CreateTranslation(0, 60, 0);
         m *= Matrix.CreateRotationX(Microsoft.Xna.Framework.MathHelper.ToRadians(90));
         m *= baseTransformation.ToXnaMatrix();
         astroObject.Transform = m;
         astroObject.IsVisible = true;

         // Transform FrameworkElement
         // Center at origin of the TextBlock
         var centerAtOrigin = Matrix3DFactory.CreateTranslation(-txt.ActualWidth * 0.5, -txt.ActualHeight * 0.5, 0);
         // Swap the y-axis
         var scale = Matrix3DFactory.CreateScale(1, -1, 1);
         // Move a bit away from the center
         var translation = Matrix3DFactory.CreateTranslation(0, 50, 0);
         // Calculate the complete transformation matrix based on the first detection result
         var world = centerAtOrigin * translation * scale * baseTransformation;

         // Calculate the final transformation matrix by using the camera projection matrix 
         var vp = Matrix3DFactory.CreateViewportTransformation(Viewport.ActualWidth, Viewport.ActualHeight);
         var mp = Matrix3DFactory.CreateViewportProjection(world, Matrix3D.Identity, arDetector.Projection, vp);

         // Apply the final transformation matrix to the TextBox
         txt.Projection = new Matrix3DProjection { ProjectionMatrix = mp };
         txt.Visibility = Visibility.Visible;
      }

      private void BtnCaptureClick(object sender, RoutedEventArgs e)
      {
         // Request webcam access and start the capturing
         if (CaptureDeviceConfiguration.RequestDeviceAccess())
         {
            captureSource.Start();
            Earth.Scale = Sun.Scale = 50;
            Sun.IsVisible = true;
            ArCtrls.Visibility = Visibility.Visible;

            // Capture periodically
            CompositionTarget.Rendering += (s, e2) => captureSource.CaptureImageAsync();
         }
      }

      private void ChkFlipChecked(object sender, RoutedEventArgs e)
      {
         ViewportContainer.RenderTransform = new ScaleTransform { ScaleX = -1, ScaleY = 1 };
      }

      private void ChkFlipUnchecked(object sender, RoutedEventArgs e)
      {
         ViewportContainer.RenderTransform = new MatrixTransform { Matrix = System.Windows.Media.Matrix.Identity };
      }

      private void ChkFullscreenChecked(object sender, RoutedEventArgs e)
      {
         Application.Current.Host.Content.IsFullScreen = true;
      }

      private void ChkFullscreenUnchecked(object sender, RoutedEventArgs e)
      {
         Application.Current.Host.Content.IsFullScreen = false;
      }

      private void ChkWireframeChecked(object sender, RoutedEventArgs e)
      {
         if (Sun != null)
         {
            Sun.ShowWireframe = true;
         }
         if (Earth != null)
         {
            Earth.ShowWireframe = true;
         }
      }

      private void ChkWireframeUnchecked(object sender, RoutedEventArgs e)
      {
         if (Sun != null)
         {
            Sun.ShowWireframe = false;
         }
         if (Earth != null)
         {
            Earth.ShowWireframe = false;
         }
      }
   }
}
