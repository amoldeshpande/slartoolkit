#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       A generic Marker detector that searches markers in data provided by an IXrgbReader.
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
using System.Windows.Media.Media3D;

using System.Collections.Generic;

namespace SLARToolKit
{
   /// <summary>
   /// A generic Marker detector that searches markers in data provided by an IXrgbReader.
   /// </summary>
   public class GenericMarkerDetector : AbstractMarkerDetector
   {
      private XrgbIRaster buffer;

      /// <summary>
      /// Creates a new instance of the GenericMarkerDetector.
      /// </summary>
      public GenericMarkerDetector()
      {
      }

      /// <summary>
      /// Creates a new instance of the GenericMarkerDetector.
      /// </summary>
      /// <param name="width">The width of the bitmap that will be used for detection.</param>
      /// <param name="height">The height of the bitmap that will be used for detection.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <param name="markers">A list of markers that should be detected.</param>
      /// <param name="adaptive">Performs an adaptive bitmap thresholding if set to true. Default = false.</param>
      public GenericMarkerDetector(int width, int height, double nearPlane, double farPlane, IList<Marker> markers, bool adaptive = false)
         : this()
      {
         Initialize(width, height, nearPlane, farPlane, markers, adaptive);
      }

      /// <summary>
      /// Initializes the detector for multiple marker detection.
      /// </summary>
      /// <param name="width">The width of the bitmap that will be used for detection.</param>
      /// <param name="height">The height of the bitmap that will be used for detection.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <param name="markers">A list of markers that should be detected.</param>
      /// <param name="adaptive">Performs an adaptive bitmap thresholding if set to true. Default = false.</param>
      public void Initialize(int width, int height, double nearPlane, double farPlane, IList<Marker> markers, bool adaptive = false)
      {
         this.buffer = new XrgbIRaster(width, height);
         Initialize(width, height, nearPlane, farPlane, markers, XrgbIRaster.BufferType, adaptive);
      }

      /// <summary>
      /// Initializes the detector for single marker detection.
      /// </summary>
      /// <param name="width">The width of the bitmap that will be used for detection.</param>
      /// <param name="height">The height of the bitmap that will be used for detection.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <param name="markers">Marker(s) that should be detected.</param>
      /// <param name="adaptive">Performs an adaptive bitmap thresholding if set to true. Default = false.</param>
      public void Initialize(int width, int height, double nearPlane, double farPlane, Marker[] markers, bool adaptive = false)
      {
         Initialize(width, height, nearPlane, farPlane, new List<Marker>(markers), adaptive);
      }

      /// <summary>
      /// Detects all markers in the bitmap.
      /// </summary>
      /// <param name="xrgbReader">A generic XRGB buffer.</param>
      /// <returns>The results of the detection.</returns>
      public DetectionResults DetectAllMarkers(IXrgbReader xrgbReader)
      {
         // Check argument
         if (xrgbReader == null)
         {
            throw new ArgumentNullException("xrgbReader");
         }

         // Update buffer and check size
         this.buffer.XrgbReader = xrgbReader;
         if (!filteredBuffer.getSize().isEqualSize(this.buffer.getSize()))
         {
            throw new ArgumentException("The size of the xrgbReader differs from the initialized size.", "xrgbReader");
         }

         // Detect markers
         return base.DetectAllMarkers(this.buffer);
      }
   }
}
