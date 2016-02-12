#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Convert methods.
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

namespace SLARToolKitBalderSample
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
      public static Balder.Math.Matrix ToBalderMatrix(this System.Windows.Media.Media3D.Matrix3D matrix)
      {
         var m = new Balder.Math.Matrix();

         m[0, 0] = (float)matrix.M11;
         m[0, 1] = (float)matrix.M12;
         m[0, 2] = (float)matrix.M13;
         m[0, 3] = (float)matrix.M14;

         m[1, 0] = (float)matrix.M21;
         m[1, 1] = (float)matrix.M22;
         m[1, 2] = (float)matrix.M23;
         m[1, 3] = (float)matrix.M24;

         m[2, 0] = (float)matrix.M31;
         m[2, 1] = (float)matrix.M32;
         m[2, 2] = (float)matrix.M33;
         m[2, 3] = (float)matrix.M34;

         m[3, 0] = (float)matrix.OffsetX;
         m[3, 1] = (float)matrix.OffsetY;
         m[3, 2] = (float)matrix.OffsetZ;
         m[3, 3] = (float)matrix.M44;

         return m;
     }
   }
}
