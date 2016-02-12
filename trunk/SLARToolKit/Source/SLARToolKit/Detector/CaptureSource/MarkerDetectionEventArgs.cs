#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Event args for marker detection results.
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLARToolKit
{
   /// <summary>
   /// Event args for marker detection results.
   /// </summary>
   public class MarkerDetectionEventArgs : EventArgs
   {
      /// <summary>
      /// The marker detection results.
      /// </summary>
      public DetectionResults DetectionResults { get; private set; }

      /// <summary>
      /// The frame number where the results where detected.
      /// </summary>
      public long FrameNumber { get; private set; }

      /// <summary>
      /// The width of the bitmap buffer that was used for detection in screen coordinates.
      /// </summary>
      public int BufferWidth { get; private set; }

      /// <summary>
      /// The height of the bitmap buffer that was used for detection in screen coordinates.
      /// </summary>
      public int BufferHeight { get; private set; }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="detectionResults">The detection results.</param>
      /// <param name="width">The width of the bitmap buffer that was used for detection in screen coordinates.</param>
      /// <param name="height">The height of the bitmap buffer that was used for detection in screen coordinates.</param>
      /// <param name="frameNumber">The frame number where the results where detected.</param>
      public MarkerDetectionEventArgs(DetectionResults detectionResults, int width, int height, long frameNumber)
      {
         this.DetectionResults   = detectionResults;
         this.BufferWidth              = width;
         this.BufferHeight             = height;
         this.FrameNumber        = frameNumber;
      }
   }
}
