#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Custom camera.
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

namespace SLARToolKitBalderSample
{
   /// <summary>
   /// Custom camera.
   /// </summary>
   public class Camera : Balder.View.Camera
   {
      public Balder.Math.Matrix CustomProjectionMatrix { get; set; }   

      public override Balder.Math.Matrix ProjectionMatrix
      {
         get
         {
            if (CustomProjectionMatrix == null)
            {
               return base.ProjectionMatrix;
            }
            return CustomProjectionMatrix;
         }
      }
   }
}
