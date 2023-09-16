using System.Text;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Wrappers
{
    /// <summary>
    /// Extensions to allow for pretty printing
    /// </summary>
    public static class PrintExtensions
    {
        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        public static StringBuilder PrettyPrint(this IWrapper wrapper)
        {
            switch (wrapper)
            {
                case AACSMediaKeyBlock item: return item.PrettyPrint();
                case BDPlusSVM item: return item.PrettyPrint();
                case BFPK item: return item.PrettyPrint();
                case BSP item: return item.PrettyPrint();
                case CFB item: return item.PrettyPrint();
                case CIA item: return item.PrettyPrint();
                case GCF item: return item.PrettyPrint();
                case InstallShieldCabinet item: return item.PrettyPrint();
                case LinearExecutable item: return item.PrettyPrint();
                case MicrosoftCabinet item: return item.PrettyPrint();
                case MSDOS item: return item.PrettyPrint();
                case N3DS item: return item.PrettyPrint();
                case NCF item: return item.PrettyPrint();
                case NewExecutable item: return item.PrettyPrint();
                case Nitro item: return item.PrettyPrint();
                case PAK item: return item.PrettyPrint();
                case PFF item: return item.PrettyPrint();
                case PlayJAudioFile item: return item.PrettyPrint();
                case PortableExecutable item: return item.PrettyPrint();
                case Quantum item: return item.PrettyPrint();
                case SGA item: return item.PrettyPrint();
                case VBSP item: return item.PrettyPrint();
                case VPK item: return item.PrettyPrint();
                case WAD item: return item.PrettyPrint();
                case XZP item: return item.PrettyPrint();
                default: return new StringBuilder();
            }
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this AACSMediaKeyBlock item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.AACSMediaKeyBlock.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BDPlusSVM item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BDPlusSVM.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BFPK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BFPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this BSP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.BSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CFB item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.CFB.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this CIA item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.CIA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this GCF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.GCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this InstallShieldCabinet item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.InstallShieldCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this LinearExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.LinearExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MicrosoftCabinet item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.MicrosoftCabinet.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this MSDOS item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.MSDOS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this N3DS item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.N3DS.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NCF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.NCF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this NewExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.NewExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Nitro item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.Nitro.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PAK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PAK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PFF item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PFF.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PlayJAudioFile item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PlayJAudioFile.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this PortableExecutable item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.PortableExecutable.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this Quantum item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.Quantum.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this SGA item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.SGA.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VBSP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.VBSP.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this VPK item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.VPK.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this WAD item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.WAD.Print(builder, item.Model);
            return builder;
        }

        /// <summary>
        /// Export the item information as pretty-printed text
        /// </summary>
        private static StringBuilder PrettyPrint(this XZP item)
        {
            StringBuilder builder = new StringBuilder();
            SabreTools.Printing.XZP.Print(builder, item.Model);
            return builder;
        }
    }
}