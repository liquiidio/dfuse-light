using System.Text;

namespace DeepReader.Classes;

public static class StringExtensions
{
    public static byte[] ToBytes(this string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }
}