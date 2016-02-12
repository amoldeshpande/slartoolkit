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

class MediaStreamSink WrlSealed :
    public Microsoft::WRL::RuntimeClass<
    Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom>,
    IMFStreamSink,
    IMFMediaEventGenerator,
    IMFMediaTypeHandler
    >
{
public:

    MediaStreamSink(
        __in const MW::ComPtr<IMFMediaSink>& sink, 
        __in DWORD id, 
        __in const MW::ComPtr<IMFMediaType>& mt,
        __in MediaSampleHandler^ sampleHandler
    );

    //
    // IMFStreamSink
    //

    IFACEMETHODIMP GetMediaSink(__deref_out IMFMediaSink **sink);
    IFACEMETHODIMP GetIdentifier(__out DWORD *identifier);
    IFACEMETHODIMP GetMediaTypeHandler(__deref_out IMFMediaTypeHandler **handler);
    IFACEMETHODIMP ProcessSample(__in_opt IMFSample *sample);
    IFACEMETHODIMP PlaceMarker(__in MFSTREAMSINK_MARKER_TYPE markerType, __in const PROPVARIANT * markerValue, __in const PROPVARIANT * contextValue);
    IFACEMETHODIMP Flush();

    //
    // IMFMediaEventGenerator
    //

    IFACEMETHODIMP GetEvent(__in DWORD flags, __deref_out IMFMediaEvent **event);
    IFACEMETHODIMP BeginGetEvent(__in IMFAsyncCallback *callback, __in_opt IUnknown *state);
    IFACEMETHODIMP EndGetEvent(__in IMFAsyncResult *result, __deref_out IMFMediaEvent **event);
    IFACEMETHODIMP QueueEvent(__in MediaEventType met, __in REFGUID extendedType, __in HRESULT status, __in_opt const PROPVARIANT *value);

    //
    // IMFMediaTypeHandler
    //

    IFACEMETHODIMP IsMediaTypeSupported(__in IMFMediaType *mediaType, __deref_out_opt  IMFMediaType **closestMediaType);
    IFACEMETHODIMP GetMediaTypeCount(__out DWORD *typeCount);
    IFACEMETHODIMP GetMediaTypeByIndex(__in DWORD index, __deref_out  IMFMediaType **mediaType);
    IFACEMETHODIMP SetCurrentMediaType(__in IMFMediaType *mediaType);
    IFACEMETHODIMP GetCurrentMediaType(__deref_out_opt IMFMediaType **mediaType);
    IFACEMETHODIMP GetMajorType(__out GUID *majorType);

    //
    // Misc
    //

    void InternalSetCurrentMediaType(__in const MW::ComPtr<IMFMediaType>& mediaType);
    void RequestSample();
    void Shutdown();

private:

    bool _IsMediaTypeSupported(__in const MW::ComPtr<IMFMediaType>& mt) const;
    void _UpdateMediaType(__in const MW::ComPtr<IMFMediaType>& mt);

    void _VerifyNotShutdown()
    {
        if (_shutdown)
        {
            CHK(MF_E_SHUTDOWN);
        }
    }

    MW::ComPtr<IMFMediaSink> _sink;
    MW::ComPtr<IMFMediaEventQueue> _eventQueue;
    MW::ComPtr<IMFMediaType> _curMT;

    MediaSampleHandler^ _sampleHandler;

    GUID _majorType;
    GUID _subType;
    unsigned int _width;
    unsigned int _height;
    DWORD _id;
    bool _shutdown;

    MWW::SRWLock _lock;
};

}