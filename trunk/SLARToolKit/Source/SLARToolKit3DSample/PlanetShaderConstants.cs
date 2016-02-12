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

using Microsoft.Xna.Framework;

namespace SLARToolKit3DSample
{

   // c0: WorldMatrix = Transform
   // c4: WorldInverseTransposeMatrix = WorldInverseTransposeMatrix
   // c8: WorldViewProjectionMatrix = Transform * camera.ViewTransform * camera.ProjectionTransform
   // c12: ViewInverseMatrix = Matrix.Invert(camera.ViewTransform));
   // c16: TotalSeconds = (float)time.Seconds + (float)time.Milliseconds/1000.0f;
   // c17: LightPos
   public struct PlanetShaderConstants
   {
      public Matrix WorldMatrix;
      public Matrix WorldInverseTransposeMatrix;
      public Matrix WorldViewProjectionMatrix;
      public Matrix ViewInverseMatrix;
      public Vector4 TotalSeconds;
      public Vector4 LightPos;
   }
}
