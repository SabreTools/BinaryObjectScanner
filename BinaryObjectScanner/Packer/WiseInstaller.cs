using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class WiseInstaller : IExecutableCheck<NewExecutable>, IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // If the overlay header can be found
            if (nex.FindWiseOverlayHeader() > -1)
                return "Wise Installation Wizard Module";

            return null;
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // If the overlay header can be found
            if (pex.FindWiseOverlayHeader() > -1)
                return "Wise Installation Wizard Module";

            // If the section header can be found
            if (pex.FindWiseSection() != null)
                return "Wise Installation Wizard Module";

            return null;
        }
    }
}
