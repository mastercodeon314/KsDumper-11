using System;
using System.IO;
using System.Text;

namespace KsDumper11
{
    public class ModuleSummary
    {
        public ulong ModuleBase { get; set; }

        public string ModuleFileName { get; set; }

        public string ModuleName { get; set; }

        public uint ModuleImageSize { get; set; }

        public ulong ModuleEntryPoint { get; set; }

        private ModuleSummary(ulong moduleBase, string moduleFileName, uint moduleImageSize, ulong moduleEntryPoint)
        {
            this.ModuleBase = moduleBase;
            this.ModuleFileName = moduleFileName;
            this.ModuleImageSize = moduleImageSize;
            this.ModuleEntryPoint = moduleEntryPoint;
            this.ModuleName = Path.GetFileName(moduleFileName);
        }

        public static ModuleSummary FromStream(BinaryReader reader)
        {
            ulong moduleBase = reader.ReadUInt64();
            string moduleFileName = Encoding.Unicode.GetString(reader.ReadBytes(512)).Split(new char[1])[0];
            uint moduleImageSize = reader.ReadUInt32();
            ulong moduleEntryPoint = reader.ReadUInt64();
            return new ModuleSummary(moduleBase, moduleFileName, moduleImageSize, moduleEntryPoint);
        }
    }
}
