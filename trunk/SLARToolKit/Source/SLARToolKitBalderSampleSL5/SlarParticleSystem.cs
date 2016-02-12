#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-02-24 00:35:56 +0100 (Mi, 24 Feb 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKitSample/Properties/AssemblyInfo.cs $
//   Id:                $Id: AssemblyInfo.cs 48548 2010-02-23 23:35:56Z unknown $
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
using Balder.Imaging;
using Balder.Materials;
using Balder.Math;
using Microsoft.Xna.Framework;
using SLARToolKitBalderSampleSL5.ParticleSystem;
using Color = Balder.Color;

namespace SLARToolKitBalderSampleSL5
{
   public class SlarParticleSystem : ParticleSystem<Cuboid>
   {
      readonly Random rand;
      readonly DirectionalParticleEmitter<Cuboid> directedParticleEmitter;

      public Color MinColor { get; set; }
      public Color MaxColor { get; set; }

      public float MinSize { get; set; }
      public float MaxSize { get; set; }

      public float Frequency
      {
         get { return directedParticleEmitter.Frequency; }
         set { directedParticleEmitter.Frequency = value; }
      }

      public float Angle
      {
         get { return directedParticleEmitter.AngleDegree; }
         set { directedParticleEmitter.AngleDegree = value; }
      }

      public float Velocity
      {
         get { return directedParticleEmitter.MinVelocity; }
         set
         {
            var diff = directedParticleEmitter.MaxVelocity - directedParticleEmitter.MinVelocity;
            directedParticleEmitter.MinVelocity = value;
            directedParticleEmitter.MaxVelocity = value + diff;
         }
      }

      public SlarParticleSystem()
      {
         rand = new Random();

         MinColor = new Color(0, 0, 0, 255);
         MaxColor = new Color(255, 255, 255, 255);

         MinSize = 5;
         MaxSize = 10;

         DoRebirth = false;
         MaxParticles = 200;

         directedParticleEmitter = new DirectionalParticleEmitter<Cuboid>
                                   {
                                      MinVelocity = 40,
                                      MaxVelocity = 60,
                                      Frequency = 1,
                                      GetNewParticleCallback = velocity =>
                                                               {
                                                                  var color = new Color
                                                                     (
                                                                     (byte)rand.Next(MinColor.Red, MaxColor.Red),
                                                                     (byte)rand.Next(MinColor.Green, MaxColor.Green),
                                                                     (byte)rand.Next(MinColor.Blue, MaxColor.Blue),
                                                                     (byte)rand.Next(MinColor.Alpha, MaxColor.Alpha)
                                                                     );

                                                                  var size = MinSize + rand.NextDouble() * (MaxSize - MinSize);

                                                                  var partilce = new Cuboid(new Coordinate(size, size, size))
                                                                                 {
                                                                                    Material = new Material
                                                                                               {
                                                                                                  Diffuse = color,
                                                                                                  DoubleSided = true,
                                                                                               },
                                                                                 };
                                                                  partilce.PhysicsBody.ApplyBodyImpulse(new Vector3(velocity.X, velocity.Y, velocity.Z));
                                                                  partilce.PhysicsBody.AngularVelocity = new Vector3(GetRandAngular(), GetRandAngular(), GetRandAngular());
                                                                  return partilce;
                                                               }
                                   };
         Emitters = new List<IParticleEmitter<Cuboid>> { directedParticleEmitter };

         UpdateParticleCallback = (particle, life) => particle.Update();
      }

      private float GetRandAngular()
      {
         return (float)(rand.NextDouble() - 0.5);
      }

      protected override double GetDeltaTime(DateTime now)
      {
         return 0.1;
      }
   }
}
