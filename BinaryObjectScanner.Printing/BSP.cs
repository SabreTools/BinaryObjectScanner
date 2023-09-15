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

            builder.AppendLine(header.Version, "  Version");
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

                builder.AppendLine(lump.Offset, "    Offset");
                builder.AppendLine(lump.Length, "    Length");
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

            builder.AppendLine(header.TextureCount, "  Texture count");
            builder.AppendLine("  Offsets:");
            if (header.Offsets == null || header.Offsets.Length == 0)
            {
                builder.AppendLine("  No offsets");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < header.Offsets.Length; i++)
            {
                builder.AppendLine(header.Offsets[i], $"    Offset {i}");
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

                builder.AppendLine(texture.Name, "    Name");
                builder.AppendLine(texture.Width, "    Width");
                builder.AppendLine(texture.Height, "    Height");
                builder.AppendLine("    Offsets:");
                if (texture.Offsets == null || texture.Offsets.Length == 0)
                {
                    builder.AppendLine("    No offsets");
                    continue;
                }
                else
                {
                    for (int j = 0; j < texture.Offsets.Length; j++)
                    {
                        builder.AppendLine(texture.Offsets[i], $"      Offset {j}");
                    }
                }
                // Skip texture data
                builder.AppendLine(texture.PaletteSize, "    Palette size");
                // Skip palette data
            }
            builder.AppendLine();
        }

    }
}