using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/*
https://svn.blender.org/svnroot/bf-blender/trunk/blender/doc/blender_file_format/mystery_of_the_blend.html
*/

namespace ByteFarm.OpenBionics.Contrib.BlenderInfo
{
    public class BlenderReader
    {
        private readonly string _filePath;

        public BlenderReader(string path)
        {
            _filePath = path;
        }


        public IEnumerable<BlenderToken> ReadStructure()
        {
            var fs = File.OpenRead(_filePath);
            try
            {
                var fileHeader = ReadFileHeader(fs);
                yield return fileHeader;

                while (fs.Position < fs.Length)
                {
                    yield return ReadFileBlock(fs, fileHeader);
                }
            }
            finally
            {
                fs.Close();
            }
        }

        private BlenderFileBlockToken ReadFileBlock(FileStream fs, BlenderFileHeaderToken fileHeader)
        {
            /*
            code	char[4]	File-block identifier	0	4
 size	integer	Total length of the data after the file-block-header	4	4
 old memory address	void*	Memory address the structure was located when written to disk	8	pointer-size (4/8)
 SDNA index	integer	Index of the SDNA structure	8+pointer-size	4
 count	integer	Number of structure located in this file-block	12+pointer-size	4

            */

            var identifier = new byte[4];
            fs.Read(identifier, 0, identifier.Length);
            var id = Encoding.ASCII.GetString(identifier);

            var dataLength = ReadInt(fs, fileHeader.Endianness);

            var memoryAddress = fileHeader.PointerSize == 4
                ? ReadInt(fs, fileHeader.Endianness)
                : ReadLong(fs, fileHeader.Endianness);

            var sdnaIndex = ReadInt(fs, Endianness.LittleEndian);

            var count = ReadInt(fs, fileHeader.Endianness);

            var data = new byte[dataLength];
            fs.Read(data, 0, data.Length);

            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data, 0, data.Length);

            return new BlenderFileBlockToken(id, dataLength, memoryAddress, sdnaIndex, count,
                BitConverter.ToString(hash));
        }

        private int ReadInt(FileStream fs, Endianness endianness)
        {
            var buffer = new byte[4];
            fs.Read(buffer, 0, buffer.Length);

            if ((endianness == Endianness.LittleEndian && BitConverter.IsLittleEndian) ||
                (endianness == Endianness.BigEndian && !BitConverter.IsLittleEndian))
                return BitConverter.ToInt32(buffer, 0);

            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        private long ReadLong(FileStream fs, Endianness endianness)
        {
            var buffer = new byte[8];
            fs.Read(buffer, 0, buffer.Length);

            if ((endianness == Endianness.LittleEndian && BitConverter.IsLittleEndian) ||
                (endianness == Endianness.BigEndian && !BitConverter.IsLittleEndian))
                return BitConverter.ToInt64(buffer, 0);

            Array.Reverse(buffer);


            return BitConverter.ToInt64(buffer, 0);
        }

        private BlenderFileHeaderToken ReadFileHeader(Stream s)
        {
            /*
            identifier	char[7]	File identifier (always 'BLENDER')	0	7
pointer-size	char	Size of a pointer; all pointers in the file are stored in this format. '_' means 4 bytes or 32 bit and '-' means 8 bytes or 64 bits.	7	1
endianness	char	Type of byte ordering used; 'v' means little endian and 'V' means big endian.	8	1
version-number	char[3]	Version of Blender the file was created in; '254' means version 2.54	9	3

    */
            var identifer = new byte[7];
            s.Read(identifer, 0, identifer.Length);
            var id = Encoding.ASCII.GetString(identifer);

            var pointerSize = (char)s.ReadByte();
            var sz = pointerSize == '_' ? 4 : pointerSize == '-' ? 8 : -1;

            if (sz == -1)
                throw new Exception("Invalid PointerSize value read.");

            var e = (char)s.ReadByte();
            var endianness = (e == 'v' ? Endianness.LittleEndian : e == 'V' ? Endianness.BigEndian : Endianness.Unknown);

            if (endianness == Endianness.Unknown)
                throw new Exception("Invalid Endianness value read");

            var ver = new byte[3];
            s.Read(ver, 0, ver.Length);

            var version = Encoding.ASCII.GetString(ver);
            return new BlenderFileHeaderToken(id, sz, endianness, version);
        }
    }
}