using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ZMachineLibrary
{
    partial class Machine
    {
        private abstract class OpcodeHandler
        {
            abstract public String Name();
            public void FailUnimplemented(Machine machine)
            {
                machine.finish = true;
                log.Error("Unimplemented function: " + Name());
            }
        }

        private abstract class OpcodeHandler_0OP : OpcodeHandler
        {
            abstract public void Run(Machine machine, IConsoleIO consoleIO);
        }

        private abstract class OpcodeHandler_1OP : OpcodeHandler
        {
            abstract public void Run(Machine machine, IConsoleIO consoleIO, ushort v1);
        }

        private abstract class OpcodeHandler2OP : OpcodeHandler
        {
            // Implement one or the other of these:
            public virtual void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                FailUnimplemented(machine);
            }
            public virtual void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                Run(machine, consoleIO, operands[0], operands[1]);
            }
        }

        private abstract class OpcodeHandlerOPVAR : OpcodeHandler
        {
            abstract public void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands);
        }

        // Unknown OP Classes
        private class OpUnknown2op : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "UNKNOWN 2OP";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                FailUnimplemented(machine);
            }
        }

        private class OpUnknown1op : OpcodeHandler_1OP
        {
            public override String Name()
            {
                return "UNKNOWN 1OP";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                FailUnimplemented(machine);
            }
        }

        private class OpUnknown0op : OpcodeHandler_0OP
        {
            public override String Name()
            {
                return "UNKNOWN 0OP";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }

        private class OpUnknownOpVar : OpcodeHandlerOPVAR
        {
            public override String Name()
            {
                return "UNKNOWN OPVAR";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                FailUnimplemented(machine);
            }
        }

        // 2OP Classes
        // Branch Opcodes 1 - 7, 10 
        // Store Opcodes 8, 9, 15 - 25

        /// <summary>
        /// op_je
        /// </summary>
        private class OpJe : OpcodeHandler2OP
        {
            string op = "op_je";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                log.Debug("in Run()");
                bool branchOn = false;
                for (int i = 1; i < operands.Count; i++)
                {
                    if (operands[0] == operands[i])
                    {
                        branchOn = true;
                    }
                }
                log.Info(op);
                machine.Branch(branchOn);
                log.Debug("Out Run");
            }
        }

        /// <summary>
        /// op_jl
        /// </summary>
        private class OpJl : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_jl";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.Branch((short)v1 < (short)v2);
            }
        }

        /// <summary>
        /// op_jg
        /// </summary>
        private class OpJg : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_jg";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.Branch((short)v1 > (short)v2);
            }
        }

        /// <summary>
        /// op_dec_chk
        /// </summary>
        private class OpDecChk : OpcodeHandler2OP
        {
            string op = "op_dec_chk";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                log.Debug("In Run");
                log.Info("op");
                int value = ((short)machine.GetVar(v1)) - 1;
                machine.SetVar(v1, (ushort)value);
                machine.Branch(value < v2);
                log.Debug("out Run");
            }
        }

        /// <summary>
        /// op_inc_chk
        /// </summary>
        private class OpIncChk : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_inc_chk";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                int value = ((short)machine.GetVar(v1)) + 1;
                machine.SetVar(v1, (ushort)value);
                machine.Branch(value > v2);
            }
        }

        /// <summary>
        /// op_jin
        /// </summary>
        private class OpJin : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_jin";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.Branch(machine.objectTable.GetParent(v1) == v2);
            }
        }

        /// <summary>
        /// op_test
        /// </summary>
        private class OpTest : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_test";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.Branch((v1 & v2) == v2);
            }
        }

        private class OpOr : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_or";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)(v1 | v2));
            }
        }

        private class OpAnd : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_and";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)(v1 & v2));
            }
        }

        /// <summary>
        /// op_test_attr
        /// </summary>
        private class OpTestAttr : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_test_attr";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                log.Debug("Looking for attribute in obj " + v1 + " attribute:" + machine.objectTable.GetObjectAttribute(v1, v2));
                machine.Branch(machine.objectTable.GetObjectAttribute(v1, v2) == true);
            }
        }

        /// <summary>
        /// op_set_attr
        /// </summary>
        private class OpSetAttr : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_set_attr";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.objectTable.SetObjectAttribute(v1, v2, true);
            }
        }

        /// <summary>
        /// op_clear_attr
        /// </summary>
        private class OpClearAttr : OpcodeHandler2OP
        {
            public override String Name()
            {
                return "op_clear_attr";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.objectTable.SetObjectAttribute(v1, v2, false);
            }
        }

        /// <summary>
        /// op_store
        /// </summary>
        private class OpStore : OpcodeHandler2OP
        {
            public override String Name() { return "op_store"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.SetVar(v1, v2);
            }
        }

        /// <summary>
        /// op_insert_obj
        /// </summary>
        private class OpInsertObj : OpcodeHandler2OP
        {
            public override String Name() { return "op_insert_obj"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                int newSibling = machine.objectTable.GetChild(v2); // 
                machine.objectTable.UnlinkObject(v1);
                machine.objectTable.SetChild(v2, v1);
                machine.objectTable.SetParent(v1, v2);
                machine.objectTable.SetSibling(v1, newSibling);
                // after the operation the child of v2 is v1
                // and the sibling of v1 is whatever was previously the child of v2.
                // All children of v1 move with it. (Initially O can be at any point in the object tree; it may legally have parent zero.)    
            }
        }

        /// <summary>
        /// op_loadw
        /// </summary>
        private class OpLoadw : OpcodeHandler2OP
        {
            public override String Name() { return "op_loadw"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                ushort value = machine.memory.GetWord((uint)v1 + (uint)(2 * v2));
                machine.SetVar((ushort)machine.PcGetByte(), value);
            }
        }

        /// <summary>
        /// op_loadb
        /// </summary>
        private class OpLoadb : OpcodeHandler2OP
        {
            public override String Name() { return "op_loadb"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                {
                    ushort value = machine.memory.GetByte((uint)v1 + (uint)v2);
                    machine.SetVar((ushort)machine.PcGetByte(), value);
                }
            }
        }

        /// <summary>
        /// op_get_prop
        /// </summary>
        private class OpGetProp : OpcodeHandler2OP
        {
            public override String Name() { return "op_get_prop"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetObjectProperty(v1, v2));
            }
        }

        /// <summary>
        /// op_get_prop_addr
        /// </summary>
        private class OpGetPropAddr : OpcodeHandler2OP
        {
            public override String Name() { return "op_get_prop_addr"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetObjectPropertyAddress(v1, v2));
            }
        }

        /// <summary>
        /// op_get_next_addr
        /// </summary>
        private class OpGetNextAddr : OpcodeHandler2OP
        {
            public override String Name() { return "op_get_next_addr"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2) { machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetNextObjectPropertyIdAfter(v1, v2)); }
        }

        /// <summary>
        /// op_add
        /// </summary>
        private class OpAdd : OpcodeHandler2OP
        {
            public override String Name() { return "op_add"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2) { machine.SetVar(machine.PcGetByte(), (ushort)((short)v1 + (short)v2)); }
        }

        /// <summary>
        /// op_sub
        /// </summary>
        private class OpSub : OpcodeHandler2OP
        {
            public override String Name() { return "op_sub"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                int result = (short)v1 - (short)v2;
                machine.SetVar(machine.PcGetByte(), (ushort)result);
            }
        }

        /// <summary>
        /// op_div
        /// </summary>
        private class OpDiv : OpcodeHandler2OP
        {
            string op = "op_div";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                log.Debug("In Run()");
                log.Info(op);
                if (v2 != 0)
                {
                    int result = (short)v1 / (short)v2;
                    machine.SetVar(machine.PcGetByte(), (ushort)result);
                }
                log.Debug("Out Run()");
            }
        }

        /// <summary>
        /// op_mod
        /// </summary>
        private class OpMod : OpcodeHandler2OP
        {
            public override String Name() { return "op_mod"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                if (v2 == 0) machine.finish = true;         // Interpreter cannot div by 0

                int result = (short)v1 % (short)v2;
                machine.SetVar(machine.PcGetByte(), (ushort)result);
            }
        }

        private class OpMul : OpcodeHandler2OP
        {
            public override String Name() { return "op_mul"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1, ushort v2)
            {
                int result = (short)v1 * (short)v2;
                machine.SetVar(machine.PcGetByte(), (ushort)result);
            }
        }

        // 1OP Classes
        // Branch Opcodes 128 - 130 
        // Store Opcodes 129 - 132, 136, 142 - 143
        private class OpJz : OpcodeHandler_1OP
        {
            public override String Name() { return "op_jz"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.Branch(v1 == 0);
            }
        }

        private class OpGetSibling : OpcodeHandler_1OP
        {
            public override String Name() { return "op_get_sibling"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetSibling(v1));
                machine.Branch(machine.objectTable.GetSibling(v1) != 0);
            }
        }

        private class OpGetChild : OpcodeHandler_1OP
        {
            public override String Name() { return "op_get_child"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetChild(v1));
                machine.Branch(machine.objectTable.GetChild(v1) != 0);
            }
        }
        private class OpGetParent : OpcodeHandler_1OP
        {
            public override String Name() { return "op_get_parent"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1) { machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetParent(v1)); }
        }

        private class OpGetPropLen : OpcodeHandler_1OP
        {
            string op = "op_get_prop_len";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                log.Debug("In Run");
                log.Info(op);
                machine.SetVar(machine.PcGetByte(), (ushort)machine.objectTable.GetObjectPropertyLengthFromAddress(v1));
                log.Debug("Out Run");
            }
        }

        /// <summary>
        /// op_inc
        /// </summary>
        private class OpInc : OpcodeHandler_1OP
        {
            public override String Name() { return "op_inc"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                int value = ((short)machine.GetVar(v1)) + 1;
                machine.SetVar(v1, (ushort)value);
            }
        }

        /// <summary>
        /// op_dec
        /// </summary>
        private class OpDec : OpcodeHandler_1OP
        {
            public override String Name() { return "op_dec"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                int value = ((short)machine.GetVar(v1)) - 1;
                machine.SetVar(v1, (ushort)value);
            }
        }

        /// <summary>
        /// op_not
        /// </summary>
        private class OpNot : OpcodeHandler_1OP
        {
            public override String Name() { return "op_not"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.SetVar(machine.PcGetByte(), (ushort)(~v1));
            }
        }

        /// <summary>
        /// op_print_addr
        /// </summary>
        private class OpPrintAddr : OpcodeHandler_1OP
        {
            public override String Name() { return "op_print_addr"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                consoleIO.Out(machine.memory.GetZSCII(v1, 0).str);
            }
        }

        /// <summary>
        /// op_remove_obj
        /// </summary>
        private class OpRemoveObj : OpcodeHandler_1OP
        {
            public override String Name() { return "op_remove_obj"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.objectTable.SetParent(v1, 0);
            }
        }

        /// <summary>
        /// op_print_obj
        /// </summary>
        private class OpPrintObj : OpcodeHandler_1OP
        {
            public override String Name() { return "op_print_obj"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                consoleIO.Out(machine.objectTable.ObjectName(v1));
            }
        }

        /// <summary>
        /// op_ret
        /// </summary>
        private class OpRet : OpcodeHandler_1OP
        {
            public override String Name() { return "op_ret"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.PopRoutineData(v1);
            }
        }

        /// <summary>
        /// op_jump
        /// </summary>
        private class OpJump : OpcodeHandler_1OP
        {
            public override String Name() { return "op_jump"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                int offset = (short)v1;
                machine.pc += (uint)(offset - 2);
            }
        }

        /// <summary>
        /// op_print_paddr
        /// </summary>
        private class OpPrintPaddr : OpcodeHandler_1OP
        {
            public override String Name() { return "op_print_paddr"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                consoleIO.Out(machine.memory.GetZSCII((uint)v1 * 2, 0).str);
            }
        }

        /// <summary>
        /// op_load
        /// </summary>
        private class OpLoad : OpcodeHandler_1OP
        {
            public override String Name() { return "op_load"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, ushort v1)
            {
                machine.SetVar(machine.PcGetWord(), v1);
            }
        }

        // 0OP Classes
        // Branch Opcodes 181, 182 , 189, 191
        /// <summary>
        /// op_rtrue
        /// </summary>
        private class OpRtrue : OpcodeHandler_0OP
        {
            public override String Name() { return "op_rtrue"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                machine.PopRoutineData(1);
            }
        }

        /// <summary>
        /// op_rfalse
        /// </summary>
        private class OpRfalse : OpcodeHandler_0OP
        {
            public override String Name() { return "op_rfalse"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                machine.PopRoutineData(0);
            }
        }

        /// <summary>
        /// op_print
        /// </summary>
        private class OpPrint : OpcodeHandler_0OP
        {
            public override String Name()
            {
                return "op_print";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                log.Debug("Getting string at " + machine.pc);
                Memory.StringAndReadLength str = machine.memory.GetZSCII(machine.pc, 0);
                consoleIO.Out(str.str);
                machine.pc += (uint)str.bytesRead;
                log.Debug("New pc location: " + machine.pc);

            }
        }

        /// <summary>
        /// op_print_ret
        /// </summary>
        private class OpPrintRet : OpcodeHandler_0OP
        {
            public override String Name() { return "op_print_ret"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                Memory.StringAndReadLength str = machine.memory.GetZSCII(machine.pc, 0);
                consoleIO.Out(str.str);
                machine.pc += (uint)str.bytesRead;
                machine.PopRoutineData(1);
            }
        }

        /// <summary>
        /// op_nop
        /// </summary>
        private class OpNop : OpcodeHandler_0OP
        {
            public override String Name() { return "op_nop"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                return;
            }
        }

        /// <summary>
        /// op_save
        /// </summary>
        private class OpSave : OpcodeHandler_0OP
        {
            public override String Name() { return "op_save"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_restore
        /// </summary>
        private class OpRestore : OpcodeHandler_0OP
        {
            public override String Name() { return "op_restore"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_restart
        /// </summary>
        private class OpRestart : OpcodeHandler_0OP
        {
            public override String Name() { return "op_restart"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_ret_popped
        /// </summary>
        private class OpRetPopped : OpcodeHandler_0OP
        {
            public override String Name() { return "op_ret_popped"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                machine.PopRoutineData(machine.GetVar(0));
            }
        }

        /// <summary>
        /// op_pop
        /// </summary>
        private class OpPop : OpcodeHandler_0OP
        {
            public override String Name() { return "op_pop"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                machine.GetVar(0);
            }
        }

        /// <summary>
        /// op_quit
        /// </summary>
        private class OpQuit : OpcodeHandler_0OP
        {
            public override String Name() { return "op_quit"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                machine.finish = true;
            }
        }

        /// <summary>
        /// op_new_line
        /// </summary>
        private class OpNewLine : OpcodeHandler_0OP
        {
            public override String Name() { return "op_new_line"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                consoleIO.Out("\n");
            }
        }

        /// <summary>
        /// op_show_status
        /// </summary>
        private class OpShowStatus : OpcodeHandler_0OP
        {
            public override String Name() { return "op_show_status"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_verify
        /// </summary>
        private class OpVerify : OpcodeHandler_0OP
        {
            public override String Name() { return "op_verify"; }
            public override void Run(Machine machine, IConsoleIO consoleIO)
            {
                FailUnimplemented(machine);
            }
        }


        // VAR OP Classes

        /// <summary>
        /// op_call
        /// </summary>
        private class OpCall : OpcodeHandlerOPVAR
        {
            string op = "op_call";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                log.Debug("In Run()");
                if (operands[0] == 0)
                {
                    ushort variable = machine.PcGetByte();
                    log.Info(op + " " + variable + ",0");
                    machine.SetVar(variable, 0); //set return value to zero
                }
                else
                {
                    log.Info(op);
                    machine.PushRoutineData(operands);
                }
            }
        }

        /// <summary>
        /// op_storew
        /// </summary>
        private class OpStorew : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_storew"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                machine.memory.SetWord((uint)(operands[0] + 2 * operands[1]), operands[2]);
            }
        }

        /// <summary>
        /// op_storeb
        /// </summary>
        private class OpStoreb : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_storeb"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                machine.memory.SetByte((uint)(operands[0] + operands[1]), (byte)operands[2]);
            }
        }

        /// <summary>
        /// op_put_prop
        /// </summary>
        private class OpPutProp : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_put_prop"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                machine.objectTable.SetObjectProperty(operands[0], operands[1], operands[2]);
            }
        }

        /// <summary>
        /// op_sread
        /// </summary>
        private class OpSread : OpcodeHandlerOPVAR
        {
            public override String Name()
            {
                return "op_sread";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {

                machine.lex.Read(operands[0], operands[1]);

            }
        }

        /// <summary>
        /// op_print_char
        /// </summary>
        private class OpPrintChar : OpcodeHandlerOPVAR
        {
            public override String Name()
            {
                return "op_print_char";
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                Console.Write(machine.memory.GetZChar(operands[0]));
            }
        }

        /// <summary>
        /// op_print_num
        /// </summary>
        private class OpPrintNum : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_print_num"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                consoleIO.Out(Convert.ToString(operands[0]));
            }
        }

        /// <summary>
        /// op_random
        /// </summary>
        private class OpRandom : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_random"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                int value = 0;
                if (operands[0] > 0)
                {
                    Random random = new Random();
                    value = (ushort)random.Next(1, operands[0]);
                }
                else
                {
                    Random random = new Random(operands[0]);
                    value = (ushort)random.Next(1, operands[0]);
                }
                machine.SetVar(machine.PcGetByte(), (ushort)value);  // If range is negative, the random number generator is seeded to that value and the return value is 0
            }
        }

        /// <summary>
        /// op_push
        /// </summary>
        private class OpPush : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_push"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                machine.SetVar(0, operands[0]);
            }
        }

        /// <summary>
        /// op_pull
        /// </summary>
        private class OpPull : OpcodeHandlerOPVAR
        {
            string op = "op_pull";
            public override String Name()
            {
                return (op);
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                log.Debug("In " + op + " Run()");
                log.Info(op);
                machine.SetVar(operands[0], machine.GetVar(0));
            }
        }

        /// <summary>
        /// op_split_window
        /// </summary>
        private class OpSplitWindow : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_split_window"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_set_window
        /// </summary>
        private class OpSetWindow : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_set_window"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                FailUnimplemented(machine);
            }
        }

        /// <summary>
        /// op_output_stream
        /// </summary>
        private class OpOutputStream : OpcodeHandlerOPVAR
        {
            public override String Name() { return "op_output_stream"; }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                if (operands[0] != 0)
                {
                    if (operands[0] < 0)
                    {
                        // deselect output stream
                    }
                    else if (operands[0] == 3)
                    {
                        Memory.StringAndReadLength str = machine.memory.GetZSCII((uint)operands[1] + 2, machine.memory.GetWord((uint)operands[1]));
                    }
                    else
                    {
                        // select output stream
                    }
                }
            }
        }

        /// <summary>
        /// op_input_stream
        /// </summary>
        private class OpInputStream : OpcodeHandlerOPVAR
        {
            public override String Name()
            {
                return ("op_input_stream");
            }
            public override void Run(Machine machine, IConsoleIO consoleIO, List<ushort> operands)
            {
                FailUnimplemented(machine);
            }
        }
    }
}
