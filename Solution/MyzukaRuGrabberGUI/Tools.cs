using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using KlotosLib;

namespace MyzukaRuGrabberGUI
{
    internal class Tools
    {
        #region Gets the build date and time (by reading the COFF header)
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms680313
        /// </summary>
        private struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };

        private static DateTime GetBuildDateTime(Assembly assembly)
        {
            if(assembly==null) {throw new ArgumentNullException("assembly");}
            if (File.Exists(assembly.Location)==false) {throw new FileNotFoundException("Файл сборки '"+assembly.FullName+"' не найден", assembly.Location);}
            Byte[] buffer = new Byte[Math.Max(Marshal.SizeOf(typeof(Tools._IMAGE_FILE_HEADER)), 4)];
            using (FileStream fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
            {
                fileStream.Position = 0x3C;
                fileStream.Read(buffer, 0, 4);
                fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                fileStream.Read(buffer, 0, 4); // "PE\0\0"
                fileStream.Read(buffer, 0, buffer.Length);
            }
            GCHandle pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Tools._IMAGE_FILE_HEADER coffHeader = (Tools._IMAGE_FILE_HEADER)Marshal.PtrToStructure
                    (pinnedBuffer.AddrOfPinnedObject(), typeof(Tools._IMAGE_FILE_HEADER));

                return TimeZone.CurrentTimeZone.ToLocalTime
                    (new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond));
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }
        #endregion

        internal static IEnumerable<String> GetAssemblyDescription(Assembly[] Assemblies)
        {
            Assemblies.ThrowIfNullOrEmpty();
            IEnumerable<String> output = from Assembly ass in Assemblies
                let ver = ass.GetName().Version
                let name = ass.GetName().Name
                let b2 = Tools.GetBuildDateTime(ass).ToString("yyyy-MM-dd HH.mm.ss") + " EET"
                let gac = (ass.GlobalAssemblyCache == true ? " from GAC, " : " from file, ")
                where KlotosLib.StringTools.StringExtensionMethods.IsIn(name, StringComparison.Ordinal, "HtmlAgilityPack", "KlotosLib") ||
                    name.StartsWith("MyzukaRuGrabber", StringComparison.OrdinalIgnoreCase)
                orderby name ascending
                select ass.GetName().Name + gac + "Compiled with .NET " + ass.ImageRuntimeVersion + "\r\nVersion: " + ver.ToString() + ". Build date: " + b2;
            return output;
        }
    }
}
