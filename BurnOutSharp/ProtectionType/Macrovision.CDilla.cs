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
    /// C-Dilla (https://web.archive.org/web/19980204101314/http://www.c-dilla.com/).
    /// As of May 2000, Ç-Dilla Ltd was renamed to Ç-Dilla Labs (https://web.archive.org/web/20000519231449/http://www.c-dilla.com:80/). As of June 2001, it was renamed to Macrovision Europe (https://web.archive.org/web/20010628061027/http://www.c-dilla.com:80/).
    /// AutoCAD appears to have been using C-Dilla products since 1996 (https://web.archive.org/web/19980204102208/http://www.c-dilla.com/press/201296.html).
    /// https://knowledge.autodesk.com/support/3ds-max/troubleshooting/caas/sfdcarticles/sfdcarticles/Description-of-the-C-Dilla-License-Management-System.html
    /// https://web.archive.org/web/20040223025801/www.macrovision.com/products/legacy_products/safecast/safecast_cdilla_faq.shtml
    /// https://forums.anandtech.com/threads/found-c-dilla-on-my-computer-get-it-off-get-it-off-heeeelp.857554/
    /// https://www.extremetech.com/extreme/53108-macrovision-offers-closer-look-at-safecastcdilla
    /// https://www.cadlinecommunity.co.uk/hc/en-us/articles/201873101-C-dilla-Failing-to-Install-or-Stops-Working-
    /// https://archive.org/details/acadlt2002cd
    /// https://archive.org/details/telepower
    /// 
    /// It seems that C-Dilla License Management System is a newer name for their CD-Secure product, based on this URL (https://web.archive.org/web/20050211004709/http://www.macrovision.com/products/cdsecure/downloads.shtml) leading to a download of LMS.
    /// Known versions:
    /// 1.31.34 (https://archive.org/details/PCDDec1995).
    /// 3.23.000 (https://archive.org/details/3ds-max-4.2original).
    /// 3.24.010 (https://archive.org/details/ejay_nestle_trial).
    /// 3.27.000 (https://download.autodesk.com/mne/web/support/3dstudio/C-Dilla3.27.zip).
    /// 
    /// TODO:
    /// Investigate C-Dilla CD-Compress.
    /// Find older (pre version 3?) versions of CD-Secure. First known reference: https://web.archive.org/web/19980204101657/http://www.c-dilla.com/press/index94.html
    /// </summary>
    public partial class Macrovision
    {
        // TODO: Add C-Dilla checks.
    }
}
