using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TracerLibrary;

namespace ZMachineLibrary
{
    public class Memory
    {
        #region Fields

        byte[] memory;
        Dictionary<string, ushort> _address;

        #endregion
        #region Constructors

        public Memory(int size)
        {
            // Class constructor
            memory = new byte[size];
            _address = new Dictionary<string, ushort>();
        }

        #endregion
        #region Properties

        #endregion 
        #region Methods
        // Set an address structure

        public void SetAddress(byte version)
        {
            _address.Clear();
            if (version < 4)
            {
                _address.Add("ADDR_VERSION", 0x00);
                _address.Add("ADDR_HIGH", 0x04);
                _address.Add("ADDR_INITIALPC", 0x06);
                _address.Add("ADDR_DICT", 0x08);
                _address.Add("ADDR_OBJECTS", 0x0a);
                _address.Add("ADDR_GLOBALS", 0x0c);
                _address.Add("ADDR_STATIC", 0x0e);
                _address.Add("ADDR_ABBREVS", 0x18);
            }
            else if (version > 3)
            {
                _address.Add("ADDR_VERSION", 0x00);
                _address.Add("ADDR_HIGH", 0x04);
                _address.Add("ADDR_INITIALPC", 0x06);
                _address.Add("ADDR_DICT", 0x08);
                _address.Add("ADDR_OBJECTS", 0x0a);
                _address.Add("ADDR_GLOBALS", 0x0c);
                _address.Add("ADDR_STATIC", 0x0e);
                _address.Add("ADDR_ABBREVS", 0x18);
            }
        }

        public ushort GetAddress(string name)
        {
            ushort value = 0;
            try
            {
                value = _address[name];
            }
            catch
            { }

            return (value);          
        }



        //input byte array [] from file, output size specified byte array []
        public void Load(string filename)
        {
            //Load data into temp variable, copy into specified byte array

            if (File.Exists(filename) == true)
            {
                byte[] src = File.ReadAllBytes(filename);

                if (src.Length <= memory.Length)
                {

                    int i = 0;
                    foreach (byte b in src)
                    {
                        //read through bytes and write in new byte array []
                        memory[i] = src[i];
                        i++;
                    }
                }
            }
        }

        //assign given byte value @ hex address location
        public void SetByte(uint address, byte val)
        {
            memory[address] = val;
        }


        //access given byte location & return byte stored in data
        public byte GetByte(uint address)
        { 
            return (byte) memory[address];
        }


        //assign 16-bit value from given 16-bit memory address
        public void SetWord(uint address, ushort val)
        {
            byte a = (byte) ((val >> 8) & 0xff);
            byte b = (byte) (val & 0xff);

            // byte[] a = BitConverter.GetBytes(val);
            // byte first = (byte) a[0];
            // byte second = (byte) a[1];
            // first <<= 8;                             // shift first byte up 8 bits
            memory[address] = a;
            memory[address + 1] = b;     
        }


        public ushort GetWord(uint address)
        { 
            //access 16-bit value and return "word" stored in data
            //read in two bytes, shift first byte left (<<) 8 bits
            byte a = memory[address];
            byte b = memory[address + 1];
            ushort val = (ushort) ((a << 8) + b);

            return val;
        }

        public short GetSignedWord(uint address)
        {
            return (short)GetWord(address);
        }



        public string GetAbbrev(int abbrevIndex)
        {
            StringAndReadLength abbrev = GetZSCII((uint)(GetWord((uint)(GetWord(_address["ADDR_ABBREVS"]) + abbrevIndex * 2)) * 2), 0);
            return abbrev.str;
        }

        /// <summary>
        /// 
        /// </summary>
        public class StringAndReadLength
        {
            public String str = "";
            public int bytesRead = 0;   // We need to return how many bytes were read so that op_print can correctly place the pc after reading.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zchar"></param>
        /// <returns></returns>
        public char GetZChar(int zchar)
        {
            if (zchar == 0)         // null char
                return (char)0;
            else if (zchar == 8)    // delete char
                return (char)0;
            else if (zchar == 13)   // newline char
                return '\n';
            else if (zchar == 27)
                return (char)0;
            else if (zchar >= 129 && zchar <= 154)
                return (char)0;
            else if (
                zchar >= 32 && zchar <= 126 || // standard ascii
                zchar >= 155 && zchar <= 251)         // Take in 10-bit zscii and return ascii
                return (char)zchar;
            else
            {
                TraceInternal.TraceVerbose("Invalid 10-bit char: " + (char)zchar);
                return (char)0;
            }
        }

