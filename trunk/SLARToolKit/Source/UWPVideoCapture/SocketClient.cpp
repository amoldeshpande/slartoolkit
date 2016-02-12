#include "pch.h"
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <WinSock2.h>
#include <mstcpip.h>
#include "SocketClient.h"

namespace UWPVideoCapture
{
	bool SocketClient::Connect(int port)
	{
		m_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		if (m_socket != INVALID_SOCKET)
		{
			struct sockaddr_in addr = { 0 };
			addr.sin_family = PF_INET;
			addr.sin_addr.s_addr = inet_addr("127.0.0.1");
			addr.sin_port = htons(port);
			if (connect(m_socket, (sockaddr*)&addr, sizeof(addr)) != SOCKET_ERROR)
			{
				int opt = 1;
				setsockopt(m_socket, IPPROTO_TCP, TCP_NODELAY, (char*)&opt, sizeof(opt));
				opt = 1000;
				setsockopt(m_socket, SOL_SOCKET, SO_SNDTIMEO, (char*)&opt, sizeof(opt));
				/*opt = 1;
				ioctlsocket(m_socket, FIONBIO, (u_long*)&opt);
				opt = 1;
				if (WSAIoctl(m_socket, SIO_LOOPBACK_FAST_PATH, &opt, sizeof(opt), NULL, 0, NULL, NULL, NULL) == SOCKET_ERROR)
				{
					OutputDebugStringA("loopback fast path not supported\r\n");
				}*/
				return true;
			}
		}
		return false;
	}
	void SocketClient::Close()
	{
		closesocket(m_socket);
	}
	
	bool SocketClient::Send(std::vector<unsigned char>& data)
	{
		int len = (int)data.size();
		WSABUF buffers[2];
		buffers[0].buf = (CHAR*)&len;
		buffers[0].len = sizeof(len);
		buffers[1].buf = (CHAR*)&data[0];
		buffers[1].len = data.size();
		
		DWORD sent = 0;
		if(WSASend(m_socket,buffers,2,&sent,0,NULL,NULL) == SOCKET_ERROR)
		{
			dprintf("Send failed error %d\n", WSAGetLastError());
			return false;
		}
		sendCounter++;
		return true;
	}
	extern "C" {
#if _DEBUG
		void dprintf(char * format, ...)
		{
			char buffer[256];
			va_list args;
			va_start(args, format);
			vsprintf_s(buffer, ARRAYSIZE(buffer), format, args);
			va_end(args);
			OutputDebugStringA(buffer);
		}
#else
		void dprintf(char * format, ...)
		{

		}
#endif
	}
}
