#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Marker detector that subsequently searches markers in the CaptureSource's data.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using System.Collections.Generic;
using System.Windows.Media;

namespace SLARToolKit
{
   /// <summary>
   /// Marker detector that subsequently searches markers in the CaptureSource's data.
   /// </summary>
   public class CaptureSourceMarkerDetector 
       : AbstractMarkerDetector
   {
      private ArgbRaster buffer;
      private DetectorVideoSink videoSink;
      private double nearPlane;
      private double farPlane;
      private IList<Marker> markers;
      private long highestFrameNumber;
      private bool isMultiThreaded;

      /// <summary>
      /// Event raised when a marker detection was completed. 
      /// This event is raised in a background thread and not in the UI thread.
      /// </summary>
      public event EventHandler<MarkerDetectionEventArgs> MarkersDetected;

      /// <summary>
      /// If true, the detection will be performed multi-threaded. It's often faster, 
      /// but it's not guaranteed that each frame is used for detection due to concurrency, 
      /// which should not be a huge problem.
      /// </summary>
      public bool IsMultithreaded 
      {
         get { return isMultiThreaded; }
         set
         {
            isMultiThreaded = value;
            if (videoSink != null)
            {
               videoSink.IsMultithreaded = isMultiThreaded;
            }
         }
      }

      /// <summary>
      /// Creates a new instance of the CaptureSourceMarkerDetector.
      /// </summary>
      public CaptureSourceMarkerDetector()
         : base()
      {
         this.IsMultithreaded = false;
      }

      /// <summary>
      /// Creates a new instance of the CaptureSourceMarkerDetector.
      /// </summary>
      /// <param name="captureSource">The capture source.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <param name="markers">A list of markers that should be detected.</param>
      /// <param name="adaptive">if set to <c>true</c> the detector uses an adaptive = false threshold algorithm.</param>
      public CaptureSourceMarkerDetector(CaptureSource captureSource, double nearPlane, double farPlane, IList<Marker> markers, bool adaptive = false)
         : this()
      {
         Initialize(captureSource, nearPlane, farPlane, markers, adaptive);
      }

      /// <summary>
      /// Initializes the detector for multiple marker detection.
      /// </summary>
      /// <param name="captureSource">The capture source.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <param name="markers">A list of markers that should be detected.</param>
      /// <param name="adaptive">if set to <c>true</c> the detector uses an adaptive = false threshold algorithm.</param>
      public void Initialize(CaptureSource captureSource, double nearPlane, double farPlane, IList<Marker> markers, bool adaptive = false)
      {
         this.videoSink = new DetectorVideoSink(this) 
         { 
            CaptureSource = captureSource, 
            IsMultithreaded = isMultiThreaded 
         };
         this.nearPlane = nearPlane;
         this.farPlane = farPlane;
         this.markers = markers;
         this.isAdaptive = adaptive;
      }

      /// <summary>
      /// Changes the format.
      /// </summary>
      /// <param name="width">The width of the buffer that will be used for detection.</param>
      /// <param name="height">The height of the buffer that will be used for detection.</param>
      internal void ChangeFormat(int width, int height)
      {
         this.buffer = new ArgbRaster(width, height);
         base.Initialize(width, height, nearPlane, farPlane, markers, ArgbRaster.BufferType, isAdaptive);
      }

      /// <summary>
      /// Called when the detection starts.
      /// </summary>
      internal void Start()
      {
         highestFrameNumber = -1;
      }

      /// <summary>
      /// Detects all markers in the bitmap.
      /// </summary>
      /// <param name="argbBuffer">The ARGB byte buffer containing the current frame.</param>
      /// <param name="frameNumber">The current frame number of the buffer.</param>
      /// <returns>The results of the detection.</returns>
      internal void DetectAllMarkers(byte[] argbBuffer, long frameNumber)
      {
         // Check argument
         if (argbBuffer == null)
         {
            throw new ArgumentNullException("argbBuffer");
         }

         // Update buffer and check size
         this.buffer.Buffer = argbBuffer;

         // Detect markers
         var detectedMarkers = base.DetectAllMarkers(this.buffer);

         // Only fire newer frames and leave older frames out
         if (frameNumber > highestFrameNumber)
         {
            // Thread safe exchange
#if WINDOWS_PHONE
            highestFrameNumber = frameNumber;
#else
            System.Threading.Interlocked.Exchange(ref highestFrameNumber, frameNumber);
#endif
            // Fire Event
            OnMarkersDetected(new MarkerDetectionEventArgs(detectedMarkers, base.bufferWidth, base.bufferHeight, frameNumber));
         }
      }

      /// <summary>
      /// Fires the MarkersDetected event.
      /// </summary>
      /// <param name="args">The MarkerDetectionEventArgs.</param>
      private void OnMarkersDetected(MarkerDetectionEventArgs args)
      {
         if (MarkersDetected != null)
         {
            MarkersDetected(this, args);
         }
      }
   }
}