        public StringAndReadLength GetZSCII(uint address, uint numBytes)
        {
            String[] zalphabets = { "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", " \n0123456789.,!?_#'\"/\\-:()" };
            int currentAlphabet = 0;
            int bytesRead = 0;
            int read10Bit = 0;
            int abbrevSet = 0;
            int zchar10 = 0;

            String output = "";
            String debugOutput = "";
            int z = 0;

            bool stringComplete = false;
            while (!stringComplete)
            {
                //Get another word from memory
                ushort word = GetWord(address);
                if (((word >> 15) & 0x1) == 1) stringComplete = true;

                int[] c = new int[3];        // Store three zchars across 16 bits (two bytes). May have to make a dynamic list and pad it once every 3 reads.

                c[0] = (word >> 10) & 0x01f;
                c[1] = (word >> 5) & 0x01f;
                c[2] = word & 0x01f;

                TraceInternal.TraceVerbose("Reading Zchars \t\t 1:{0}\t 2:{1}\t 3:{2}", c[0], c[1], c[2]);

                /* ZSCII codes are 10-bit unsigned values between 0 and 1023.
                 * Summary of the ZSCII rules:
                 *  - The codes 0 to 31 are undefined in V3 except as follows - 
                 * 0 = null (output only)
                 * 13 = newline
                 * 32-126 = standard ASCII
                 * 155-251 = extra characters (other languages, punctuation marks, mathematical or dingbat characters)
                 * The codes 256 to 1023 are undefined, so that for all practical purposes ZSCII is an 8-bit unsigned code.
                 *       0123456789abcdef0123456789abcdef
                        --------------------------------
                 $20   !"#$%&'()*+,-./0123456789:;<=>?
                 $40  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_
                 $60  'abcdefghijklmnopqrstuvwxyz{!}~ 
                        --------------------------------
                 * 
                 * Note that if a random stretch of memory is accidentally printed as a string (due to an error in the story file), 
                 * illegal ZSCII codes may well be printed using the 4-Z-character escape sequence. It's helpful for interpreters 
                 * to filter out any such illegal codes so that the resulting on-screen mess will not cause trouble for the terminal 
                 * (e.g. by causing the interpreter to print ASCII 12, clear screen, or 7, bell sound).
                 */

                for (int i = 0; i < 3; i++)
                {

                    if (read10Bit > 0)
                    {
                        switch (read10Bit)
                        {
                            case 2:
                                {
                                    zchar10 = c[i] << 5;
                                    break;
                                }
                            case 1:
                                {
                                    zchar10 += c[i];

                                    output += GetZChar(zchar10);

                                    debugOutput += GetZChar(zchar10); z++;
                                    TraceInternal.TraceVerbose(debugOutput + "(" + z + ")");

                                    break;
                                }
                        }
                        --read10Bit;
                    }
                    else if (abbrevSet != 0)
                    {
                        int abbrevId = (abbrevSet - 1) * 32 + c[i];
                        //get the abbrev string using the current character and append it
                        output += GetAbbrev(abbrevId);
                        debugOutput += GetAbbrev(abbrevId); z++;
                        TraceInternal.TraceVerbose("Abbrev: |" + abbrevId + "| : " + GetAbbrev(abbrevId));

                        abbrevSet = 0;
                    }
                    else if (c[i] == 0)
                    {
                        output += " ";
                        debugOutput += (" "); z++;
                        TraceInternal.TraceVerbose("SPACE" + "(" + z + ")");

                    }
                    else if (c[i] == 1 || c[i] == 2 || c[i] == 3)  // Get abbreviation at given address using next two values
                    {
                        abbrevSet = c[i];
                    }
                    else if (c[i] == 4) //Change currentAlphabet = Uppercase (A1)
                        currentAlphabet = 1;
                    else if (c[i] == 5) //Change currentAlphabet = Punctuation (A2)
                        currentAlphabet = 2;
                    else if (c[i] == 6 && currentAlphabet == 2)
                    {
                        read10Bit = 2;
                    }
                    else
                    {
                        output += zalphabets[currentAlphabet][c[i] - 6];

                        debugOutput += zalphabets[currentAlphabet][c[i] - 6]; z++;
                        TraceInternal.TraceVerbose(debugOutput);
                        currentAlphabet = 0;
                    }

                }

                address += 2;               // move to next word
                bytesRead += 2;             // store number of bytesRead for pointer

                if (numBytes > 0 && bytesRead >= numBytes)
                {
                    stringComplete = true;
                    TraceInternal.TraceVerbose(debugOutput + "(" + z + ")");
                }

            }

            StringAndReadLength ret = new StringAndReadLength
            {
                bytesRead = bytesRead,
                str = output
            };

            return ret;
        }

        // Print the file header
        public void DumpHeader()
        {
            TraceInternal.TraceVerbose("Type : " + GetByte(_address["ADDR_VERSION"]));
            TraceInternal.TraceVerbose("Base : " + GetWord(_address["ADDR_HIGH"]));
            TraceInternal.TraceVerbose("PC   : " + GetWord(_address["ADDR_INITIALPC"]));
            TraceInternal.TraceVerbose("Dict : " + GetWord(_address["ADDR_DICT"]));
            TraceInternal.TraceVerbose("Obj  : " + GetWord(_address["ADDR_OBJECTS"]));
        }

        public uint GetCrc32()
        {
            Crc32 crc32 = new Crc32();
            return crc32.ComputeChecksum(memory);
        }
        #endregion
    }
}
