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
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Primitives3D;
using System.Windows;
using Microsoft.Xna.Framework;
using SolarWind;
using PixelShader = Microsoft.Xna.Framework.Graphics.PixelShader;

namespace SLARToolKit3DSample
{
   public class Earth : AstroObject
   {
      PlanetShaderConstants earthConstants;

      SpherePrimitive mesh;

      VertexShader earthVertexShader;
      PixelShader earthPixelShader;

      // Lower atmosphere shaders
      VertexShader atmosphereVertexShader;
      PixelShader cloudsPixelShader;
      PixelShader lowerAtmospherePixelShader;

      // Upper atmosphere shaders
      VertexShader upperAtmosphereVertexShader;
      PixelShader upperAtmospherePixelShader;

      Texture2D dayTexture;
      Texture2D nightTexture;
      Texture2D nightLightsTexture;
      Texture2D normalTexture;
      Texture2D maskTexture;
      Texture2D cloudTexture;
      Texture2D atmosphereTexture;

      RasterizerState ccwState;
      RasterizerState cwState;

      DepthStencilState depthState;
      BlendState cloudBlendState;
      BlendState atmosphereBlendState;

      public bool AtmosphereVisible { get; set; }

      public override bool ShowWireframe
      {
         get { return base.ShowWireframe; }
         set
         {
            base.ShowWireframe = value;
            if (Moon != null)
            {
               Moon.ShowWireframe = value;
            }
         }
      }

      public override Vector3 LightPosition
      {
         get
         {
            return base.LightPosition;
         }
         set
         {
            base.LightPosition = value;
            if (Moon != null)
            {
               Moon.LightPosition = value;
            }
         }
      }

      public Moon Moon { get; set; }

      public override event PropertyChangedEventHandler PropertyChanged;

      public Earth()
      {
         Transform = Matrix.CreateWorld(new Vector3(), Vector3.Forward, Vector3.Up);
         AtmosphereVisible = true;
         Moon = new Moon();
         Scale = 1;
         LightPosition = LightPosition;
         ShowWireframe = ShowWireframe;
      }

      public override void LoadContent()
      {
         // Load mesh
         mesh = new SpherePrimitive(1.0f, 50);

         // Load effects
         earthVertexShader = VertexShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/EarthVS.vs", UriKind.Relative)).Stream);
         earthPixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/EarthPS.ps", UriKind.Relative)).Stream);
         atmosphereVertexShader = VertexShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/AtmosphereVS.vs", UriKind.Relative)).Stream);
         cloudsPixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/CloudsPS.ps", UriKind.Relative)).Stream);
         lowerAtmospherePixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/AtmosphereLowerPS.ps", UriKind.Relative)).Stream);
         upperAtmospherePixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/AtmosphereUpperPS.ps", UriKind.Relative)).Stream);
         upperAtmosphereVertexShader = VertexShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/AtmosphereUpperVS.vs", UriKind.Relative)).Stream);

         // Load textures
         atmosphereTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthAtmosphere.png");
         cloudTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthClouds.png");
         dayTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthDay.jpg");
         maskTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthMask.png");
         nightTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthNight.png");
         nightLightsTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthNightLights.png");
         normalTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Earth/EarthNormal.jpg");

         // Set initial state
         depthState = new DepthStencilState
         {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
            DepthBufferFunction = CompareFunction.LessEqual
         };

         cloudBlendState = new BlendState()
         {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One
         };

         atmosphereBlendState = new BlendState()
         {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One
         };

         // Load Moon data 
         Moon.LoadContent();
      }

      public override void Draw(GraphicsDevice gd, TimeSpan time, Camera camera)
      {
         if(!IsVisible)
         {
            return;
         }

         cwState = new RasterizerState
         {
            CullMode = CullMode.CullClockwiseFace,
            FillMode = ShowWireframe ? FillMode.WireFrame : FillMode.Solid,
         };

         ccwState = new RasterizerState
         {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = ShowWireframe ? FillMode.WireFrame : FillMode.Solid,
         };

         // Pass 1 - Draw Earth

         // s0: DayTexture = EarthDay
         // s1: NightTexture = EarthNight
         // s2: NightLightsTexture = EarthNightLights
         // s3: NormalTexture = EarthNormal
         // s4: MaskTexture = EarthMask
         gd.Textures[0] = dayTexture;
         gd.Textures[1] = nightTexture;
         gd.Textures[2] = nightLightsTexture;
         gd.Textures[3] = normalTexture;
         gd.Textures[4] = maskTexture;

         var worldMatrix = Transform;
         worldMatrix = Matrix.CreateScale((float) Scale) * worldMatrix;
         earthConstants.WorldMatrix = worldMatrix;
         earthConstants.WorldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(worldMatrix));
         earthConstants.WorldViewProjectionMatrix = worldMatrix * camera.ViewTransform * camera.ProjectionTransform;
         earthConstants.ViewInverseMatrix = Matrix.Invert(camera.ViewTransform);
         earthConstants.TotalSeconds.X = (float)time.TotalSeconds;
         earthConstants.LightPos = new Vector4(LightPosition, 1);

         gd.SetVertexShaderConstantFloat4(0, ref earthConstants);

         var showWireframe = new Vector4(ShowWireframe ? 1.0f : 0.0f, 0f, 0f, 0f);
         gd.SetPixelShaderConstantFloat4(0, ref showWireframe);

         gd.SetVertexShader(earthVertexShader);
         gd.SetPixelShader(earthPixelShader);

         gd.DepthStencilState = depthState;
         gd.BlendState = BlendState.Opaque;
         gd.RasterizerState = ccwState;

         mesh.Draw(gd);

         if (AtmosphereVisible)
         {
            //
            // Pass 2 - Draw clouds
            //

            // s0: CloudTexture = EarthClouds
            gd.Textures[0] = cloudTexture;
            gd.SetVertexShader(atmosphereVertexShader);
            gd.SetPixelShader(cloudsPixelShader);
            gd.BlendState = cloudBlendState;

            mesh.Draw(gd);

            //
            // Pass 3 - Lower atmosphere
            //                

            // s0: AtmosphereTexture = EarthAtmosphere
            gd.Textures[0] = atmosphereTexture;
            gd.SetPixelShader(lowerAtmospherePixelShader);
            gd.BlendState = atmosphereBlendState;

            mesh.Draw(gd);

            //
            // Pass 4 - Upper atmosphere
            //
            gd.SetVertexShader(upperAtmosphereVertexShader);
            gd.SetPixelShader(upperAtmospherePixelShader);
            gd.RasterizerState = cwState;

            mesh.Draw(gd);
         }

         // Rotate and draw Moon
         var transform = Matrix.CreateTranslation(0, 0, 1f);
         transform *= Matrix.CreateRotationY((float)time.TotalSeconds * 0.05f);
         transform *= worldMatrix;
         Moon.Transform = transform;
         Moon.Draw(gd, time, camera);
      }
   }
}
