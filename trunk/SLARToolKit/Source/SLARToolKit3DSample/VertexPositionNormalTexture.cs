#region File Description
//-----------------------------------------------------------------------------
// VertexPositionNormal.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Primitives3D
{
   /// <summary>
   /// Custom vertex type for vertices that have just a
   /// position and a normal, without any texture coordinates.
   /// </summary>
   public struct VertexPositionNormalTexture
   {
      public Vector3 Position;
      public Vector3 Normal;
      public Vector2 TextureCoordinate;


      /// <summary>
      /// Constructor.
      /// </summary>
      public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
      {
         Position = position;
         Normal = normal;
         TextureCoordinate = textureCoordinate;
      }

      /// <summary>
      /// A VertexDeclaration object, which contains information about the vertex
      /// elements contained within this struct.
      /// </summary>
      public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
      (
          new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
          new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
          new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
      );

   }
}
