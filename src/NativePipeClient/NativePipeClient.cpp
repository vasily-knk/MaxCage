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

        uint32_t size = message.length();
        WriteFile(pipe, &size, 4, &sent_len, NULL);
        WriteFile(pipe, message.c_str(), size, &sent_len, NULL);
        Sleep(2000);
    }

    cout << "Messages sent" << endl;

    CloseHandle(pipe);
    return 0;
}

