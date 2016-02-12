#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       VideoSink that triggers a marker detector.
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace SLARToolKit
{
   /// <summary>
   /// VideoSink that triggers a marker detector.
   /// </summary>
   internal class DetectorVideoSink : VideoSink
   {
      private CaptureSourceMarkerDetector detector;
      private VideoFormat vidFormat;
      private long frameCounter;

      /// <summary>
      /// If true, the detection will be called multi-threaded.
      /// </summary>
      public bool IsMultithreaded { get; set; }

      /// <summary>
      /// Initializes a new DetectorVideoSink.
      /// </summary>
      /// <param name="detector">The CaptureSourceMarkerDetector to use.</param>
      public DetectorVideoSink(CaptureSourceMarkerDetector detector)
         : base()
      {
         this.detector = detector;
      }

      /// <summary>
      /// Invoked when a video device starts capturing video data.
      /// </summary>
      protected override void OnCaptureStarted()
      {
         frameCounter = 0;
         detector.Start();
      }

      /// <summary>
      /// Invoked when a video device stops capturing video data.
      /// </summary>
      protected override void OnCaptureStopped()
      {
      }

      /// <summary>
      /// Invoked when a video device reports a  video format change.
      /// </summary>
      /// <param name="videoFormat">The new video format.</param>
      protected override void OnFormatChange(VideoFormat videoFormat)
      {
         if (videoFormat.PixelFormat != PixelFormatType.Format32bppArgb)
         {
            throw new InvalidOperationException(String.Format("Only 32 Bit ARGB pixel format is supported, not {0}.", videoFormat.PixelFormat));
         }

         detector.ChangeFormat(videoFormat.PixelWidth, videoFormat.PixelHeight);
         vidFormat = videoFormat;
      }

      /// <summary>
      /// Invoked when a video device captures a complete video sample / frame.
      /// </summary>
      /// <param name="sampleTime">The time when the sample was captured in 100 nanosecond units.</param>
      /// <param name="frameDuration"> The duration of the sample in 100 nanosecond units.</param>
      /// <param name="sampleData">A byte stream containing video data, to be interpreted per the relevant video format information.</param>
      protected override void OnSample(long sampleTime, long frameDuration, byte[] sampleData)
      {
         if (IsMultithreaded)
         {
            ThreadPool.QueueUserWorkItem(data => ExecuteDetection(sampleData, frameCounter));
         }
         else
         {
            ExecuteDetection(sampleData, frameCounter);
         }
         frameCounter++;
      }

      /// <summary>
      /// Executes the detection
      /// </summary>
      /// <param name="sampleData">The sample data form the webcam.</param>
      /// <param name="frameNumber">The current frame number.</param>
      private void ExecuteDetection(byte[] sampleData, long frameNumber)
      {
         // Copy buffer to get rid of the negative stride
         int h = vidFormat.PixelHeight;
         int s = vidFormat.Stride;
         int offset, sp;
         byte[] buffer = new byte[vidFormat.PixelWidth * h << 2];
         if (s < 0)
         {
            for (int y = 0; y < h; y++)
            {
               sp = -s;
               offset = sp * (h - (y + 1));
               Buffer.BlockCopy(sampleData, offset, buffer, sp * y, sp);
            }
         }
         else
         {
            for (int y = 0; y < h; y++)
            {
               offset = s * y;
               Buffer.BlockCopy(sampleData, offset, buffer, offset, s);
            }
         }

         // Call detector
         detector.DetectAllMarkers(buffer, frameNumber);
      }
   }
}