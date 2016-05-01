#include "stdafx.h"

typedef vector<char> bytes_t;

template<typename T>
void append_primitive(T value, bytes_t &dst)
{
    size_t old_size = dst.size();
    dst.resize(dst.size() + sizeof(value));
    *reinterpret_cast<T*>(&dst.at(old_size)) = value;
}

template<typename T>
void append(T value, bytes_t &dst, typename std::enable_if<std::is_fundamental<T>::value>::type* = 0)
{
    append_primitive(value, dst);
}

void append(string const &str, bytes_t &dst)
{
    append(uint32_t(str.size()), dst);
    std::copy(str.begin(), str.end(), std::back_inserter(dst));
}

typedef int32_t message_id_t;
typedef uint32_t message_size_t;

template<message_id_t Id>
struct message_base_t
{
    static const message_id_t msg_id = Id;
};

struct text_message_t
    : message_base_t<0>
{
    explicit text_message_t(string const str)
        : str(str)
    {}

    friend void append(text_message_t const &message, bytes_t &dst)
    {
        append(message.str, dst);
    }
    
    string str;
};


template<typename T>
void append_message(T const &message, bytes_t &dst)
{
    size_t start_offset = dst.size();
    
    append(message_size_t(0), dst);
    append(message.msg_id, dst);
    append(message, dst);

    message_size_t message_size = dst.size() - sizeof(message_size_t) - sizeof(message_id_t);
    *reinterpret_cast<message_size_t*>(&dst.at(start_offset)) = message_size;
}


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

    vector<char const*> strs = 
    {
        "Fuck", "you", "you", "fucking", "cunt"
    };

    bytes_t bytes;
    for (char const *str : strs)
    {
        text_message_t msg(str);

        bytes.clear();
        append_message(msg, bytes);
        
        DWORD sent_len = 0;

        cout << "sent: " << str << endl;
        WriteFile(pipe, bytes.data(), bytes.size(), &sent_len, NULL);
        Sleep(2000);
    }

    cout << "Messages sent" << endl;

    CloseHandle(pipe);
    return 0;
}

