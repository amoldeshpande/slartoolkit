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
using Balder.Math;

namespace SLARToolKitBalderSampleSL5.ParticleSystem
{
   public class DirectionalParticleEmitter<T> : IParticleEmitter<T> where T : RenderableNode
   {
      double passedSecondsSum;
      readonly Random rand;

      public float MinVelocity { get; set; }
      public float MaxVelocity { get; set; }

      public Vector Direction { get; set; }
      public float AngleDegree { get; set; }

      public float Frequency { get; set; }

      public bool IsEnabled { get; set; }

      public Func<Vector, T> GetNewParticleCallback { get; set; }

      public DirectionalParticleEmitter()
      {
         rand = new Random();

         MinVelocity = 1.3f;
         MaxVelocity = 1.5f;

         Direction = Vector.UnitY;
         AngleDegree = 20;

         Frequency = 10;
         IsEnabled = true;
      }

      public IList<T> Emit(double passedSeconds)
      {
         var result = new List<T>();

         if (!IsEnabled)
         {
            return result;
         }

         if (GetNewParticleCallback != null)
         {
            // Get number of new particles to emit based on frequency
            passedSecondsSum += passedSeconds;
            var nParticles = (int)(passedSecondsSum * Frequency);

            // If new particles will be emitted, reset the counter, otherwise sum it up
            if (nParticles > 0)
            {
               passedSecondsSum = 0;
            }

            // Calc the emit cone along the direction
            var dir = Direction;
            dir.Normalize();
            var perpendicular = GetPerpendicular(dir);
            var angleRad = MathHelper.ToRadians(AngleDegree);

            // Emit n particles
            for (var i = 0; i < nParticles; i++)
            {
               // Rotate perpendicular vector randomly
               var q = FromAngleAxis((float)(rand.NextDouble() * MathHelper.TwoPi), dir);
               perpendicular = QuaternionMulVector(q, perpendicular);

               // Get final direction by randomly rotating around the direction
               var angle = (float)(rand.NextDouble() * angleRad);
               q = FromAngleAxis(angle, perpendicular);
               var velocity = QuaternionMulVector(q, dir);
               velocity.Normalize();

               // Calculate random velocity along the new direction 
               var velocityScale = (float)GetRandom(MinVelocity, MaxVelocity);
               velocity *= velocityScale;

               // Create new particle
               result.Add(GetNewParticleCallback(velocity));
            }
         }

         return result;
      }

      private static Quaternion FromAngleAxis(float angle, Vector axis)
      {
         var halfAngle = 0.5f * angle;
         var sin = (float)Math.Sin(halfAngle);
         var w = (float)Math.Cos(halfAngle);
         var x = sin * axis.X;
         var y = sin * axis.Y;
         var z = sin * axis.Z;

         return new Quaternion(x, y, z, w);
      }

      private static Vector QuaternionMulVector(Quaternion q, Vector v)
      {
         // From NVIDIA SDK
         var qvec = new Vector(q.X, q.Y, q.Z);
         var uv = qvec.Cross(v);
         var uuv = qvec.Cross(uv);
         uv *= (2.0f * q.W);
         uuv *= 2.0f;
         return v + uv + uuv;
      }

      private static Vector GetPerpendicular(Vector dir)
      {
         // Find closest cardinal base vector to direction
         var dirSq = new Vector(dir.X * dir.X, dir.Y * dir.Y, dir.Z * dir.Z);
         var unit = Vector.UnitZ;
         if (dirSq.X < dirSq.Y && dirSq.X < dirSq.Z) unit = Vector.UnitX;
         else if (dirSq.Y < dirSq.X && dirSq.Y < dirSq.Z) unit = Vector.UnitY;

         // Get perpendicular vector to plane spanned by direction and correponding unit vector
         var perpendicular = dir.Cross(unit);
         perpendicular.Normalize();
         return perpendicular;
      }

      private double GetRandom(double min, double max)
      {
         return min + rand.NextDouble() * (max - min);
      }
   }
}
