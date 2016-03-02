//*@@@+++@@@@******************************************************************
//
// Microsoft Windows Media Foundation
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// Portions Copyright (c) Microsoft Open Technologies, Inc. 
//
//*@@@---@@@@******************************************************************

#pragma once

namespace UWPVideoCapture
{
	ref class CaptureFrameGrabber;
	class SocketClient;
	public ref class UWPVideoCaptureHelper sealed
	{
	public:
		UWPVideoCaptureHelper();
		virtual ~UWPVideoCaptureHelper();
		Windows::Foundation::IAsyncOperation<bool>^ Start(Windows::Media::Capture::MediaCaptureInitializationSettings^ settings, int width, int height, int port);
		void GetFrame();
		void Stop();
		void Rotate(int rotAngle);

	private:

		Platform::Agile<WMC::MediaCapture> m_capture;
		UWPVideoCapture::CaptureFrameGrabber^ m_grabber;
		std::shared_ptr<SocketClient> m_socketClient;
		bool m_stop;
		int m_width;
		int m_height;
	};

}