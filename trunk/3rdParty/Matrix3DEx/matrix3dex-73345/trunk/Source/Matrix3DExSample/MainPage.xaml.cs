#region Header
//
//   Project:           Matrix3DEx - Silverlight Matrix3D extensions
//   Description:       Sample.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;

namespace Matrix3DExSample
{
   public partial class MainPage : UserControl
   {
      private const int MaxElementCount = 80;
      private const int NearPlane = 1;
      private const int FarPlane = 4000;

      private List<TransformableElement> Elements;
      private TransformableElement SelectedElement;
      private Random rand;
      private double f;

      public double TranslateX { get; set; }
      public double TranslateY { get; set; }
      public double TranslateZ { get; set; }
      public double ScaleX { get; set; }
      public double ScaleY { get; set; }
      public double ScaleZ { get; set; }
      public double RotateX { get; set; }
      public double RotateY { get; set; }
      public double RotateZ { get; set; }
      public double CameraX { get; set; }
      public double CameraY { get; set; }
      public double CameraZ { get; set; }
      public double CameraLookAtX { get; set; }
      public double CameraLookAtY { get; set; }
      public double CameraLookAtZ { get; set; }
      public double FieldOfView { get; set; }

      public MainPage()
      {
         InitializeComponent();
      }

      private void Init()
      {
         // Vars
         TranslateX = TranslateY = TranslateZ = 0;
         ScaleX = ScaleY = ScaleZ = 2;
         RotateX = RotateY = RotateZ = 0;
         CameraX = CameraY = 0;
         CameraZ = -4000;
         CameraLookAtX = CameraLookAtY = 0;
         CameraLookAtZ = 1;
         FieldOfView = 60;
         rand = new Random();

         // Add random elements
         double w = this.Viewport.Width;
         double h = this.Viewport.Height;
         this.Elements = new List<TransformableElement>(MaxElementCount);
         for (int i = 0; i < MaxElementCount; i++)
         {
            var element = new Image
            {
               //CacheMode   = new BitmapCache(),
               Source      = new BitmapImage(new Uri(String.Format("Pics/{0}.jpg", i), UriKind.Relative)),
               Stretch     = Stretch.None,
            };
            element.MouseLeftButtonUp += (s, e) => ElementPicked(s);

            double w2 = w * 2;
            double h2 = h * 2;
            Elements.Add(new TransformableElement
            {
               Element = element,
               PositionX = rand.NextDouble() * w2 * 2 - w2,
               PositionY = rand.NextDouble() * h2 * 2 - h2,
               PositionZ = -rand.Next(NearPlane, FarPlane),
            });
         }

         // Add UIElements to Viewport canvas in the right z-order
         // Silverlight rendering seems not to use a z-buffer
         var sortedUiElems = from e in Elements 
                             orderby e.PositionZ descending 
                             select e.Element;
         sortedUiElems.ToList().ForEach(this.Viewport.Children.Add);


         // Let's go
         this.DataContext = this;
         CompositionTarget.Rendering += (s, e) => Update();
      }

      private void Update()
      {
         // Animation
         if (ChkAnimated.IsChecked.Value)
         {
            CameraZ = -Math.Abs(Math.Sin(f)) * (FarPlane - NearPlane) * 1.5;
            CameraY = Math.Sin(f * 10) * 1000;
            f += 0.008;
         }

         // Create global transformations
         var vw = Viewport.Width;
         var vh = Viewport.Height;
         var invertYAxis      = Matrix3DFactory.CreateScale(1, -1, 1);
         var translate        = Matrix3DFactory.CreateTranslation(TranslateX, TranslateY, TranslateZ);         
         var rotateX          = Matrix3DFactory.CreateRotationX(MathHelper.ToRadians(RotateX));
         var rotateY          = Matrix3DFactory.CreateRotationY(MathHelper.ToRadians(RotateY));
         var rotateZ          = Matrix3DFactory.CreateRotationZ(MathHelper.ToRadians(RotateZ));
         var scale            = Matrix3DFactory.CreateScale(ScaleX, ScaleY, ScaleZ);
         var lookAt           = Matrix3DFactory.CreateLookAtLH(CameraX, CameraY, CameraZ, CameraLookAtX, CameraLookAtY, CameraLookAtZ);
         var viewport         = Matrix3DFactory.CreateViewportTransformation(vw, vh);
         var projectionMatrix = Matrix3D.Identity;
         if (ChkPerspective.IsChecked.Value)
         {
            projectionMatrix = Matrix3DFactory.CreatePerspectiveFieldOfViewLH(MathHelper.ToRadians(FieldOfView), vw / vh, NearPlane, FarPlane);
         }
         else
         {
            projectionMatrix = Matrix3DFactory.CreateOrthographicLH(vw, vh, NearPlane, FarPlane);
         }

         // Transform all elements
         var selectedMatrix = Matrix3D.Identity;
         foreach (var elem in this.Elements)
         {
            // The UIElement
            var e = elem.Element;

            // Create basic transformation matrices
            var centerAtOrigin   = Matrix3DFactory.CreateTranslation(-e.ActualWidth * 0.5, -e.ActualHeight * 0.5, 0);
            var baseTranslate    = Matrix3DFactory.CreateTranslation(elem.PositionX, elem.PositionY, elem.PositionZ);

            // Combine the transformation matrices
            var m = Matrix3D.Identity;
            m = m * centerAtOrigin;
            m = m * invertYAxis;

            // Apply the world transformation to the selected element
            if (elem == SelectedElement)
            {
               m = m * scale;
               m = m * rotateX * rotateY * rotateZ;
               m = m * translate;

               // Should the camera target be fixed at the selected element?
               if (ChkLookAtSelected.IsChecked.Value)
               {
                  lookAt = Matrix3DFactory.CreateLookAtLH(CameraX, CameraY, CameraZ, elem.PositionX, elem.PositionY, elem.PositionZ);
               }
            }

            // Calculate the final view projection matrix
            m = m * baseTranslate;
            m = Matrix3DFactory.CreateViewportProjection(m, lookAt, projectionMatrix, viewport);

            if (elem == SelectedElement)
            {
               selectedMatrix = m;
            }

            // Apply the transformation to the UIElement
            e.Projection = new Matrix3DProjection { ProjectionMatrix = m };
         }

         // Trace
         TxtTrace1.Text = String.Format("{0} Elements. Matrix:\r\n{1}", this.Elements.Count, selectedMatrix.Dump());
      }


      private void ElementPicked(object sender)
      {
         this.SelectedElement = Elements.Where(e => e.Element == sender).FirstOrDefault();
      }

      private void UserControl_Loaded(object sender, RoutedEventArgs e)
      {
         Init();
      }

      private void ChkGPUAccel_Checked(object sender, RoutedEventArgs e)
      {
         // Activate GPU acceleration
         if (this.Elements != null)
         {
            foreach (var el in this.Elements)
            {
               el.Element.CacheMode = new BitmapCache();
            }
         }
      }

      private void ChkGPUAccel_Unchecked(object sender, RoutedEventArgs e)
      {
         // Deactivate GPU acceleration
         if (this.Elements != null)
         {
            foreach (var el in this.Elements)
            {
               el.Element.CacheMode = null;
            }
         }
      } 

      private void ChkGPUVis_Checked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Settings.EnableCacheVisualization = true;
      }

      private void ChkGPUVis_Unchecked(object sender, RoutedEventArgs e)
      {
         App.Current.Host.Settings.EnableCacheVisualization = false;
      }    
   }
}
