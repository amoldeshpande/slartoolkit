#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       WriteableBitmap Map (texture).
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
using Balder.Materials;

namespace SLARToolKitBalderSample
{
   /// <summary>
   /// WriteableBitmap Map (texture).
   /// </summary>
   public class WriteableBitmapMap : IMap
   {
      private WriteableBitmap writeableBitmap;
      public WriteableBitmap WriteableBitmap
      {
         get { return writeableBitmap; }
         set
         {
            writeableBitmap = value;
            HasPixelChanges = true;
            InitializeWidth();
            InitializeHeight();
         }
      }

      public int Width
      {
         get { return writeableBitmap.PixelWidth; }
      }

      public int Height
      {
         get { return writeableBitmap.PixelHeight; }
      }

      public bool IsDynamic
      {
         get { return true; }
      }

      public int WidthBitCount { get; private set; }

      public int HeightBitCount { get; private set; }

      public bool HasPixelChanges { get; private set; }


      public WriteableBitmapMap(WriteableBitmap writeableBitmap)
      {
         WriteableBitmap = writeableBitmap;
      }

      public WriteableBitmapMap()
         : this(new WriteableBitmap(512, 512))
      {
      }

      public int[] GetPixelsAs32BppARGB()
      {
         HasPixelChanges = false;
         return writeableBitmap.Pixels;
      }

      private void InitializeWidth()
      {
         var log = System.Math.Log(Width) / System.Math.Log(2);

         var logAsInt = (int)log;
         var logDiff = log - (double)logAsInt;
         if (logDiff == 0)
         {
            WidthBitCount = (int)log;
         }
      }

      private void InitializeHeight()
      {
         var log = System.Math.Log(Height) / System.Math.Log(2);

         var logAsInt = (int)log;
         var logDiff = log - (double)logAsInt;
         if (logDiff == 0)
         {
            HeightBitCount = (int)log;
         }
      }

   }
}

