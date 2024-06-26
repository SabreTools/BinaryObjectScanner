namespace BinaryObjectScanner.Protection
{
    public class phenoProtect
    {
        // Currently implemented as a text file check, more checks are likely possible but currently unknown.
        // Current checks based off Redump entry 84082 are found in the InstallShield setup.inx file, but the game also checks if the original disc is present in the drive after installation as well, so it seems unlikely for the InstallShield check to be relevant at that stage.
        // A later version of it can be found in Redump entry 102493, which is found in the InstallShield setup.ins file, and also has a disc check when the game is run. On top of this, there is also a serial number check present. It is currently unknown how to uniquely detect either of them.
        // The disc checks may be completely generic and undetectable, as these checks seem to be more lax than the installer checks. 
        // <see href="https://github.com/TheRogueArchivist/DRML/blob/main/entries/phenoProtect/phenoProtect.md"/>
    }
}
