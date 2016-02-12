#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       A square with 4 points.
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
   /// A square with 4 points.
   /// </summary>
   public struct Square
   {
      /// <summary>
      /// The first point of the square.
      /// </summary>
      public Point P1;

      /// <summary>
      /// The second point of the square.
      /// </summary>
      public Point P2;

      /// <summary>
      /// The third point of the square.
      /// </summary>
      public Point P3;

      /// <summary>
      /// The fourth point of the square.
      /// </summary>
      public Point P4;

      /// <summary>
      /// Calculates the center point of this rectangle.
      /// </summary>
      /// <returns>The center of the rectangle.</returns>
      public Point Center
      {
         get
         {
            var cX = (P1.X + P2.X + P3.X + P4.X) * 0.25;
            var cY = (P1.Y + P2.Y + P3.Y + P4.Y) * 0.25;
            return new Point(cX, cY);
         }
      }

      /// <summary>
      /// Creates a new square.
      /// </summary>
      /// <param name="p1">The first point of the square.</param>
      /// <param name="p2">The second point of the square.</param>
      /// <param name="p3">The third point of the square.</param>
      /// <param name="p4">The fourth point of the square.</param>
      public Square(Point p1, Point p2, Point p3, Point p4)
      {
         P1 = p1;
         P2 = p2;
         P3 = p3;
         P4 = p4;
      }

      /// <summary>
      /// Creates a new square.
      /// </summary>
      /// <param name="p1x">The x-coordinate for the first point of the square.</param>
      /// <param name="p1y">The y-coordinate for the first point of the square.</param>
      /// <param name="p2x">The x-coordinate for the second point of the square.</param>
      /// <param name="p2y">The y-coordinate for the second point of the square.</param>
      /// <param name="p3x">The x-coordinate for the third point of the square.</param>
      /// <param name="p3y">The y-coordinate for the third point of the square.</param>
      /// <param name="p4x">The x-coordinate for the fourth point of the square.</param>
      /// <param name="p4y">The y-coordinate for the fourth point of the square.</param>
      public Square(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double p4x, double p4y)
         : this(new Point(p1x, p1y), new Point(p2x, p2y), new Point(p3x, p3y), new Point(p4x, p4y))
      {
      }
   }
}
