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
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Primitives3D;
using System.Windows;
using Microsoft.Xna.Framework;
using SolarWind;

namespace SLARToolKit3DSample
{
   public class Moon : AstroObject
   {
      PlanetShaderConstants shaderConstants;

      SpherePrimitive mesh;
      VertexShader moonVertexShader;
      PixelShader moonPixelShader;
      Texture2D moonTexture;
      Texture2D moonNormalTexture;

      RasterizerState ccwState;
      DepthStencilState depthState;

      public Moon()
      {
         Transform = Matrix.CreateWorld(new Vector3(), Vector3.Forward, Vector3.Up);
      }

      public override void LoadContent()
      {
         // Load mesh
         mesh = new SpherePrimitive(0.2f, 50);

         // Load effects
         moonVertexShader = VertexShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/MoonVS.vs", UriKind.Relative)).Stream);
         moonPixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/MoonPS.ps", UriKind.Relative)).Stream);

         // Load textures
         moonTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Moon/moon.jpg");
         moonNormalTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Moon/moon_normal.jpg");

         // Set initial state
         depthState = new DepthStencilState
         {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
            DepthBufferFunction = CompareFunction.LessEqual
         };
      }

      public override void Draw(GraphicsDevice gd, TimeSpan time, Camera camera)
      {
         if (!IsVisible)
         {
            return;
         }

         ccwState = new RasterizerState
         {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = ShowWireframe ? FillMode.WireFrame : FillMode.Solid,
         };

         // s0: DayTexture = Moon
         // s1: NormalTexture = MoonNormal
         gd.Textures[0] = moonTexture;
         gd.Textures[1] = moonNormalTexture;

         var transform = Transform;
         transform = Matrix.CreateScale((float) Scale) * transform;
         shaderConstants.WorldMatrix = transform;
         shaderConstants.WorldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(transform));
         shaderConstants.WorldViewProjectionMatrix = transform * camera.ViewTransform * camera.ProjectionTransform;
         shaderConstants.ViewInverseMatrix = Matrix.Invert(camera.ViewTransform);
         shaderConstants.TotalSeconds.X = (float)time.TotalSeconds;
         shaderConstants.LightPos = new Vector4(LightPosition, 1);

         gd.SetVertexShaderConstantFloat4(0, ref shaderConstants);

         var showWireframe = new Vector4(ShowWireframe ? 1 : 0);
         gd.SetPixelShaderConstantFloat4(0, ref showWireframe);

         gd.SetVertexShader(moonVertexShader);
         gd.SetPixelShader(moonPixelShader);

         gd.DepthStencilState = depthState;
         gd.BlendState = BlendState.Opaque;
         gd.RasterizerState = ccwState;

         mesh.Draw(gd);
      }
   }
}
