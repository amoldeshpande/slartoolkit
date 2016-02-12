#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Result from an AR marker detection.
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Resources;

using jp.nyatla.nyartoolkit.cs.core;
using jp.nyatla.nyartoolkit.cs.detector;

namespace SLARToolKit
{
   /// <summary>
   /// Result from an AR marker detection.
   /// </summary>
   public class DetectionResult
   {
      /// <summary>
      /// A reference to the found marker.
      /// </summary>
      public Marker Marker { get; private set; }

      /// <summary>
      /// The confidence / quality  of the result (Is this really the marker?). The maximum value is 1.
      /// </summary>
      public double Confidence { get; private set; }

      /// <summary>
      /// The transformation matrix for the marker.
      /// </summary>
      public Matrix3D Transformation { get; private set; }

      /// <summary>
      /// The pixel coordinates where the square marker was found. 
      /// </summary>
      public Square Square { get; private set; }

      /// <summary>
      /// Creates a new detection result
      /// </summary>
      /// <param name="marker">A reference to the found marker.</param>
      /// <param name="confidence">The confidence / quality  of the result.</param>
      /// <param name="transformation">The transformation matrix for the marker.</param>
      /// <param name="square">The pixel coordinates where the square marker was found. </param>
      public DetectionResult(Marker marker, double confidence, Matrix3D transformation, Square square)
      {
         this.Marker          = marker;
         this.Confidence      = confidence;
         this.Transformation  = transformation;
         this.Square          = square;
      }
   }
}
