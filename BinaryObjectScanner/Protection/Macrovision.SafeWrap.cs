using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// SafeWrap (https://web.archive.org/web/20010803054324/http://www.macrovision.com:80/solutions/software/safewrap.php3) is a wrapper created by Macrovision used to protect software.
    /// It's used (to an unknown extent) within SafeCast and SafeDisc (https://web.archive.org/web/20030829044647/http://www.macrovision.com:80/solutions/software/SafeWrap_FAQ_July2003.pdf Section #3).
    /// It can be used separately from any other Macrovision product, such as (supposedly) for RioPort (https://www.cdrinfo.com/d7/content/macrovision-invests-next-generation-security-and-transport-technologies-internet-delivery).
    /// No direct SafeWrap only sample has currently been found, nor is it exactly clear what parts of SafeCast/SafeDisc are SafeWrap.
    /// It's claimed that SafeWrap uses one DLL and one .SYS "security driver" (https://web.archive.org/web/20030829044647/http://www.macrovision.com:80/solutions/software/SafeWrap_FAQ_July2003.pdf Section #30).
    /// This would appear to be referring to the "drvmgt.dll" and "secdrv.sys" files, the latter of which is officially referred to as the "Macrovision SECURITY Driver" in some SafeDisc versions.
    /// This may not be fully accurate, as not all of these files are known to always be used with SafeCast, though this does need further investigation.
    /// The .stxt* sections found in various Macrovision products may indicate SafeWrap, as they started being used around the time that SafeWrap was made public.
    /// It may also be that the "BoG" string known to be present in Macrovision executables could be related to SafeWrap, though its use predates the first known mention of SafeWrap by a few years.
    /// SafeWrap may have its origins in SafeDisc-related tech if that's the case, further confusing matters.
    /// This unforuantely has several possible unpleasant implications for current checks, such as if the X.YY.ZZZ version string is a generic property of SafeWrap and can be non-standard when used for different products.
    /// Currently, no known patents or trademarks for SafeWrap have been found.
    ///
    /// Further information and resources:
    /// Macrovision press release that mentions SafeWrap: https://www.sec.gov/Archives/edgar/data/1027443/000100547701501658/ex99-1.txt
    /// Macrionvision "Tamper-proof" blurb advertising SafeWrap: https://web.archive.org/web/20030412093353/http://www.macrovision.com/solutions/software/tamper.php3
    /// SafeAudio news that mentions SafeWrap: https://www.cdmediaworld.com/hardware/cdrom/news/0201/safeaudio_3.shtml
    /// The titles for a few audio DRM FAQ's include the text "SafeWrap™ Frequently Asked Questions" in the page title, but this may be a copy-paste error:
    /// https://web.archive.org/web/20030324080804/http://www.macrovision.com:80/solutions/audio/images/SafeAudio_FAQ_Public_5-02.pdf + https://web.archive.org/web/20030403050432/http://www.macrovision.com:80/solutions/audio/Audio_protection_FAQ_Public_March2003.pdf
    /// Eclipse SafeAudio news that mentions SafeWrap: http://www.eclipsedata.com/PDFs/21.pdf
    /// Report to congress that mentions SafeWrap: https://www.uspto.gov/sites/default/files/web/offices/dcom/olia/teachreport.pdf
    /// Patent that mentions possibly using SafeWrap: https://patents.google.com/patent/US7493289B2/en
    /// MacroSafe presentation that mentions current customer of SafeWrap/SafeCast/SafeDisc: https://slideplayer.com/slide/6840826/
    /// List of DRM companies and products: https://www1.villanova.edu/villanova/generalcounsel/copyright/digitized/companies.html
    /// Forum post that briefly mentions SafeWrap: https://www.ttlg.com/forums/showthread.php?t=70035
    /// Forum post (with no replies) that asks for information about SafeWrap: https://forum.powerbasic.com/forum/user-to-user-discussions/powerbasic-for-windows/9167-macrovision-anti-code-tampering-tool
    /// Document that mentions SafeWrap as third-party security tool: https://www.cs.clemson.edu/course/cpsc420/material/Papers/Guide_AppSecurity.pdf
    /// Japanese PDF that mentions SafeWrap: https://ipsj.ixsq.nii.ac.jp/ej/index.php?action=pages_view_main&active_action=repository_action_common_download&item_id=64743&item_no=1&attribute_id=1&file_no=1&page_id=13&block_id=8
    /// Korean PDF that mentions SafeWrap: http://contents.kocw.or.kr/contents4/document/lec/2012/KonKuk_glocal/Nohyounghee1/9.pdf
    /// Patents that mention the possible use of SafeWrap:
    /// https://patentscope.wipo.int/search/en/detail.jsf?docId=US42774631&_cid=P20-LGE2MB-82932-1
    /// https://patentscope.wipo.int/search/en/detail.jsf?docId=US40713491&_cid=P20-LGE2MB-82932-1
    /// </summary>
    public partial class Macrovision
    {
        /// <inheritdoc cref="Interfaces.IExecutableCheck{T}.CheckExecutable(string, T, bool)"/>
        internal string? SafeWrapCheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Figure out what SafeWrap is exactly, and add checks.

            return null;
        }
    }
}
