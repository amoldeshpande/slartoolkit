using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;

using System.Windows.Resources;

public class ContentManager
{
   //
   // LoadTexture - Create a Texture2D from an image in our XAP
   //

   static public Texture2D LoadBitmap(string imageName)
   {
      //
      // Load a bitmap image through writeablebitmap so that we can populate our device
      // texture
      //

      StreamResourceInfo sr = Application.GetResourceStream(new Uri(imageName, UriKind.Relative));
      BitmapImage bs = new BitmapImage();
      bs.SetSource(sr.Stream);

      Texture2D t = new Texture2D(GraphicsDeviceManager.Current.GraphicsDevice, bs.PixelWidth, bs.PixelHeight, false, SurfaceFormat.Color);
      bs.CopyTo(t);

      return t;
   }

   static public Texture2D LoadBitmap(string imageName, int w, int h)
   {
      //
      // Load a bitmap image through writeablebitmap so that we can populate our device
      // texture
      //

      StreamResourceInfo sr = Application.GetResourceStream(new Uri(imageName, UriKind.Relative));
      BitmapImage bs = new BitmapImage();
      bs.SetSource(sr.Stream);

      var wb = new WriteableBitmap(w, h);

      ScaleTransform t = new ScaleTransform();
      t.ScaleX = (double)w / (double)bs.PixelWidth;
      t.ScaleY = (double)h / (double)bs.PixelHeight;

      Image image = new Image();
      image.Source = bs;
      wb.Render(image, t);
      wb.Invalidate();

      Texture2D tex = new Texture2D(GraphicsDeviceManager.Current.GraphicsDevice, wb.PixelWidth, wb.PixelHeight, false, SurfaceFormat.Color);
      wb.CopyTo(tex);

      return tex;
   }

   public static Texture2D LoadBitmapAndMipFromResource(string relativePath)
   {
      var assembly = System.Reflection.Assembly.GetCallingAssembly();
      var asmName = new System.Reflection.AssemblyName(assembly.FullName).Name;
      using (var stream = Application.GetResourceStream(new Uri("/" + asmName + ";component/" + relativePath, UriKind.Relative)).Stream)
      {
         return LoadBitmapAndMip(stream);
      }
   }

   public static Texture2D LoadBitmapAndMip(string imageName)
   {
      // Load bitmap
      using (var stream = Application.GetResourceStream(new Uri(imageName, UriKind.Relative)).Stream)
      {
         return LoadBitmapAndMip(stream);
      }
   }

   private static Texture2D LoadBitmapAndMip(Stream stream)
   {
      var bs = new BitmapImage();
      bs.SetSource(stream);

      // Get dimensions        
      int w = bs.PixelWidth;
      int h = bs.PixelHeight;

      if ((w % 2 != 0 && w != 1) || (h % 2 != 0 && h != 1))
         throw new InvalidOperationException("Bitmap must be power of 2.");

      // Calculate mip levels
      int mipLevels = 1;
      int maxDimension = Math.Max(w, h);
      while (maxDimension > 1)
      {
         mipLevels++;
         maxDimension /= 2;
      }

      // Create the chain
      Texture2D tex = new Texture2D(GraphicsDeviceManager.Current.GraphicsDevice, w, h, true, SurfaceFormat.Color);

      // Put bitmap into a renderable image
      Image image = new Image();
      image.Source = bs;

      // Generate mip level data
      for (int level = 0; level < mipLevels; level++)
      {
         var wb = new WriteableBitmap(w, h);

         // Scale to current mip level
         ScaleTransform t = new ScaleTransform();
         t.ScaleX = (double)w / (double)bs.PixelWidth;
         t.ScaleY = (double)h / (double)bs.PixelHeight;

         // Black out the image
         for (int c = 0; c < w * h; c++)
            wb.Pixels[c] = 0;

         // Small mip levels are rendering as white, so don't render
         if (w > 1 && h > 1)
            wb.Render(image, t);

         // Update WB
         wb.Invalidate();

         // Grab pixel data
         wb.CopyTo(tex, level, null, 0, 0);

         // Shrink for the next level
         if (w != 1)
            w /= 2;
         if (h != 1)
            h /= 2;
      }

      return tex;
   }
}
