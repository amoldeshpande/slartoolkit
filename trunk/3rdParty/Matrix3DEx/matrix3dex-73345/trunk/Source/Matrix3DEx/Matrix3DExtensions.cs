﻿#region Header
//
//   Project:           Matrix3DEx - Silverlight Matrix3D extensions
//   Description:       Extension methods for the Matrix3D struct.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2015 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;

#if SILVERLIGHT
namespace System.Windows.Media.Media3D
#else
namespace Windows.UI.Xaml.Media.Media3D
#endif
{
   /// <summary>
   /// Extension methods for the Matrix3D struct.
   /// </summary>
   public static class Matrix3DExtensions
   { 
      /// <summary>
      /// Calculates the determinant of the matrix.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <returns>The determinant of the matrix.</returns>
      public static double Determinant(this Matrix3D m)
      {
	      return m.M11 * (m.M22 * m.M33 - m.M23 * m.M32) -
                m.M12 * (m.M21 * m.M33 - m.M23 * m.M31) +
                m.M13 * (m.M21 * m.M32 - m.M22 * m.M31);
      }   
      
      /// <summary>
      /// Returns the transpose of the matrix.
      /// </summary>
      /// <param name="m">The matrix that should be transposed.</param>
      /// <returns>A new transpose of the matrix.</returns>
      public static Matrix3D Transpose(this Matrix3D m)
      {
         return new Matrix3D( m.M11, m.M21, m.M31, m.OffsetX,
		                        m.M12, m.M22, m.M32, m.OffsetY,
					               m.M13, m.M23, m.M33, m.OffsetZ,
					               m.M14, m.M24, m.M34, m.M44);
      }

      /// <summary>
      /// Returns the other handedness of the matrix by mutliplying the z values with -1.
      /// (if m is left-handed the result is right-handed and v.v.).
      /// </summary>
      /// <param name="m">The matrix that should be converted.</param>
      /// <returns>A new matrix that is the other handedness of the matrix.</returns>
      public static Matrix3D SwapHandedness(this Matrix3D m)
      {
         return new Matrix3D(     m.M11,     m.M12,     -m.M13, m.M14,
                                  m.M21,     m.M22,     -m.M23, m.M24,
                                  m.M31,     m.M32,     -m.M33, m.M34,
                              m.OffsetX, m.OffsetY, -m.OffsetZ, m.M44);
      }
      
      /// <summary>
      /// Writes the members of the values row by row into a string.
      /// </summary>
      /// <param name="m">The matrix.</param>
      /// <returns>The formatted string with the mambers.</returns>
      public static string Dump(this Matrix3D m)
      {
         var format = "| {0:##.00} : {1:##.00} : {2:##.00} : {3:##.00} |";
         return string.Format("{0}\r\n{1}\r\n{2}\r\n{3}", 
                              String.Format(format, m.M11, m.M12, m.M13, m.M14), 
                              String.Format(format, m.M21, m.M22, m.M23, m.M24), 
                              String.Format(format, m.M31, m.M32, m.M33, m.M34), 
                              String.Format(format, m.OffsetX, m.OffsetY, m.OffsetZ, m.M44));
      }
   }
}
