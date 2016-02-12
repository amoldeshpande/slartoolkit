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
//   Copyright (c) 2009-2011 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using Microsoft.Xna.Framework;

namespace SLARToolKit
{
   /// <summary>
   /// Convert methods for SL5 XNA-like 3D API.
   /// </summary>
   public static partial class Convert
   {
      /// <summary>
      /// Convert a Silverlight matrix into an Xna matrix
      /// </summary>
      /// <param name="matrix"></param>
      /// <returns></returns>
      public static Matrix ToXnaMatrix(this System.Windows.Media.Media3D.Matrix3D matrix)
      {
         var m = new Matrix(
            (float) matrix.M11,     (float) matrix.M12,     (float) matrix.M13,     (float) matrix.M14,
            (float) matrix.M21,     (float) matrix.M22,     (float) matrix.M23,     (float) matrix.M24,
            (float) matrix.M31,     (float) matrix.M32,     (float) matrix.M33,     (float) matrix.M34,
            (float) matrix.OffsetX, (float) matrix.OffsetY, (float) matrix.OffsetZ, (float) matrix.M44);

         return m;
      }
   }
}
