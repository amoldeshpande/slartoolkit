using Microsoft.Xna.Framework;


namespace SolarWind
{
   public class Camera
   {
      public Matrix ViewTransform { get; set; } // View
      public Matrix ProjectionTransform { get; set; }

      public float Near { get; set; }
      public float Far { get; set; }

      private float aspectRatio;
      public float AspectRatio
      {
         get
         {
            return aspectRatio;
         }
         set
         {
            aspectRatio = value;
            ProjectionTransform = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio, Near, Far);
         }
      }

      Vector3 cameraPosition;
      Vector3 cameraLookAtTarget;
      Matrix orbitRotation;

      public Camera(float aspectRatio)
         : this(aspectRatio, new Vector3(0, 0, 2), Vector3.Zero)
      {
      }

      public Camera(float aspectRatio, Vector3 position, Vector3 lookAt)
      {
         Near = 0.1f;
         Far = 1000f;
         AspectRatio = aspectRatio;
         cameraPosition = position;
         cameraLookAtTarget = lookAt;
         orbitRotation = Matrix.Identity;
         ViewTransform = Matrix.CreateLookAt(cameraPosition, cameraLookAtTarget, Vector3.Up);
      }

      public Vector3 Position
      {
         set
         {
            cameraPosition = value;

            Matrix world = Matrix.CreateTranslation(cameraPosition) * orbitRotation;
            ViewTransform = Matrix.CreateLookAt(world.Translation, cameraLookAtTarget, Vector3.Up);
         }
         get
         {
            return cameraPosition;
         }
      }

      public Matrix OrbitRotation
      {
         set
         {
            orbitRotation = value;

            Matrix world = Matrix.CreateTranslation(cameraPosition) * orbitRotation;
            ViewTransform = Matrix.CreateLookAt(world.Translation, cameraLookAtTarget, Vector3.Up);
         }
         get
         {
            return orbitRotation;
         }
      }
   }
}
