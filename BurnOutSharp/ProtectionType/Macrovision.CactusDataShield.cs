using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// CactusDataShield was a copy protection originally developed by Midbar Technologies, which was then purchased by Macrovision in 2002 (https://variety.com/2002/digital/news/macrovision-acquires-midbar-cuts-ttr-link-1117875824/).
    /// Macrovision's product page for CDS: https://web.archive.org/web/20050215235405/http://www.macrovision.com/products/cds/index.shtml
    /// CDS-100 appears to function by attempting to prevent dumping/ripping the discs protected with it.
    /// CDS-200+ uses a dedicated audio player to play the music "legitimately".
    /// Patent relating to CDS-100: https://patents.google.com/patent/US6425098B1/
    /// Known CDS versions:
    /// CDS-100 ("The Loveparade Compilation 2001" by various artists (Barcode 74321 86986 2) (Likely Discogs Release Code [r155963]) and "World Of Our Own" (Limited Edition) by Westlife (Barcode 7 43218 98572 0) (Discogs Release Code [r1357706])).
    /// CDS-200 (PlayJ) (("Volumia!" by Puur (Barcode 7 43218 63282 2) (Discogs Release Code [r795427])) (Confirmed to be CDS-200 from https://www.cdrinfo.com/d7/content/cactus-data-shield-200?page=2).
    /// CDS200.0.4 - 3.0 build 16a (Redump entry 95036)
    /// CDS200.0.4 - 3.0 build 16c ("TMF Hitzone 20" by various artists (Barcode 7 31458 37062 8)).
    /// CDS200.0.4 - 4.1 build 2a ("Ich Habe Einen Traum" by Uwe Busse (Barcode 9 002723 251203)).
    /// CDS200.0.4 - 4.1 build 2e ("Hallucinations" by David Usher (Barcode 7 24359 30322 2)).
    /// CDS200.5.11.90 - 5.10.090 ("Finn 5 Fel!" by Gyllene Tider (Barcode 7 24357 10922 2)).
    /// CDS-300
    /// Further information:
    /// https://www.cdrinfo.com/d7/content/cactus-data-shield-200
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_cactus_data_shield.shtml
    /// </summary>
    public partial class Macrovision
    {
        // TODO: Port existing CactusDataShield checks here.
    }
}
