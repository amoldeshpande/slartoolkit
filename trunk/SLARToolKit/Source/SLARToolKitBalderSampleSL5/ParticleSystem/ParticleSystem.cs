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
using Balder.Rendering;
using Microsoft.Practices.ServiceLocation;

namespace SLARToolKitBalderSampleSL5.ParticleSystem
{
   public class ParticleSystem<T> : RenderableNode where T : RenderableNode, IParticle
   {
      DateTime? lastUpdate;
      readonly List<int> lifeTimes;

      public List<T> Particles { get; private set; }
      public IList<IParticleEmitter<T>> Emitters { get; set; }
      public bool DoRebirth { get; set; }
      //public int? MaxLife { get; set; }
      public int? MaxParticles { get; set; }

      public Action<T, int> UpdateParticleCallback { get; set; }

      private readonly INodeRenderingService nodeRenderingService;

      public ParticleSystem()
      {
         Emitters = new List<IParticleEmitter<T>>();
         Particles = new List<T>();
         lifeTimes = new List<int>();
         DoRebirth = true;
         IsEnabled = true;

         nodeRenderingService = ServiceLocator.Current.GetInstance<INodeRenderingService>();
      }

      public virtual void Update()
      {
         if (!IsEnabled)
         {
            return;
         }

         // Calculate time
         var now = DateTime.Now;
         var sec = GetDeltaTime(now);
         lastUpdate = now;

         // Increment life
         for (var i = 0; i < lifeTimes.Count; i++)
         {
            lifeTimes[i]++;
         }

         // Update particles
         if (UpdateParticleCallback != null)
         {
            for (var i = 0; i < Particles.Count; i++)
            {
               UpdateParticleCallback(Particles[i], lifeTimes[i]);
            }
         }

         // A particle absorber logic like the emitters would fit here


         // Emit new particles 
         var newParticles = new List<T>();
         foreach (var emitter in Emitters)
         {
            newParticles.AddRange(emitter.Emit(sec));
         }

         // Use temp. list to avoid cross-thread issues with enumerable
         var tempParticles = new List<T>(Particles);

         //// Remove the old guys
         //if (MaxLife.HasValue)
         //{
         //   for (var i = tempParticles.Count - 1; i >= 0; i--)
         //   {
         //      if (lifeTimes[i] > MaxLife)
         //      {
         //         tempParticles.RemoveAt(i);
         //         lifeTimes.RemoveAt(i);
         //      }
         //   }
         //}

         if (DoRebirth && (MaxParticles.HasValue && tempParticles.Count >= MaxParticles))
         {
            var count = newParticles.Count > tempParticles.Count ? tempParticles.Count : newParticles.Count;
            tempParticles.RemoveRange(0, count);
            lifeTimes.RemoveRange(0, count);
         }

         // Add new particles
         var max = MaxParticles.HasValue ? MaxParticles.Value : int.MaxValue;
         for (var i = 0; i < newParticles.Count && tempParticles.Count < max; i++)
         {
            var newParticle = newParticles[i];
            tempParticles.Add(newParticle);
            lifeTimes.Add(0);
         }

         // Update Particles list
         Particles = tempParticles;
      }

      public void Reset()
      {
         lifeTimes.Clear();
         Particles.Clear();
      }

      public override void Render(Balder.Display.Viewport viewport, Balder.Rendering.DetailLevel detailLevel)
      {
         base.Render(viewport, detailLevel);
         foreach (var particle in Particles)
         {
            if (!particle.HasPrepared)
            {
               particle.Prepare(viewport);
               particle.HasPrepared = true;
            }

            particle.Update();
            nodeRenderingService.PrepareNode(viewport, particle);
            nodeRenderingService.PrepareNodeForRendering(particle, viewport);
            nodeRenderingService.RenderNode(particle, viewport, DetailLevel.Full);
         }
      }

      protected virtual double GetDeltaTime(DateTime now)
      {
         var sec = 0.0;
         if (lastUpdate.HasValue)
         {
            sec = (now - lastUpdate).Value.TotalSeconds;
         }
         return sec;
      }
   }
}
