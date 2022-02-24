/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

namespace Niflib
{
    using System;
    using System.Data;
    using System.IO;

    /// <summary>
    /// Class NiHeader.
    /// </summary>
    public class NiHeader
    {
        /// <summary>
        /// The version string
        /// </summary>
        public string VersionString;

        /// <summary>
        /// The version
        /// </summary>
        public eNifVersion Version = (eNifVersion)4294967295u;

        /// <summary>
        /// The user version
        /// </summary>
        public uint UserVersion;

        /// <summary>
        /// The user version2
        /// </summary>
        public uint UserVersion2;

        /// <summary>
        /// The block types
        /// </summary>
        public NiString[] BlockTypes;

         /// <summary>
        /// The max indexed string length
        /// </summary>
        public uint MaxStringLength;

        /// <summary>
        /// The strings
        /// </summary>
        public NiString[] Strings;

        /// <summary>
        /// The block type index
        /// </summary>
        public ushort[] BlockTypeIndex;

        /// <summary>
        /// The block sizes
        /// </summary>
        public uint[] BlockSizes;

        /// <summary>
        /// The number blocks
        /// </summary>
        public uint NumBlocks;

        /// <summary>
        /// The unkown int
        /// </summary>
        public uint UnkownInt;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiHeader"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// or
        /// NIF Version not supported yet!
        /// </exception>
        /// <exception cref="VersionNotFoundException">Version 20.0.0.5 not supported!</exception>
        public NiHeader(NiFile file, BinaryReader reader)
        {
            int num = 0;
            long position = reader.BaseStream.Position;
            while (reader.ReadByte() != 10)
            {
                num++;
            }
            reader.BaseStream.Position = position;
            VersionString = new string(reader.ReadChars(num));
            reader.ReadByte();
            uint version = reader.ReadUInt32();
            Version = (eNifVersion)version;
            if ((int)Version <= 0x03010000)
            {
                for (uint i2 = 0; i2 < 3; i2++)
                {
                    reader.ReadString();
                }
            }
            if ((int)Version >= 0x14000004)
            {
                byte endianness = reader.ReadByte();
            }
            if ((int)Version >= 0x0A010000)
            {
                UserVersion = reader.ReadUInt32();
            }
            if ((int)Version >= 0x0303000D)
            {
                NumBlocks = reader.ReadUInt32();
            }
            if ((int)Version >= 0x0A010000)
            {
                if ((UserVersion >= 10) || ((UserVersion == 1) && (version != 0x0A020000)))
                {
                    UserVersion2 = reader.ReadUInt32();
                }
            }
            if ((int)Version >= 0x1E000002)
            {
                int unkInt = reader.ReadInt32();
            }
            if (((int)Version >= 0x0A000102) && ((int)Version <= 0x0A000102))
            {
                if ((int)Version <= 0x0A000102)
                {
                    int unk = reader.ReadInt32();
                }
                byte len = reader.ReadByte();
                reader.ReadBytes(len);
                len = reader.ReadByte();
                reader.ReadBytes(len);
                len = reader.ReadByte();
                reader.ReadBytes(len);
            }
            if ((int)Version >= 0x0A010000)
            {
                if (((UserVersion >= 10) || ((UserVersion == 1) && ((int)Version != 0x0A020000))))
                {
                    if ((int)Version <= 0x0A000102)
                    {
                        int unk = reader.ReadInt32();
                    }
                    byte len = reader.ReadByte();
                    reader.ReadBytes(len);
                    len = reader.ReadByte();
                    reader.ReadBytes(len);
                    len = reader.ReadByte();
                    reader.ReadBytes(len);
                }
            }
            if ((int)Version >= 0x0A000100)
            {
                ushort num2 = reader.ReadUInt16();
                BlockTypes = new NiString[(int)num2];
                for (int i = 0; i < (int)num2; i++)
                {
                    BlockTypes[i] = new NiString(file, reader);
                }
                BlockTypeIndex = new ushort[NumBlocks];
                int num3 = 0;
                while ((long)num3 < (long)((ulong)NumBlocks))
                {
                    BlockTypeIndex[num3] = reader.ReadUInt16();
                    num3++;
                }
            }
            if ((int)Version >= 0x14020007)
            {
                BlockSizes = new uint[NumBlocks];
                for (uint i2 = 0; i2 < NumBlocks; i2++)
                {
                    BlockSizes[i2] = reader.ReadUInt32();
                };
            }
            if ((int)Version >= 0x14010003)
            {
                uint numStrings = reader.ReadUInt32();
                MaxStringLength = reader.ReadUInt32();
                Strings = new NiString[numStrings];
                for (int i = 0; i < numStrings; ++i)
                {
                   Strings[i] = new NiString(file, reader);
                }
            }
            if ((int)Version >= 0x0A000100)
            {
                UnkownInt = reader.ReadUInt32();
            }
        }
    }
}
