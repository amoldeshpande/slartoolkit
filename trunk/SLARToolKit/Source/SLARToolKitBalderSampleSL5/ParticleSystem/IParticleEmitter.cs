#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2011 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Collections.Generic;
using Balder;
using Balder.Math;

namespace SLARToolKitBalderSampleSL5.ParticleSystem
{
   public interface IParticleEmitter<T> where T : RenderableNode
   {
      float MinVelocity { get; set; }
      float MaxVelocity { get; set; }
      float Frequency { get; set; }
      bool IsEnabled { get; set; }

      Func<Vector, T> GetNewParticleCallback { get; set; }

      IList<T> Emit(double passedSeconds);
   }
}