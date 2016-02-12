#pragma once
namespace UWPVideoCapture
{
	class SocketClient
	{
		SOCKET m_socket;
		int sendCounter;
	public:
		SocketClient() :m_socket(INVALID_SOCKET),sendCounter(0) {}
		bool Connect(int port);
		void Close();
		bool Send(std::vector<unsigned char>& data);
	};
}