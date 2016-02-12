#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Convert methods.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-07-28 22:16:55 +0200 (Mi, 28 Jul 2010) $
//   Changed in:        $Revision: 53170 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKitBalderSample/Balder/BalderConvert.cs $
//   Id:                $Id: BalderConvert.cs 53170 2010-07-28 20:16:55Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using Balder.Math;

namespace SLARToolKitBalderSampleSL5
{
   /// <summary>
   /// Convert methods.
   /// </summary>
   public static class BalderConvert
   {
      /// <summary>
      /// Convert a Silverlight matrix into a Balder matrix
      /// </summary>
      /// <param name="matrix"></param>
      /// <returns></returns>
      public static Matrix ToBalder(this System.Windows.Media.Media3D.Matrix3D matrix)
      {
         var m = new Matrix
                 {
                    M11 = (float)matrix.M11,
                    M12 = (float)matrix.M12,
                    M13 = (float)matrix.M13,
                    M14 = (float)matrix.M14,

                    M21 = (float)matrix.M21,
                    M22 = (float)matrix.M22,
                    M23 = (float)matrix.M23,
                    M24 = (float)matrix.M24,

                    M31 = (float)matrix.M31,
                    M32 = (float)matrix.M32,
                    M33 = (float)matrix.M33,
                    M34 = (float)matrix.M34,

                    M41 = (float)matrix.OffsetX,
                    M42 = (float)matrix.OffsetY,
                    M43 = (float)matrix.OffsetZ,
                    M44 = (float)matrix.M44
                 };

         return m;
      }

      public static Matrix ToBalder(this Microsoft.Xna.Framework.Matrix matrix)
      {
         var m = new Matrix
                 {
                    M11 = matrix.M11,
                    M12 = matrix.M12,
                    M13 = matrix.M13,
                    M14 = matrix.M14,

                    M21 = matrix.M21,
                    M22 = matrix.M22,
                    M23 = matrix.M23,
                    M24 = matrix.M24,

                    M31 = matrix.M31,
                    M32 = matrix.M32,
                    M33 = matrix.M33,
                    M34 = matrix.M34,

                    M41 = matrix.M41,
                    M42 = matrix.M42,
                    M43 = matrix.M43,
                    M44 = matrix.M44
                 };

         return m;
      }

      public static Microsoft.Xna.Framework.Vector3 ToXna(this Coordinate coordinate)
      {
         return new Microsoft.Xna.Framework.Vector3((float)coordinate.X, (float)coordinate.Y, (float)coordinate.Z);
      }

      public static Coordinate ToBalder(this Microsoft.Xna.Framework.Vector3 vector3)
      {
         return new Coordinate(Convert.ToDouble(vector3.X), Convert.ToDouble(vector3.Y), Convert.ToDouble(vector3.Z));
      }
   }
}
