using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using TracerLibrary;

namespace ZMachineLibrary
{
    public class ObjectTable
    {
        #region Fields
        Memory memory = new Memory(1024 * 128);
        public int tp = 0;                                 // pointer to move through tables
        public int objectId = 0;                           // Object ID

        #endregion
        #region Constructors
        public ObjectTable (Memory mem)
        {
            memory = mem;
        }
        #endregion

        public int GetDefaultProperty(int property)     // Start reading from the Program Defaults Table (before list of objects)
        {
            tp = memory.GetWord((uint)memory.GetAddress("ADDR_OBJECTS"));
            tp += (property - 1) * 2;
            int defaultProperty = TpGetWord();
            return defaultProperty;
        }

        /// <summary>
        /// getObjectTable
        /// </summary>
        /// <returns></returns>
        public int GetObjectTable()                     // Set Table Pointer to the beginning of the object table. (After Default Properties)
        {
            tp = memory.GetWord((uint)memory.GetAddress("ADDR_OBJECTS")) + (31 * 2);
            return tp;
        }

        /// <summary>
        /// GetPropertyTableAddress
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public int GetPropertyTableAddress(int objectId)// Return property table address for given object. Leaves Table Pointer at beginning of next property.
        {
            GetObjectTable();

            tp = GetObjectAddress(objectId) + 7;
            int propertyTableAddress = TpGetWord();
          return propertyTableAddress;
        }

        /// <summary>
        /// getObjectAddress
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public int GetObjectAddress(int objectId)       // take an objectId and consult the objectTable
        {          
            GetObjectTable();
            tp += 9 * (objectId - 1);             // tp will already be set at the start of the object table
            int objectAddress = tp;              // each object is 9 bytes above the previous    

            return objectAddress;
        }

        /// <summary>
        /// getObjectPropertyAddress
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public int GetObjectPropertyAddress(int objectId, int property)  // Get property address for an object's property (if it exists)
        {
            int propertyAddress; 
            tp = GetPropertyTableAddress(objectId);
            int text_length = TpGetByte();  // Read the first byte of the property address, figure out the text-length, then skip forward to the first property address.
            tp += (text_length * 2);         // skip past header

            int sizeByte = TpGetByte();     // get initial sizeByte (property 1)
            while (sizeByte > 0)             // Let the search begin! Move through properties looking for a propertyId matching the property given.
            {
                int propertyId = sizeByte & 31;
                int propLen = (sizeByte / 32) + 1;
                if (property == propertyId)
                {
                    propertyAddress = tp;
                    return propertyAddress;              
                }
                tp += propLen;
                sizeByte = TpGetByte();
            }
                return 0;
        }

        /// <summary>
        /// getNextObjectPropertyIdAfter
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public int GetNextObjectPropertyIdAfter(int objectId, int property) // Get property address down the chain given a certain object.
        {
            int propLen;
            int nextPropertyId;
            byte sizeByte;

            if (property == 0)      // Return first property
            {
                tp = GetPropertyTableAddress(objectId);
                int text_length = TpGetByte();  // Read the first byte of the property address, figure out the text-length, then skip forward to the first property address.
                tp += (text_length * 2);         // skip past header
                sizeByte = TpGetByte();

                return sizeByte & 31;       // read property id
            }

            // Since we are not at the start of the property we want...
            tp = GetObjectPropertyAddress(objectId, property);              // Set pointer to start of given property
            sizeByte = memory.GetByte((uint)tp - 1);       // Find length of property
            propLen = GetObjectPropertyLengthFromAddress(tp);
             
                if (sizeByte == 0)                    // No next property
                    return 0;

            tp += propLen;                            // Move pointer to start next property   
            nextPropertyId = TpGetByte() & 31;       // Read next property number
            
            return nextPropertyId;
        }

        /// <summary>
        /// getObjectPropertyLengthFromAddress
        /// </summary>
        /// <param name="propertyAddress"></param>
        /// <returns></returns>
        public int GetObjectPropertyLengthFromAddress(int propertyAddress)
        {
            if (propertyAddress == 0)
            {
                return 0;
            }
            int sizeByte = memory.GetByte((uint)propertyAddress - 1);  // arranged as 32 times the number of data bytes minus one, plus the property number
            int propLen = (sizeByte / 32) + 1;
            return propLen;
        }

        /// <summary>
        /// getObjectProperty
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public int GetObjectProperty(int objectId, int property)
        {
            // each property is stored as a block
            //size byte     the actual property data
            //---between 1 and 8 bytes--

            int propertyAddress = GetObjectPropertyAddress(objectId, property);

            if (propertyAddress == 0)
            {
                return GetDefaultProperty(property); 
            }

            int propertyData;
            int propLen = GetObjectPropertyLengthFromAddress(propertyAddress);  // get size of property

            if (propLen == 1)                                   // Return size of property (byte or word)
                propertyData = TpGetByte();
            else if (propLen == 2)
                propertyData = TpGetWord();
            else
            {
                propertyData = 0;
                TraceInternal.TraceVerbose("Property Length " + propLen + " is an unspecified length call for opcodes.");
            }

            return propertyData;
        }

        /// <summary>
        /// getParent
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public int GetParent(int objectId)
        {
            // version 3 = 4, version 5 = 6
            tp = GetObjectAddress(objectId) + 4;        // move past object header (4 attribute bytes)
            return TpGetByte();
        }

