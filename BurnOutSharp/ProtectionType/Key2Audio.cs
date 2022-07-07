namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// Key2Audio is an audio CD copy protection created by Sony. The initial version, simply called key2audio, appears to simply attempt to make the disc unplayable on a computer. 
    /// Further investigation is needed to determine if this first version is able to be detected, or if there's no identifying data present on these.
    /// The other major version, key2AudioXS, appears to be a standard audio CD protection that uses, at the very least, WMDS DRM, and quite possibly OpenMG as well.
    /// Key2AudioXS appears to have three sessions total, and some reports online indicate it may have a partially invalid TOC.
    /// References:
    /// https://web.archive.org/web/20070507073131/http://www.cdfreaks.com/reviews/Key2Audio-explained-and-should-we-fear-it-
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_key2audio.shtml
    /// </summary>
    public class Key2Audio
    {
        // TODO: Implement
    }
}
