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
    

    char a = 117;
    DWORD len = 0;
    WriteFile(pipe, &a, 1, &len, NULL);

    CloseHandle(pipe);
    return 0;
}

