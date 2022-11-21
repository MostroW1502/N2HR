namespace NES20HeaderRepair.NESHeaderRepair
{
    public static class BaseTypeExtensions
    {
        public static string ToHeaderString(this byte[] Header)
        {
            string t = string.Empty;
            foreach (byte b in Header) { t += $"{b:X2} "; }
            return t;
        }

        public static string ToSha1String(this byte[] Sha1)
        {
            string t = string.Empty;
            if (Sha1 == null || Sha1.Length < 1) return t;
            foreach (byte b in Sha1) { t += $"{b:X2}"; }
            return t;
        }

        public static byte[] GetBytes(this byte[] Data, int Offset, int Count)
        {
            byte[] result = new byte[Count];
            int current = 0;
            for (int i = Offset; i < Offset + Count; i++) { result[current++] = Data[i]; }
            return result;
        }

        public static ushort CalculateSUM16(this byte[] Data)
        {
            ushort sum = 0;
            foreach (byte b in Data) { sum += b; }
            return sum;
        }
    }
}