        /// <summary>
        /// getSibling
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public int GetSibling(int objectId)
        {
            // version 3 = 5, version 5 = 8
            tp = GetObjectAddress(objectId) + 5;                           // move past object header (4 attribute bytes + parent byte)
            return TpGetByte();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public int GetChild(int objectId)
        {
            // version 3 = 6, version 5 = 10
            tp = GetObjectAddress(objectId) + 6;                           // move past object header (4 attribute bytes + parent byte + sibling byte)
            return TpGetByte();
        }

        /// <summary>
        /// GetObjectAttribute
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public bool GetObjectAttribute(int objectId, int attributeId)
        {
            tp = GetObjectAddress(objectId);
            bool [] attributes = new bool[32];
            byte attributeByte;

            // Why not just make a boolean array of the attributes?!
            for (int i = 0; i < 4; i++)
            {
                attributeByte = TpGetByte();
                for (int j = 0; j < 8; j++)
                {
                    attributes[i * 8 + j] = ((attributeByte >> (7 - j)) & 0x01) > 0;// Take the next byte and cut the first (MSB) value off the top. Add to boolean array attribute[] and voila!
                }
            }
            
            return attributes[attributeId];
        }

        /// <summary>
        /// setObjectAttribute
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="attributeId"></param>
        /// <param name="value"></param>
        public void SetObjectAttribute(int objectId, int attributeId, bool value)
        {
            byte a;                     // If can find the right byte in the memory, I can bitwise AND or NOT to fill or clear it.
            uint address = (uint)GetObjectAddress(objectId);
            TraceInternal.TraceVerbose("BEFORE address: " + address + " " + memory.GetByte(address) + "," + memory.GetByte(address + 1) + "," + memory.GetByte(address + 2) + "," + memory.GetByte(address + 3));
            byte attributeByte = (byte)(attributeId / 8);           // The byte of the attribute header that we are working in
            int attributeShift = 7 - (attributeId % 8);

            if (value == true)
            {
                a = memory.GetByte(address + (uint)attributeByte);        // Read the byte given by the attribute segment our attribute Id is in.
                a |= (byte)(1 << attributeShift);                      // Fill a bit at the given shift in that byte.
            }
            else 
            {
                a = memory.GetByte(address + (uint)attributeByte);
                a &= (byte)~(1 << attributeShift);
            }
            TraceInternal.TraceVerbose("AFTER address: " + address + " " + memory.GetByte(address) + "," + memory.GetByte(address + 1) + "," + memory.GetByte(address + 2) + "," + memory.GetByte(address + 3));
            memory.SetByte(address + attributeByte, a);
        }

        /// <summary>
        /// setObjectProperty
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetObjectProperty(int objectId, int property, ushort value)
        {
            int propertyAddress = GetObjectPropertyAddress(objectId, property);

            if (propertyAddress == 0)
            {
                property = GetDefaultProperty(property);
            }
            int propLen = GetObjectPropertyLengthFromAddress(propertyAddress);  // get size of property

            if (propLen == 1)                                   // Check size of property
                memory.SetWord((uint)propertyAddress, (ushort)(value & 0xff));
            else if (propLen == 2)
                memory.SetWord((uint)propertyAddress, value);            
        }

        /// <summary>
        /// setParent
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="parentId"></param>
        public void SetParent(int objectId, int parentId)
        {
            tp = GetObjectAddress(objectId) + 4;                           // move past object header (4 attribute bytes)
            memory.SetByte((uint)tp, (byte)parentId);
        }

        /// <summary>
        /// setChild
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="siblingId"></param>
        public void SetSibling(int objectId, int siblingId)
        {
            tp = GetObjectAddress(objectId) + 5;                           // move past object header (4 attribute bytes)
            memory.SetByte((uint)tp, (byte)siblingId);
        }

        /// <summary>
        /// setChild
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="childId"></param>
        public void SetChild(int objectId, int childId)
        {
            tp = GetObjectAddress(objectId) + 6;                           // move past object header (4 attribute bytes + parent byte + sibling byte)
            memory.SetByte((uint)tp, (byte)childId);
        }

        // -------------------------- 
        /// <summary>
        /// tp_getByte
        /// </summary>
        /// <returns></returns>
        public byte TpGetByte()
        {
            byte next = memory.GetByte((uint)tp);
            tp++;
            return (byte)next;
        }

        /// <summary>
        /// tp_getWord
        /// </summary>
        /// <returns></returns>
        public ushort TpGetWord()
        {
            ushort next = memory.GetWord((uint)tp);
            tp += 2;
            return next;
        }

        public String ObjectName(int objectId)   
        {
            String name;
            if (objectId == 0)
            {
                return "Unable to find Object";
            }
            Memory.StringAndReadLength str = new Memory.StringAndReadLength();
            tp = GetPropertyTableAddress(objectId);            // An object's name is stored in the header of its property table, and is given in the text-length.
            int textLength = TpGetByte();                     // The first byte is the text-length number of words in the short name
            if (textLength == 0)
            {
                return "Name not found";                        
            }
            name = memory.GetZSCII((uint)tp, (uint)textLength * 2).str;
            return name;
        }

        public void UnlinkObject(int objectId)
        {
            // Get parent of object. If no parent, no need to unlink it.
            int parentId = GetParent(objectId);
            if (parentId == 0)
                return;

            // Get next sibling
            int nextSibling = GetSibling(objectId);

            // Get very first sibling in list
            int firstSibling = GetChild(parentId);

            // Clear out the parent/sibling fields since they'll no longer be valid when this is done.
            SetParent(objectId, 0);
            SetSibling(objectId, 0);

            // Remove object from the list of siblings
            if (firstSibling == objectId)   // If this object is the first child, just set it to the next child.
                SetChild(parentId, nextSibling);
            else
            {
                int sibId = firstSibling;
                int lastSibId = sibId;
                do
                {
                    lastSibId = sibId;
                    sibId = GetSibling(sibId);
                } while (sibId != objectId);
                SetSibling(lastSibId, nextSibling);
            }
        }

    }
}
