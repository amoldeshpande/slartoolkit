//*@@@+++@@@@******************************************************************
//
// Microsoft Windows Media Foundation
// Copyright (C) Microsoft Corporation. All rights reserved.
//
// Portions Copyright (c) Microsoft Open Technologies, Inc. 
//
//*@@@---@@@@******************************************************************

#pragma once

#include "MFIncludes.h"


namespace UWPVideoCapture {

class MediaSink;
class SocketClient;

enum class CaptureStreamType
{
    Preview = 0,
    Record
};

ref class CaptureFrameGrabber sealed
{
public:

    virtual ~CaptureFrameGrabber();

internal:

    static concurrency::task<CaptureFrameGrabber^> CreateAsync(_In_ WMC::MediaCapture^ capture, _In_ WMMp::VideoEncodingProperties^ props,std::shared_ptr<SocketClient> client)
    {
        return CreateAsync(capture, props,client, CaptureStreamType::Preview);
    }

    static concurrency::task<CaptureFrameGrabber^> CreateAsync(_In_ WMC::MediaCapture^ capture, _In_ WMMp::VideoEncodingProperties^ props, std::shared_ptr<SocketClient> sock, CaptureStreamType streamType);

  //  concurrency::task<MW::ComPtr<IMF2DBuffer2>> GetFrameAsync();
	void GetFrame();
    concurrency::task<void> FinishAsync();

private:

    CaptureFrameGrabber(_In_ WMC::MediaCapture^ capture, _In_ WMMp::VideoEncodingProperties^ props,std::shared_ptr<SocketClient>sock, CaptureStreamType streamType);

    void ProcessSample(_In_ MediaSample^ sample);

    Platform::Agile<WMC::MediaCapture> _capture;
    ::Windows::Media::IMediaExtension^ _mediaExtension;

    MW::ComPtr<MediaSink> _mediaSink;

    CaptureStreamType _streamType;

	std::shared_ptr<SocketClient> m_socketClient;
    enum class State
    {
        Created,
        Started,
        Closing,
        Closed
    } _state;

    //std::queue<concurrency::task_completion_event<MW::ComPtr<IMF2DBuffer2>>> _videoSampleRequestQueue;
    AutoMF _mf;
    MWW::SRWLock _lock;
};

}