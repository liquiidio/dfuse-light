using System.Text;

namespace DeepReader;

public static class StringExtensions
{
    public static byte[] ToBytes(this string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }
}