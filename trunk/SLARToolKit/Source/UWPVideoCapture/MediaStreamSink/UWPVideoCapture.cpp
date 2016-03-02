//*@@@+++@@@@******************************************************************
//
// Microsoft Windows Media Foundation
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// Portions Copyright (c) Microsoft Open Technologies, Inc. 
//
//*@@@---@@@@******************************************************************
#include <pch.h>
#include <winsock2.h>
#include "CaptureFrameGrabber.h"
#include "UWPVideoCapture.h"
#include "SocketClient.h"

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Microsoft::WRL;
using namespace Windows::Media::MediaProperties;
using namespace Windows::Media::Capture;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace UWPVideoCapture
{

	UWPVideoCaptureHelper::UWPVideoCaptureHelper()
		: m_width(0)
		, m_height(0)
		, m_stop(false)
		, m_socketClient(NULL)
	{

	}

	UWPVideoCaptureHelper::~UWPVideoCaptureHelper()
	{
		if (m_socketClient != NULL)
		{
			m_socketClient->Close();
			m_socketClient = nullptr;
		}

	}
	IAsyncOperation<bool>^ UWPVideoCaptureHelper::Start(MediaCaptureInitializationSettings^ settings,int width, int height, int port)
	{	
		m_width = width;
		m_height = height;
		m_capture = ref new MediaCapture();
		return create_async([this, settings,port]()
		{
			bool result = false;
			m_socketClient = std::make_shared<SocketClient>();
			if (!m_socketClient )
			{
				return false;
			}
			create_task(m_capture->InitializeAsync(settings)).then([this,&result,port]() {

				auto props = safe_cast<VideoEncodingProperties^>(m_capture->VideoDeviceController->GetMediaStreamProperties(MediaStreamType::VideoPreview));
				props->Subtype = MediaEncodingSubtypes::Bgra8; // Ask for color conversion to match WriteableBitmap
				props->Width = m_width;
				props->Height = m_height;
				auto t =  ::UWPVideoCapture::CaptureFrameGrabber::CreateAsync(m_capture.Get(), props,m_socketClient);
			    t.then([this,&result,port](CaptureFrameGrabber^ grb)
				{
					m_grabber = grb;
					result = (m_grabber != nullptr);
					if (result)
					{
						if (!m_socketClient->Connect(port))
						{
							result = false;
						}
					}
				}).wait();				
			}).wait();
			return result;
		});
	}	
	void UWPVideoCaptureHelper::GetFrame()
	{
		m_grabber->GetFrame();
	}
	void UWPVideoCaptureHelper::Stop()
	{
		m_stop = true;
		auto t= m_grabber->FinishAsync();
		t.wait();
	}
	static  GUID  RotationKey = { 0xC380465D , 0x2271,0x428C,{0x9B,0x83,0xEC,0xEA,0x3B,0x4A,0x85,0xC1 } };
	void UWPVideoCaptureHelper::Rotate(int rotAngle)
	{
		if (m_capture != nullptr)
		{
			auto props = m_capture->VideoDeviceController->GetMediaStreamProperties(MediaStreamType::VideoPreview);
			props->Properties->Insert(RotationKey, rotAngle);
		}
	}
}