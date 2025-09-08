using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Models.PortableExecutable.ResourceEntries;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Spoon Installer
    /// </summary>
    public class SpoonInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // <see href="https://www.virustotal.com/gui/file/ad876d9aa59a2c51af776ce7c095af69f41f2947c6a46cfe87a724ecf8745084/details"/>
            var name = exe.AssemblyDescription;
            if (name.OptionalEquals("Spoon Installer"))
                return "Spoon Installer";

            // Get the resource data
            // TODO: This should be replaced by a helper method on the wrapper
            var resourceData = exe.ResourceData;
            if (resourceData != null)
            {
                var resourceValue = Array.Find([.. resourceData.Values], rd => rd is AssemblyManifest);
                if (resourceValue != null && resourceValue is AssemblyManifest manifest)
                {
                    var identities = manifest?.AssemblyIdentities ?? [];
                    var nameIdentity = Array.Find(identities, ai => !string.IsNullOrEmpty(ai?.Name));

                    // <see href="https://www.virustotal.com/gui/file/ad876d9aa59a2c51af776ce7c095af69f41f2947c6a46cfe87a724ecf8745084/details"/>
                    if (nameIdentity?.Name == "Illustrate.Spoon.Installer")
                        return "Spoon Installer";
                }
            }

            return null;
        }
    }
}
