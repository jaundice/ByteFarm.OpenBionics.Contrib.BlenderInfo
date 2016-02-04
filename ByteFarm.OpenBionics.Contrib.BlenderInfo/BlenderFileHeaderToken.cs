namespace ByteFarm.OpenBionics.Contrib.BlenderInfo
{
    public class BlenderFileHeaderToken : BlenderToken
    {
        public BlenderFileHeaderToken(string identifer, int sz, Endianness endianness, string version)
        {
            Identifier = identifer;
            PointerSize = sz;
            Endianness = endianness;
            Version = version;
        }

        public string Identifier { get; }
        public int PointerSize { get; }
        public Endianness Endianness { get; }
        public string Version { get; }

        public override string ToString()
        {
            return
                $"Identifier: {Identifier} \r\nPointerSize:{PointerSize}\r\nEndianness:{Endianness}\r\nVersion:{Version}";
        }
    }
}