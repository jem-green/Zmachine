using System.Collections.Generic;

namespace ZMachineLibrary
{
	internal delegate void OpcodeHandler(List<ushort> args);

	internal struct Opcode
	{
		public string Name { get; set; }
		public OpcodeHandler Handler { get; set; }
	}
}