using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Pair<T1, T2>
{
    public T1 first;
    public T2 second;

    public Pair(T1 first, T2 second)
    {
        this.first = first;
        this.second = second;
    }
}

public static class Pair
{
    public static Pair<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Pair<T1, T2>(first, second);
        return tuple;
    }
}
