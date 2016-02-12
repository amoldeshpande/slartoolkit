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

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace SLARToolKit
{
    /// <summary>
    /// Marker detector that subsequently searches markers in the CaptureSource's data.
    /// </summary>
    public class CaptureSourceMarkerDetector
       : AbstractMarkerDetector
    {
        private ArgbRaster buffer;
        private UWPMediaSink videoSink;
        private double nearPlane;
        private double farPlane;
        private IList<Marker> markers;


        /// <summary>
        /// Event raised when a marker detection was completed. 
        /// This event is raised in a background thread and not in the UI thread.
        /// </summary>
        public event EventHandler<MarkerDetectionEventArgs> MarkersDetected;

        /// <summary>
        /// Creates a new instance of the CaptureSourceMarkerDetector.
        /// </summary>
        public CaptureSourceMarkerDetector()
           : base()
        {

        }        
        /// <summary>
        /// Initializes the detector for multiple marker detection.
        /// </summary>
        /// <param name="nearPlane">The near view plane of the frustum.</param>
        /// <param name="farPlane">The far view plane of the frustum.</param>
        /// <param name="markers">A list of markers that should be detected.</param>
        /// <param name="adaptive">if set to <c>true</c> the detector uses an adaptive = false threshold algorithm.</param>
        public async Task<bool> Initialize(double nearPlane, double farPlane, IList<Marker> markers,
            Panel panel, int width, int height, int frameRate,
            bool adaptive = false)
        {
            this.videoSink = new UWPMediaSink(this);
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.markers = markers;
            this.isAdaptive = adaptive;
            await ChangeFormat(width, height);
            return await videoSink.Start(panel, width, height, frameRate);
        }

        /// <summary>
        /// Changes the format.
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        internal async Task ChangeFormat(int width, int height)
        {
            this.buffer = new ArgbRaster(width, height);
            await base.Initialize(width, height, nearPlane, farPlane, markers, ArgbRaster.BufferType, isAdaptive);
        }

        /// <summary>
        /// Called when the detection starts.
        /// </summary>
        internal void Start()
        {            
        }

        /// <summary>
        /// Detects all markers in the bitmap.
        /// </summary>
        /// <param name="argbBuffer">The ARGB byte buffer containing the current frame.</param>
        /// <returns>The results of the detection.</returns>
        internal void DetectAllMarkers(byte[] argbBuffer)
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
            // Fire Event
            OnMarkersDetected(new MarkerDetectionEventArgs(argbBuffer, detectedMarkers, base.bufferWidth, base.bufferHeight, 0));
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