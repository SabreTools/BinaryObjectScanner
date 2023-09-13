using System.Text;
using SabreTools.Models.BSP;
using static SabreTools.Models.BSP.Constants;

namespace BinaryObjectScanner.Printing
{
    public static class BSP
    {
        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("BSP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.Lumps);
            Print(builder, file.TextureHeader);
            Print(builder, file.Textures);
        }

#if NET48
        private static void Print(StringBuilder builder, Header header)
#else
        private static void Print(StringBuilder builder, Header? header)
#endif
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Lump[] lumps)
#else
        private static void Print(StringBuilder builder, Lump?[]? lumps)
#endif
        {
            builder.AppendLine("  Lumps Information:");
            builder.AppendLine("  -------------------------");
            if (lumps == null || lumps.Length == 0)
            {
                builder.AppendLine("  No lumps");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < lumps.Length; i++)
            {
                var lump = lumps[i];
                string specialLumpName = string.Empty;
                switch (i)
                {
                    case HL_BSP_LUMP_ENTITIES:
                        specialLumpName = " (entities)";
                        break;
                    case HL_BSP_LUMP_TEXTUREDATA:
                        specialLumpName = " (texture data)";
                        break;
                }

                builder.AppendLine($"  Lump {i}{specialLumpName}");
                if (lump == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Offset: {lump.Offset} (0x{lump.Offset:X})");
                builder.AppendLine($"    Length: {lump.Length} (0x{lump.Length:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, TextureHeader header)
#else
        private static void Print(StringBuilder builder, TextureHeader? header)
#endif
        {
            builder.AppendLine("  Texture Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No texture header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Texture count: {header.TextureCount}");
            builder.AppendLine($"  Offsets:");
            if (header.Offsets == null || header.Offsets.Length == 0)
            {
                builder.AppendLine("  No offsets");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < header.Offsets.Length; i++)
            {
                builder.AppendLine($"    Offset {i}: {header.Offsets[i]} (0x{header.Offsets[i]:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Texture[] textures)
#else
        private static void Print(StringBuilder builder, Texture?[]? textures)
#endif
        {
            builder.AppendLine("  Textures Information:");
            builder.AppendLine("  -------------------------");
            if (textures == null || textures.Length == 0)
            {
                builder.AppendLine("  No textures");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                builder.AppendLine($"  Texture {i}");
                if (texture == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Name: {texture.Name}");
                builder.AppendLine($"    Width: {texture.Width} (0x{texture.Width:X})");
                builder.AppendLine($"    Height: {texture.Height} (0x{texture.Height:X})");
                builder.AppendLine($"    Offsets:");
                if (texture.Offsets == null || texture.Offsets.Length == 0)
                {
                    builder.AppendLine($"    No offsets");
                    continue;
                }
                else
                {
                    for (int j = 0; j < texture.Offsets.Length; j++)
                    {
                        builder.AppendLine($"      Offset {j}: {texture.Offsets[i]} (0x{texture.Offsets[j]:X})");
                    }
                }
                // Skip texture data
                builder.AppendLine($"    Palette size: {texture.PaletteSize} (0x{texture.PaletteSize:X})");
                // Skip palette data
            }
            builder.AppendLine();
        }

    }
}