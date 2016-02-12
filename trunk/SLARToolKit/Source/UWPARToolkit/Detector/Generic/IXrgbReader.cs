﻿#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       An interface that provides data in XRGB format from a byte buffer..
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

namespace SLARToolKit
{
   /// <summary>
   /// An interface that provides data in XRGB format from a byte buffer.
   /// </summary>
   public interface IXrgbReader
   {
      /// <summary>
      /// Thw width of the buffer.
      /// </summary>
      int Width { get; }

      /// <summary>
      /// The height of the buffer.
      /// </summary>
      int Height { get; }

      /// <summary>
      /// Gets the color as XRGB byte components for the given x and y coordinate. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <returns>The color at the x and y coordinate.</returns>
      int GetPixel(int x, int y);

      /// <summary>
      /// Gets the color as XRGB byte components for a set of x and y coordinates. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <param name="x">The set of x coordinates.</param>
      /// <param name="y">The set of y coordinates.</param>
      /// <returns>The color at the x and y coordinates.</returns>
      int[] GetPixels(int[] x, int[] y);

      /// <summary>
      /// Gets the color as XRGB byte components for the whole bitmap. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <returns>The color for the whole bitmap as int array.</returns>
      int[] GetAllPixels();

      /// <summary>
      /// Gets the color as XRGB bytes for the whole bitmap. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <returns>The color for the whole bitmap as int array.</returns>
      byte[] GetAllPixelsAsByte();
   }
}