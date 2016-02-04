namespace ByteFarm.OpenBionics.Contrib.BlenderInfo
{
    public class BlenderFileBlockToken : BlenderToken
    {
        public BlenderFileBlockToken(string id, int dataLength, long memoryAddress, int sdnaIndex, int count,
            string hash)
        {
            Id = id;
            DataLength = dataLength;
            MemoryAddress = memoryAddress;
            SDNAIndex = sdnaIndex;
            Count = count;
            DataHash = hash;
        }

        public string DataHash { get; }

        public int Count { get; }

        public int SDNAIndex { get; }

        public long MemoryAddress { get; }

        public int DataLength { get; }

        public string Id { get; }

        public override string ToString()
        {
            return
                $"Identifier:{Id}\r\nDataLength:{DataLength}\r\nMemoryAddress:{MemoryAddress}\r\nSDNAIndex:{SDNAIndex}\r\nCount:{Count}\r\nDataHash:{DataHash}";
        }
    }
}