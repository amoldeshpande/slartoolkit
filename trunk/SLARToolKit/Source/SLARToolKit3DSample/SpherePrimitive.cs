#region File Description
//-----------------------------------------------------------------------------
// SpherePrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;

using Microsoft.Xna.Framework;

#endregion

namespace Primitives3D
{
   /// <summary>
   /// Geometric primitive class for drawing spheres.
   /// </summary>
   public class SpherePrimitive : GeometricPrimitive
   {
      /// <summary>
      /// Constructs a new sphere primitive, using default settings.
      /// </summary>
      public SpherePrimitive()
         : this(1, 16)
      {
      }

      Vector3 vertex = new Vector3();
      Vector2 uv = new Vector2();

      /// <summary>
      /// Constructs a new sphere primitive,
      /// with the specified size and tessellation level.
      /// </summary>
      public SpherePrimitive(float diameter, int tessellation)
      {
         if (tessellation < 3)
            throw new ArgumentOutOfRangeException("tessellation");

         int verticalSegments = tessellation;
         int horizontalSegments = tessellation * 2;

         float radius = diameter / 2;
#if false
            // Start with a single vertex at the bottom of the sphere.
            AddVertex(Vector3.Down * radius, Vector3.Down, new Vector2(1.0f,1.0f));

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++)
            {
                float latitude = ((i + 1) * MathHelper.Pi /
                                            verticalSegments) - MathHelper.PiOver2;

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);
                    Vector2 uv = new Vector2((float)(horizontalSegments - j) / (float)(horizontalSegments), (float)(verticalSegments - 1 - i) / (float)(verticalSegments));
                    //Console.WriteLine(uv);
                    AddVertex(normal * radius, normal, uv);
                }
            }

            // Finish with a single vertex at the top of the sphere.
            AddVertex(Vector3.Up * radius, Vector3.Up, new Vector2(0.0f,0.0f));

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(0);
                AddIndex(1 + (i + 1) % horizontalSegments);
                AddIndex(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex(1 + i * horizontalSegments + j);
                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);

                    AddIndex(1 + i * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + nextJ);
                    AddIndex(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++)
            {
                AddIndex(CurrentVertex - 1);
                AddIndex(CurrentVertex - 2 - (i + 1) % horizontalSegments);
                AddIndex(CurrentVertex - 2 - i);
            }
#else
         int latitudeBands = tessellation;
         int longitudeBands = tessellation * 2;

         for (int latNumber = 0; latNumber <= latitudeBands; latNumber++)
         {
            float theta = latNumber * (float)Math.PI / latitudeBands;
            float sinTheta = (float)Math.Sin(theta);
            float cosTheta = (float)Math.Cos(theta);

            for (int longNumber = 0; longNumber <= longitudeBands; longNumber++)
            {
               float phi = longNumber * 2.0f * (float)Math.PI / longitudeBands;
               float sinPhi = (float)Math.Sin(phi);
               float cosPhi = (float)Math.Cos(phi);

               vertex.X = cosPhi * sinTheta;
               vertex.Y = cosTheta;
               vertex.Z = sinPhi * sinTheta;
               uv.X = 1.0f - ((float)longNumber / (float)longitudeBands);
               uv.Y = ((float)latNumber / (float)latitudeBands);

               AddVertex(vertex * radius, vertex, uv);
            }
         }

         for (int latNumber = 0; latNumber < latitudeBands; latNumber++)
         {
            for (int longNumber = 0; longNumber < longitudeBands; longNumber++)
            {
               int first = (latNumber * (longitudeBands + 1)) + longNumber;
               int second = first + longitudeBands + 1;

               AddIndex(first);
               AddIndex(second);
               AddIndex(first + 1);

               AddIndex(second);
               AddIndex(second + 1);
               AddIndex(first + 1);
            }
         }
#endif
         //InitializePrimitive(graphicsDevice);
      }
   }
}
