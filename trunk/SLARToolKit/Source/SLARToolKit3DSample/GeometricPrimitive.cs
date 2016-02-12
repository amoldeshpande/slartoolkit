#region File Description
//-----------------------------------------------------------------------------
// GeometricPrimitive.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Primitives3D
{
   /// <summary>
   /// Base class for simple geometric primitive models. This provides a vertex
   /// buffer, an index buffer, plus methods for drawing the model. Classes for
   /// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
   /// derived from this common base, and use the AddVertex and AddIndex methods
   /// to specify their geometry.
   /// </summary>
   public abstract class GeometricPrimitive : IDisposable
   {
      #region Fields
      public List<VertexPositionNormalTexture> Vertices
      {
         get { return vertices; }
      }

      // During the process of constructing a primitive model, vertex
      // and index data is stored on the CPU in these managed lists.
      List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
      List<ushort> indices = new List<ushort>();


      // Once all the geometry has been specified, the InitializePrimitive
      // method copies the vertex and index data into these buffers, which
      // store it on the GPU ready for efficient rendering.
      VertexBuffer vertexBuffer;
      IndexBuffer indexBuffer;

      bool initialized = false;
      #endregion

      #region Initialization


      /// <summary>
      /// Adds a new vertex to the primitive model. This should only be called
      /// during the initialization process, before InitializePrimitive.
      /// </summary>
      protected void AddVertex(Vector3 position, Vector3 normal)
      {
         vertices.Add(new VertexPositionNormalTexture(position, normal, new Vector2()));
      }

      protected void AddVertex(Vector3 position, Vector3 normal, Vector2 textureCoordinates)
      {
         vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinates));
      }

      /// <summary>
      /// Adds a new index to the primitive model. This should only be called
      /// during the initialization process, before InitializePrimitive.
      /// </summary>
      protected void AddIndex(int index)
      {
         if (index > ushort.MaxValue)
            throw new ArgumentOutOfRangeException("index");

         indices.Add((ushort)index);
      }


      /// <summary>
      /// Queries the index of the current vertex. This starts at
      /// zero, and increments every time AddVertex is called.
      /// </summary>
      protected int CurrentVertex
      {
         get { return vertices.Count; }
      }


      /// <summary>
      /// Once all the geometry has been specified by calling AddVertex and AddIndex,
      /// this method copies the vertex and index data into GPU format buffers, ready
      /// for efficient rendering.
      protected void InitializePrimitive(GraphicsDevice gd)
      {
         // Create a vertex declaration, describing the format of our vertex data.

         // Create a vertex buffer, and copy our vertex data into it.
         vertexBuffer = new VertexBuffer(gd, VertexPositionNormalTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);

         vertexBuffer.SetData(0, vertices.ToArray(), 0, vertices.Count, 0);

         // Create an index buffer, and copy our index data into it.
         indexBuffer = new IndexBuffer(gd, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);

         indexBuffer.SetData(0, indices.ToArray(), 0, indices.Count);
      }


      /// <summary>
      /// Finalizer.
      /// </summary>
      ~GeometricPrimitive()
      {
         Dispose(false);
      }


      /// <summary>
      /// Frees resources used by this object.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }


      /// <summary>
      /// Frees resources used by this object.
      /// </summary>
      protected virtual void Dispose(bool disposing)
      {
         //if (disposing)
         //{
         //    if (vertexBuffer != null)
         //        vertexBuffer.Dispose();

         //    if (indexBuffer != null)
         //        indexBuffer.Dispose();
         //}
      }


      #endregion

      #region Draw


      /// <summary>
      /// Draws the primitive model, using the specified effect. Unlike the other
      /// Draw overload where you just specify the world/view/projection matrices
      /// and color, this method does not set any renderstates, so you must make
      /// sure all states are set to sensible values before you call it.
      /// </summary>
      public void Draw(GraphicsDevice gd)
      {
         if (!initialized)
         {
            InitializePrimitive(gd);
            initialized = true;
         }

         // Set our vertex declaration, vertex buffer, and index buffer.
         gd.SetVertexBuffer(vertexBuffer);
         gd.Indices = indexBuffer;

         int primitiveCount = indices.Count / 3;

         gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                 vertices.Count, 0, primitiveCount);
      }

      #endregion
   }
}
