#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Assembly Infos
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

using Balder;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
using JigLibX.Collision;
using JigLibX.Physics;

namespace SLARToolKitBalderSampleSL5
{
   public class SlarGame : Game
   {
      const float UpdateStep = 0.04f;

      readonly PhysicsSystem world;
      readonly Cuboid ground;

      public SlarParticleSystem ParticleSystem { get; private set; }

      public SlarGame()
      {
         ParticleSystem = new SlarParticleSystem();

         world = new PhysicsSystem
                 {
                    CollisionSystem = new CollisionSystemSAP { UseSweepTests = true },
                    EnableFreezing = true,
                    SolverType = PhysicsSystem.Solver.Normal,
                    NumCollisionIterations = 8,
                    NumContactIterations = 8,
                    NumPenetrationRelaxtionTimesteps = 15
                 };

         ground = new Cuboid(new Vector(300, 1, 300))
                  {
                     Material = new Material {Diffuse = new Color(50, 50, 50, 100), DoubleSided = true},
                     IsFixed = true,
                  };
      }

      public override void OnInitialize()
      {
         Children.Add(ParticleSystem);
         Children.Add(ground);
         base.OnInitialize();
      }

      public override void OnUpdate()
      {
         base.OnUpdate();

         world.Integrate(UpdateStep);
         ParticleSystem.Update();
         ground.Update();
      }

      public void SetWorldMatrix(Matrix transformation)
      {
         if (transformation != null)
         {
            ground.Transformation = transformation;
            foreach (var particle in ParticleSystem.Particles)
            {
               particle.Transformation = transformation;
            }
         }
      }
   }
}
