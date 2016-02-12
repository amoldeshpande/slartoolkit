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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using System.Text;

using Matrix = Balder.Math.Matrix;
using Balder.Math;
using Balder.Objects.Geometries;

using SLARToolKit;
using Image = Balder.Imaging.Image;

namespace SLARToolKitBalderSample
{
   /// <summary>
   /// AR Sample using the Silverlight webcam API and the 3D game engine Balder
   /// </summary>
   public partial class MainPage : UserControl
   {
      private CaptureSource captureSource;
      private BitmapMarkerDetector arDetector;
      private bool doAR;
      private float wayInterpol, timedRotation;
      private float sign;
      private Mesh ActiveMesh;
      private List<Mesh> meshs;

      public MainPage()
      {
         InitializeComponent();
      }

      private void Initialize()
      {
         try
         {
            // Init variables
            wayInterpol = 0;
            timedRotation = 0;
            sign = 1;

            // Balder designer bug workarounds
            Game.Viewport.Width = (int)this.ViewportContainer.Width;
            Game.Viewport.Height = (int)this.ViewportContainer.Height;

            Game.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Tank.Color = Balder.Color.FromArgb(255, 0, 50, 0);

            // Init Light
            Light1.Range = Light2.Range = 10;
            Light1.Strength = Light2.Strength = 0.6f;
            Light1.Specular = Light2.Specular = Balder.Color.FromArgb(255, 255, 255, 255);

            // Init combobox
            meshs = new List<Mesh>
            {
               this.Car,
               this.Tank,
               this.CubeRounded,
               this.Teapot,
            };
            lock (this.Game.Scene.RenderableNodes)
            {
               this.ActiveMesh = this.Car;
               this.Game.Scene.RenderableNodes.Clear();
               this.Game.Scene.AddNode(ActiveMesh);
            }

            // Init capture source
            captureSource = new CaptureSource
            {
               VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice(),
            };

            // Wiring the detection callback
            captureSource.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(captureSource_CaptureImageCompleted);

            // Desired format is 320 x 240 (good tracking results and performance)
            captureSource.VideoCaptureDevice.DesiredFormat = new VideoFormat(PixelFormatType.Unknown, 320, 240, 30);

            // Start all
            InitializeUpdateMethod();
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

      private void captureSource_CaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
      {
         // Set reflection map dynamically using the webcam snapsot
         if (TeapotMaterial != null)
         {
            var bitmap = e.Result.Resize(512, 512, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            TeapotMaterial.ReflectionMap = new WriteableBitmapMap(bitmap);
         }

         // Perform AR
         DetectMarkers(e.Result);
      }

      private void InitializeUpdateMethod()
      {
         doAR = false;
         DateTime startTime = DateTime.Now;
         int framesCount = 0;
         CompositionTarget.Rendering += (s, ea) =>
         {
            if (doAR)
            {
               // Take webcam snapshot. 
               // CaptureImageAsync calls the event CaptureImageCompleted += (s, e) => DetectMarkers(e.Result);
               captureSource.CaptureImageAsync();
            }
            else
            {
               var trans = Matrix.CreateTranslation(new Vector(0, 0, -400));
               timedRotation += 2;
               trans = Matrix.CreateRotationZ(timedRotation) * trans;
               ApplyFinalTransformation(trans);
            }

            // Update FPS text and reset counter every 30 frames
            if (++framesCount >= 30)
            {
               TimeSpan elapsed = DateTime.Now - startTime;
               Txt.Text = String.Format("{0:###0.00} fps", framesCount / elapsed.TotalSeconds);
               startTime = DateTime.Now;
               framesCount = 0;
            }
         };
      }

      private void InitializeDetector(int width, int height)
      {
         // Load App resources and init AR
         arDetector = new BitmapMarkerDetector();
         var marker = Marker.LoadFromResource("data/Marker_SLAR_16x16segments_80width.pat", 16, 16, 80.0);
         arDetector.Initialize(width, height, Game.Camera.Near, Game.Camera.Far, new List<Marker> {marker});
         //    Camera.CustomProjectionMatrix = arDetector.Projection.ToBalderMatrix();
      }

      private void DetectMarkers(WriteableBitmap bmp)
      {
         // Init. here because the captureSource.VideoCaptureDevice.DesiredFormat getter is not reliable and throws an Exception
         if (arDetector == null)
         {
            InitializeDetector(bmp.PixelWidth, bmp.PixelHeight);
         }

         // Detect
         var detectedResults = arDetector.DetectAllMarkers(bmp);

         // Reused for marker highlighting
         bmp.Clear();
         ViewportOverlay.Source = bmp;

         if (detectedResults.HasResults)
         {
            var trans = detectedResults[0].Transformation.ToBalderMatrix();

            if (detectedResults.Count > 1)
            {
               // Calculate distance vector
               wayInterpol += (3f * sign);
               var trans2 = detectedResults[1].Transformation.ToBalderMatrix();
               var p1 = new Vector(trans[3, 0], trans[3, 1], trans[3, 2]);
               var p2 = new Vector(trans2[3, 0], trans2[3, 1], trans2[3, 2]);
               var d = p1 - p2;
               var dn = Vector.Normalize(d);
               var p = dn * -wayInterpol;

               // Turning point ?
               if (p.LengthSquared() > d.LengthSquared())
               {
                  sign = -1;
               }
               else if (wayInterpol < 0)
               {
                  sign = 1;
               }
               var translate = Matrix.CreateTranslation(p);

               // Calculate Yaw and align the mesh to it
               var dot = Vector.Dot(Vector.UnitY, dn);
               var rotRad = Math.Acos(dot);
               var rotDeg = MathHelper.ToDegrees((float)rotRad);
               var yaw = Matrix.CreateRotationZ(150 + rotDeg + (sign > 0 ? 0 : 180));

               // Combine matrix
               trans = yaw * trans * translate;
            }
            else
            {
               // If no 2nd marker was found only rotate around z axis
               timedRotation += 2;
               trans = Matrix.CreateRotationZ(timedRotation) * trans;
            }
            // Transform the mesh and change coordinate system
            ApplyFinalTransformation(trans);

            // Highlight detected markers using the  WriteableBitmapEx
            foreach (var r in detectedResults)
            {
               bmp.DrawQuad((int)r.Square.P1.X, (int)r.Square.P1.Y,
                              (int)r.Square.P2.X, (int)r.Square.P2.Y,
                              (int)r.Square.P3.X, (int)r.Square.P3.Y,
                              (int)r.Square.P4.X, (int)r.Square.P4.Y, Colors.Red);
            }
         }
      }

      private void ApplyFinalTransformation(Matrix baseTransformation)
      {
         // Transform the mesh and change coordinate system
         Matrix swapAxis = Matrix.CreateRotationX(90);
         Matrix scale = Matrix.CreateScale(50f);
         var trans = swapAxis * scale * baseTransformation;
         this.ActiveMesh.World = trans;
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
               Video.Fill = vidBrush;

               // Ask user for permission
               if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
               {
                  captureSource.Start();
               }
               this.BtnCapture.Content = "Stop Fun";

               // Start AR
               doAR = true;
               Game.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
               // Stop AR
               doAR = false;

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


      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         Initialize();
      }

      private void BtnCapture_Click(object sender, RoutedEventArgs e)
      {
         StartStopCapture();
      }

      private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (Combo != null && Combo.SelectedValue != null)
         {
            var mesh = meshs[Combo.SelectedIndex];
            lock (this.Game.Scene.RenderableNodes)
            {
               if (this.ActiveMesh != null)
               {
                  this.Game.Scene.RenderableNodes.Remove(this.ActiveMesh);
               }
               this.Game.Scene.AddNode(mesh);
               this.ActiveMesh = mesh;
            }
         }
      }

      private void ChkFlip_Checked(object sender, RoutedEventArgs e)
      {
         this.VideoContainer.RenderTransform = new ScaleTransform { ScaleX = -1, ScaleY = 1 };
         this.Game.RenderTransform = new MatrixTransform { Matrix = System.Windows.Media.Matrix.Identity };
      }

      private void ChkFlip_Unchecked(object sender, RoutedEventArgs e)
      {
         this.Game.RenderTransform = new ScaleTransform { ScaleX = -1, ScaleY = 1 };
         this.VideoContainer.RenderTransform = new MatrixTransform { Matrix = System.Windows.Media.Matrix.Identity };
      }

      private void ChkFullscreen_Checked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Content.IsFullScreen = true;
      }

      private void ChkFullscreen_Unchecked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Content.IsFullScreen = false;
      }
   }
}
