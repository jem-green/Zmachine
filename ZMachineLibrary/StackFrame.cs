using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZMachineLibrary
{
	[DataContract]
	internal class StackFrame
	{
		[DataMember]
		public uint PC { get; set; }
		[DataMember]
		public Stack<ushort> RoutineStack { get; set; }
		[DataMember]
		public ushort[] Variables { get; set; }
		[DataMember]
		public bool StoreResult { get; set; }
		[DataMember]
		public int ArgumentCount { get; set; }

		public StackFrame()
		{
			Variables = new ushort[0x10];
			RoutineStack = new Stack<ushort>();
			StoreResult = true;
		}
	}
}