#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Provides data in XRGB format from the WriteableBitmap.
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

using System;
using System.Windows.Media.Imaging;

namespace SLARToolKit
{
   /// <summary>
   /// Provides data in XRGB format from the WriteableBitmap.
   /// </summary>
   internal class WriteableBitmapReader : IXrgbReader
   {
      public WriteableBitmap Bitmap { get; set; }

      /// <summary>
      /// Thw width of the buffer.
      /// </summary>
      public int Width { get { return Bitmap.PixelWidth; } }

      /// <summary>
      /// The height of the buffer.
      /// </summary>
      public int Height { get { return Bitmap.PixelHeight; } }

      /// <summary>
      /// Gets the color as XRGB byte components for the given x and y coordinate. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <returns>The color at the x and y coordinate.</returns>
      public int GetPixel(int x, int y)
      {
         return Bitmap.Pixels[y * Bitmap.PixelWidth + x];
      }

      /// <summary>
      /// Gets the color as XRGB int components for a set of x and y coordinates. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <param name="x">The set of x coordinates.</param>
      /// <param name="y">The set of y coordinates.</param>
      /// <returns>The color at the x and y coordinates.</returns>
      public int[] GetPixels(int[] x, int[] y)
      {
         if (x.Length != y.Length)
         {
            throw new System.ArgumentException("The length oy the x coordinate set is not equal to the y set.", "x, y");
         }

         var p = Bitmap.Pixels;
         var w = Bitmap.PixelWidth;
         var l = x.Length;
         var r = new int[l];

         for (int i = 0; i < l; i++)
         {
            r[i] = p[ y[i] * w + x[i] ];
         }

         return r;
      }

      /// <summary>
      /// Gets the color as XRGB byte components for the whole bitmap. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <returns>The color for the whole bitmap as int array.</returns>
      public int[] GetAllPixels()
      {
         return Bitmap.Pixels;
      }

      /// <summary>
      /// Gets the color as XRGB bytes for the whole bitmap. 
      /// Only the RGB part will actually be used.
      /// </summary>
      /// <returns>The color for the whole bitmap as int array.</returns>
      public byte[] GetAllPixelsAsByte()
      {
         var byteBuffer = new byte[Bitmap.Pixels.Length << 2];
         Buffer.BlockCopy(Bitmap.Pixels, 0, byteBuffer, 0, byteBuffer.Length);
         return byteBuffer;
      }
   }
}