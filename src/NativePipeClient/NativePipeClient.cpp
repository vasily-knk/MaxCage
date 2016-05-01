#include "stdafx.h"

int main()
{
    HANDLE pipe;
    while (true)
    {
        pipe = CreateFileA("\\\\.\\pipe\\my_pipe", GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
        if (pipe != INVALID_HANDLE_VALUE)
            break;

        Sleep(1000);
    }
    
    cout << "Connected to pipe" << endl;

    vector<string> messages = 
    {
        "Fuck", "you", "you", "fucking", "cunt"
    };

    for (string const &message : messages)
    {
        DWORD sent_len = 0;

        int32_t msg_id = 0;
        uint32_t msg_size = message.length() + 4;
        uint32_t str_len = message.length();

        vector<char> data(12 + message.length());
        *reinterpret_cast< int32_t*>(&data[0]) = 0;
        *reinterpret_cast<uint32_t*>(&data[4]) = message.length() + 4;
        *reinterpret_cast<uint32_t*>(&data[8]) = message.length();
        for (size_t i = 0; i < message.length(); ++i)
            data[i + 12] = message[i];


        WriteFile(pipe, data.data(), data.size(), &sent_len, NULL);
        Sleep(2000);
    }

    cout << "Messages sent" << endl;

    CloseHandle(pipe);
    return 0;
}

