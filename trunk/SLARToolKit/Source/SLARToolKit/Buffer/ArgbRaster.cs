#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       NyAR Raster implementation for an ARGB byte array.
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

using System.Windows.Media.Imaging;

using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.core.rasterreader;

namespace SLARToolKit
{
   /// <summary>
   /// NyAR Raster implementation for an ARGB byte array.
   /// </summary>
   internal class ArgbRaster : NyARRgbRaster_BasicClass
   {
      /// <summary>
      /// The Buffer type (BYTE1D_X8R8G8B8_32)
      /// </summary>
      public const int BufferType = NyARBufferType.BYTE1D_X8R8G8B8_32;

      private ArgbPixelReader pixelReader;
      private byte[] buffer;

      /// <summary>
      /// The ARGB byte Buffer.
      /// </summary>
      public byte[] Buffer 
      {
         get { return buffer; }
         set
         {
            this.buffer = value;
            // Init readers
            pixelReader.Buffer = buffer;
         }
      }

      /// <summary>
      /// Initializes a new ARGB buffer,
      /// </summary>
      /// <param name="width">The width of the buffer that will be used for detection.</param>
      /// <param name="height">The height of the buffer that will be used for detection.</param>
      public ArgbRaster(int width, int height)
         : base(width, height, BufferType)
      {
         this.pixelReader = new ArgbPixelReader(width, height);
      }

      /// <summary>
      /// Gets the RGB pixel reader implementation.
      /// </summary>
      /// <returns>The RGB pixel reader implementation.</returns>
      public override INyARRgbPixelReader getRgbPixelReader()
      {
         return pixelReader;
      }

      /// <summary>
      /// Returns the internal ARGB byte buffer.
      /// </summary>
      /// <returns></returns>
      public override object getBuffer()
      {
         return this.buffer;
      }

      /// <summary>
      /// Determines if this instance has an internal buffer.
      /// </summary>
      /// <returns></returns>
      public override bool hasBuffer()
      {
         return this.buffer != null;
      }

      /// <summary>
      /// Changes the internal buffer to another buffer.
      /// Actually not used. Only to satisfy the interface.
      /// </summary>
      /// <param name="newBuffer">The new buffer that should be used.</param>
      public override void wrapBuffer(object newBuffer)
      {
         this.Buffer = (byte[])newBuffer;
      }
   }
}
