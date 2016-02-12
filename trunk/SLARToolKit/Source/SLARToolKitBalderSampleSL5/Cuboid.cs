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
using Balder.Math;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Matrix = Balder.Math.Matrix;

namespace SLARToolKitBalderSampleSL5
{
	public interface IParticle
	{
		bool HasPrepared { get; set; }
	    void Update();
	}


	public class Cuboid : Balder.Objects.Geometries.Box, IParticle
   {
      readonly CollisionSkin collisionSkin;

      public Body PhysicsBody { get; private set; }
      public Matrix Transformation { get; set; }
	   public bool HasPrepared { get; set; }

      public bool IsFixed
      {
         get { return PhysicsBody.Immovable; }
         set { PhysicsBody.Immovable = value; }
      }

      public Cuboid(Vector dimension)
      {
         Dimension = dimension;

         PhysicsBody = new Body();
         collisionSkin = new CollisionSkin(PhysicsBody);
         PhysicsBody.CollisionSkin = collisionSkin;

         var box = new Box(Vector3.Zero, Microsoft.Xna.Framework.Matrix.Identity, Dimension.ToXna());
         collisionSkin.AddPrimitive(box, new MaterialProperties(0.1f, 0.4f, 0.9f));
         
         var com = ApplyMass(1.0f);
         //  PhysicsBody.MoveTo(Utils.VectorBalderToXna(this.Position), Microsoft.Xna.Framework.Matrix.Identity);
         collisionSkin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Microsoft.Xna.Framework.Matrix.Identity));
         PhysicsBody.EnableBody();
      }

	   private float f = 0;
      public void Update()
      {
         var pos = collisionSkin.NewPosition.ToBalder();
         var orient = PhysicsBody.CollisionSkin.GetPrimitiveLocal(0).Transform.Orientation * PhysicsBody.Orientation;
         var orientation = orient.ToBalder();

         World = orientation * Matrix.CreateTranslation(pos);
         if (Transformation != null)
         {
            World *= Transformation;
         }
         f += 0.05f;
         //World = Balder.Math.Matrix.CreateScale((float)Math.Sin(f) + 1);

      }

      private Vector3 ApplyMass(float mass)
      {
         float junk;
         Vector3 com;
         Microsoft.Xna.Framework.Matrix it;
         Microsoft.Xna.Framework.Matrix itCoM;

         var primitiveProperties = new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid, PrimitiveProperties.MassTypeEnum.Mass, mass);
         collisionSkin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCoM);

         PhysicsBody.BodyInertia = itCoM;
         PhysicsBody.Mass = junk;

         return com;
      }
   }
}
