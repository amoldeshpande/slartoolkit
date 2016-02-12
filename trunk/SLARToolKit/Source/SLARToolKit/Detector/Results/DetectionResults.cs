#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Results from an AR marker detection.
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
using System.Linq;
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
   /// Results from an AR marker detection.
   /// </summary>
   public class DetectionResults : IEnumerable<DetectionResult>
   {
      private List<DetectionResult> results;

      /// <summary>
      /// The number of results that were found.
      /// </summary>
      public int Count { get { return results.Count; } }

      /// <summary>
      /// True if this collection contains results.
      /// </summary>
      public bool HasResults { get { return results.Count > 0; } }

      /// <summary>
      /// Returns the result at index.
      /// </summary>
      /// <param name="index">The index of the result.</param>
      /// <returns>A detection result.</returns>
      public DetectionResult this[int index] { get { return results[index]; } }

      /// <summary>
      /// Gets the detection result with the highest confidence.
      /// </summary>
      public DetectionResult MostConfidableResult { get { return results.OrderByDescending(r => r.Confidence).FirstOrDefault(); }  }

      /// <summary>
      /// Creates a new detection result collection.
      /// </summary>
      public DetectionResults()
      {
         this.results = new List<DetectionResult>();
      }

      /// <summary>
      /// Adds a detection result.
      /// </summary>
      /// <param name="result">The detection result.</param>
      internal void Add(DetectionResult result)
      {
         this.results.Add(result);
      }

      /// <summary>
      /// Clears all results.
      /// </summary>
      internal void Clear()
      {
         this.results.Clear();
      }

      /// <summary>
      /// Gets an IEnumerator.
      /// </summary>
      /// <returns>An IEnumerator for the results.</returns>
      public IEnumerator<DetectionResult> GetEnumerator()
      {
         return results.GetEnumerator();
      }

      /// <summary>
      /// Gets an IEnumerator.
      /// </summary>
      /// <returns>An IEnumerator for the results.</returns>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return results.GetEnumerator();
      }
   }
}
