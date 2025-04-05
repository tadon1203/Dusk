using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.VM;
using VRC.Udon.VM.Common;
using Logger = Dusk.Core.Logger;

namespace Dusk.Utilities;

public static class UdonDisassembler
{
    private static readonly string SaveDir = Path.Combine(Directory.GetCurrentDirectory(), "Dusk");
    
    public static void Disassemble(UdonBehaviour udonBehaviour, string ubName)
    {
        try
        {
            var udonProgram = udonBehaviour._program;
            var byteCode = udonProgram.ByteCode;
            var symbolTable = udonProgram.SymbolTable;
            var heap = udonProgram.Heap;
            var eventTable = udonBehaviour._eventTable;

            var output = new StringBuilder();
            var constants = new Dictionary<uint, string>();
        
            int address = 0;
            while (address < byteCode.Length)
            {
                try
                {
                    if (eventTable != null)
                    {
                        foreach (var entry in eventTable)
                        {
                            var addresses = entry.Value;
                            if (addresses.Count > 0 && addresses.IndexOf(0) == address)
                            {
                                output.AppendLine($".func_{entry.key}\n");
                                break;
                            }
                        }
                    }
            
                    if (address + 4 > byteCode.Length)
                    {
                        Logger.Warning($"Reached end of bytecode prematurely at 0x{address:X}.");
                        break;
                    }
                    
                    uint opcode = BitConverter.ToUInt32(byteCode, address);
                    opcode = SwapEndianness(opcode);

                    switch (opcode)
                    {
                        case (uint)OpCode.NOP:
                            Logger.Debug("Resolving NOP");
                            output.AppendLine($"0x{address:X}  NOP");
                            address += 4;
                            break;
                
                        case (uint)OpCode.PUSH:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving PUSH");
                            uint pushOffset = BitConverter.ToUInt32(byteCode, address + 4);
                            pushOffset = SwapEndianness(pushOffset);
                    
                            string symbolName = symbolTable.GetSymbolFromAddress(pushOffset) ?? $"0x{pushOffset:X}";
                            string symbolType = "Unknown";
                    
                            object heapValue = heap.GetHeapVariable(pushOffset);
                            if (heapValue != null)
                            {
                                Type runtimeType = heapValue.GetType();
                                symbolType = runtimeType.FullName;
                        
                                if (symbolName.Contains("_const_"))
                                {
                                    if (runtimeType == typeof(bool) || 
                                        runtimeType == typeof(int) ||
                                        runtimeType == typeof(float) ||
                                        runtimeType == typeof(double))
                                    {
                                        constants[pushOffset] = heapValue.ToString();
                                    }
                                    else if (runtimeType == typeof(string))
                                    {
                                        constants[pushOffset] = $"\"{heapValue}\"";
                                    }
                                }
                            }
                    
                            output.AppendLine($"0x{address:X}  PUSH 0x{pushOffset:X} ({symbolName}[{symbolType}])");
                            address += 8;
                            break;
                
                        case (uint)OpCode.POP:
                            Logger.Debug("Resolving POP");
                            output.AppendLine($"0x{address:X}  POP");
                            address += 4;
                            break;
                
                        case (uint)OpCode.JUMP_IF_FALSE:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving JNE");
                            uint jneOffset = BitConverter.ToUInt32(byteCode, address + 4);
                            jneOffset = SwapEndianness(jneOffset);
                            output.AppendLine($"0x{address:X}  JNE 0x{jneOffset:X}");
                            address += 8;
                            break;
                
                        case (uint)OpCode.JUMP:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving JMP");
                            uint jumpOffset = BitConverter.ToUInt32(byteCode, address + 4);
                            jumpOffset = SwapEndianness(jumpOffset);
                            output.AppendLine($"0x{address:X4}  JMP 0x{jumpOffset:X}");
                            address += 8;
                            break;
                
                        case (uint)OpCode.EXTERN:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving EXTERN");
                            uint externAddr = BitConverter.ToUInt32(byteCode, address + 4);
                            externAddr = SwapEndianness(externAddr);
                    
                            string externName = $"unknown_extern@{externAddr:X}";
                            object externObj = heap.GetHeapVariable(externAddr);

                            if (externObj != null)
                            {
                                Type externType = externObj.GetType();
                                if (externType == typeof(UdonVM.CachedUdonExternDelegate))
                                {
                                    externName = ((UdonVM.CachedUdonExternDelegate)externObj).externSignature ?? externName;
                                }
                                else
                                {
                                    externName = externObj.ToString();
                                }
                            }
                    
                            output.AppendLine($"0x{address:X}  EXTERN \"{externName}\"");
                            address += 8;
                            break;
                
                        case (uint)OpCode.ANNOTATION:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving ANNOTATION");
                            uint annotationAddr = BitConverter.ToUInt32(byteCode, address + 4);
                            annotationAddr = SwapEndianness(annotationAddr);
                    
                            object annotationObj = heap.GetHeapVariable(annotationAddr);
                            string annotationText = annotationObj?.ToString() ?? $"unknown_annotation@{annotationAddr:X}";

                            output.AppendLine($"0x{address:X}  ANNOTATION \"{annotationText}\"");
                            address += 8;
                            break;
                
                        case (uint)OpCode.JUMP_INDIRECT:
                            if (address + 8 > byteCode.Length) break;
                    
                            Logger.Debug("Resolving JMP_INDIRECT");
                            uint indirectAddr = BitConverter.ToUInt32(byteCode, address + 4);
                            indirectAddr = SwapEndianness(indirectAddr);

                            string targetSymbol = symbolTable.HasSymbolForAddress(indirectAddr) ? symbolTable.GetSymbolFromAddress(indirectAddr) : $"0x{indirectAddr:X}";
                    
                            output.AppendLine($"0x{address:X}  JMP [{targetSymbol}]");
                            address += 8;
                            break;
                
                        case (uint)OpCode.COPY:
                            Logger.Debug("Resolving COPY");
                            output.AppendLine($"0x{address:X}  COPY");
                            address += 4;
                            break;
                
                        default:
                            Logger.Warning($"Unknown opcode: {opcode} at address 0x{address:X}");
                            address += 4;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to process bytecode at 0x{address:X}: {ex.Message}");
                    throw;
                }
            }
        
            output.AppendLine(".end");
        
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
        
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        
            Logger.Info("Saving to file...");
            File.WriteAllText($"{SaveDir}/Disassembled_{ubName}_{timestamp}.txt", output.ToString());

            if (constants.Count > 0)
            {
                var constantsOutput = new StringBuilder();
                foreach (var kv in constants)
                {
                    constantsOutput.AppendLine($"0x{kv.Key:X}: {kv.Value}");
                }
                File.WriteAllText($"{SaveDir}/Constants_{ubName}_{timestamp}.txt", constantsOutput.ToString());
            }
        }
        catch (Exception ex)
        {
            Logger.Critical($"Critical error in UdonDisassembler: {ex}");
            throw;
        }
    }

    private static uint SwapEndianness(uint value)
    {
        return ((value & 0x000000FF) << 24) |
               ((value & 0x0000FF00) << 8) |
               ((value & 0x00FF0000) >> 8) |
               ((value & 0xFF000000) >> 24);
    }
}