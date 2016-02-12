#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       A pattern matcher derived from NyARMatchPatt_Color_WITHOUT_PCA, that has a public marker getter.
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

using jp.nyatla.nyartoolkit.cs.core;

namespace SLARToolKit
{
   /// <summary>
   /// A pattern matcher derived from NyARMatchPatt_Color_WITHOUT_PCA, that has a public marker getter.
   /// </summary>
   internal class PatternMatcher : NyARMatchPatt_Color_WITHOUT_PCA
   {            
      /// <summary>
      /// The marker that was found.
      /// </summary>
      public Marker Marker { get; private set; }

      /// <summary>
      /// Creates a new instance of the PatternMatcher with a marker.
      /// </summary>
      /// <param name="marker">A marker.</param>
      public PatternMatcher(Marker marker)
         : base(marker.NyMarker)
      {
         this.Marker = marker;
      }
   }
}
