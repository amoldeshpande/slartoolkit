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
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace SLARToolKit3DSample
{
   public class Sun : AstroObject
   {
      PlanetShaderConstants shaderConstants;

      SpherePrimitive mesh;
      VertexShader sunVertexShader;
      PixelShader sunPixelShader;
      PixelShader refractionPixelShader;

      Texture2D gradientTexture;
      Texture2D turbulence2Texture;
      Texture2D turbulence1Texture;
      Texture2D sunTexture;

      RasterizerState ccwState;
      DepthStencilState depthState;

      public Vector3 Position { get { return Transform.Translation; } }
      public Texture2D ReflectionTexture { get; set; }


      public override Vector3 LightPosition
      {
         get
         {
            return Position;
         }
         set { base.LightPosition = value; }
      }

      public Vector2 ScreenSize { get; set; }

      public Sun()
      {
         Transform = Matrix.CreateWorld(new Vector3(), Vector3.Forward, Vector3.Up);
      }

      public override void LoadContent()
      {
         // Load mesh
         mesh = new SpherePrimitive(1.5f, 50);

         // Load effects
         sunVertexShader = VertexShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/SunVS.vs", UriKind.Relative)).Stream);
         sunPixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/SunPS.ps", UriKind.Relative)).Stream);
         refractionPixelShader = PixelShader.FromStream(GraphicsDeviceManager.Current.GraphicsDevice, Application.GetResourceStream(new Uri(@"/SLARToolKit3DSample;component/Shaders/RefractionPS.ps", UriKind.Relative)).Stream);

         // Load textures
         sunTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Sun/Sun.jpg");
         gradientTexture = ContentManager.LoadBitmapAndMipFromResource("Textures/Sun/FireGradient.png");
         turbulence1Texture = ContentManager.LoadBitmapAndMipFromResource("Textures/Sun/Turbulence1.png");
         turbulence2Texture = ContentManager.LoadBitmapAndMipFromResource("Textures/Sun/Turbulence2.png");

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

         
         gd.Textures[0] = sunTexture;
         gd.Textures[1] = turbulence1Texture;
         gd.Textures[2] = turbulence2Texture;
         gd.Textures[3] = gradientTexture;
         
         gd.SamplerStates[1] = SamplerState.AnisotropicWrap;
         gd.SamplerStates[2] = SamplerState.AnisotropicWrap;
         gd.SamplerStates[3] = SamplerState.LinearClamp;


         //
         // Pass 1, Sun map
         //
         var transform = Transform;
         transform = Matrix.CreateScale((float) Scale) * transform;
         shaderConstants.WorldMatrix = transform;
         shaderConstants.WorldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(transform));
         shaderConstants.WorldViewProjectionMatrix = transform * camera.ViewTransform * camera.ProjectionTransform;
         shaderConstants.ViewInverseMatrix = Matrix.Invert(camera.ViewTransform);
         shaderConstants.TotalSeconds.X = (float) time.TotalSeconds;
         shaderConstants.LightPos = new Vector4(LightPosition, 1);
         gd.SetVertexShaderConstantFloat4(0, ref shaderConstants);

         var showWireframe = new Vector4(ShowWireframe ? 1 : 0);
         gd.SetPixelShaderConstantFloat4(0, ref showWireframe);

         gd.SetVertexShader(sunVertexShader);
         gd.SetPixelShader(sunPixelShader);

         gd.DepthStencilState = depthState;
         gd.BlendState = BlendState.Opaque;
         gd.RasterizerState = ccwState;

         // Perform refraction mapping
         if(ReflectionTexture != null)
         {
            gd.Textures[0] = ReflectionTexture;
            gd.SetPixelShader(refractionPixelShader);
         }
         mesh.Draw(gd);
      }
   }
}
