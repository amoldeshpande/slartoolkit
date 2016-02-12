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
//   Copyright (c) 2009-2011 Microsoft and Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolarWind;

namespace SLARToolKit3DSample
{
   public abstract class AstroObject : INotifyPropertyChanged
   {
      private double scale;
      public virtual double Scale
      {
         get { return scale; }
         set
         {
            if (scale != value)
            {
               scale = value;
               OnPropertyChanged("Scale");
            }
         }
      }

      private Vector3 lightPos;
      public virtual Vector3 LightPosition
      {
         get { return lightPos; }
         set
         {
            if (lightPos != value)
            {
               lightPos = value;
               OnPropertyChanged("LightPosition");
            }
         }
      }

      public bool IsVisible { get; set; }

      public Matrix Transform { get; set; }

      public virtual bool ShowWireframe { get; set; }

      public virtual event PropertyChangedEventHandler PropertyChanged;

      public void OnPropertyChanged(string propertyName)
      {
         var handler = PropertyChanged;
         if (handler != null)
         {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      protected AstroObject()
      {
         Scale = 1;
         LightPosition = Vector3.Zero;
         IsVisible = true;
      }

      public abstract void LoadContent();
      public abstract void Draw(GraphicsDevice gd, TimeSpan time, Camera camera);
   }
}