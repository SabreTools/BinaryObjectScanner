using System.Collections.Generic;

namespace BinaryObjectScanner.Protection
{
    public partial class SecuROM
    {
        /// <summary>
        /// Matches hash of the Release Control-encrypted executable to known hashes
        /// </summary>
        /// <remarks>Allegedly, some version of Runaway: A Twist of Fate has RC</remarks>
        private static readonly Dictionary<string, string> MatroschkaHashDictionary = new()
        {
            {"C6DFF6B08EE126893840E107FD4EC9F6", "Alice - Madness Returns (USA)+(Europe)"},
            {"890C4DBDB7819D1FC73FE2105CED79FA", "Alice - Madness Returns (Steam v0)"},
            {"5D785311E052C6B9F2C4B1A0022BBEF7", "Alice - Madness Returns (Unknown Variant)"}, // Not sure where the RC executable came from
            {"D7703D32B72185358D58448B235BD55E", "Arcania - Gothic 4 (Australia)"}, // International version (English, French, Italian, German, Spanish)
            {"D6D6F97F99EC02A9DB4FCDE841C94726", "Arcania - Gothic 4 (Poland)"}, // Possibly Poland, Hungary?
            {"83CD6225899C08422F860095962287A5", "Arcania - Gothic 4 (Russia)"},
            // Arcania - Gothic 4 - Chinese - known to most likely exist. Likely matches support site exe.
            {"FAF6DD75DDB335101CB77A714793DC28", "Batman - Arkham City - Game of the Year Edition (UK)"},
            {"77999579EE4378BDFAC9438CC9CDB44E", "Batman - Arkham City (USA)+(Europe)"},
            {"73114CF3DEEDD0FA2BF52ACB70B048BC", "Battlefield - Bad Company 2 (GFWM)"},
            {"56C23D930F885BA5BF026FEABFC31856", "Battlefield 3 (USA)+(Europe, Asia)"},
            {"631C0ACE596722488E3393BD1AFCE731", "Battlefield 3 (Russia)"},
            {"6E481CDEBDB30B8889340CEC3300C931", "Battlefield 3 (UK)"},
            {"3963DD473C6659DB9D8F4452E6C37554", "Battlefield 3 (Unknown Variant)"},  // Not sure where the RC executable came from
            {"C5AB3931A3CBB0141CC5A4638C391F4F", "BioShock 2 (Argentina)+(Europe, Australia)+(Europe)+(Europe) (Alt)+(Netherlands)+(USA) - Multiplayer executable"},
            {"73DB35419A651CB69E78A641BBC88A4C", "BioShock 2 (Argentina)+(Europe, Australia)+(Europe)+(Europe) (Alt)+(Netherlands)+(USA) - Singleplayer executable"},
            {"E5D63D369023A1D1074E7B13952FA0F2", "BioShock 2 (Russia) - Multiplayer executable"},
            {"C39F3BCB74EA8E1215D39AC308F64229", "BioShock 2 (Russia) - Singleplayer executable"},
            {"3C340B2D4DA25039C136FEE1DC2DDE17", "Borderlands (USA)+(Europe) (En,Fr,De,Es,It)"},
            {"D35122E0E3F7B35C98BEFD706C260F83", "Crysis Warhead (Europe)+(Russia)+(USA)+(USA) (Alt)"},
            {"D9254D3353AB229806A806FCFCEABDBD", "Crysis Warhead (Japan)"},
            {"D69798C9198A6DB6A265833B350AC544", "Crysis Warhead (Turkey)"},
            {"9F574D56F1A4D7847C6A258DC2AF61A5", "Crysis Wars (Europe)+(Japan)+(Russia)+(Turkey)+(USA)+(USA) (Rerelease)"},
            {"C200ABC342A56829A5356AA0BEA5F2DF", "Dead Space 2 (Europe)+(Russia)+(USA)"},
            {"81B3415AF21C8691A1CD55A422BA64D5", "Disney TRON - Evolution (Europe) (En,Fr,De,Es,It,Nl)"},
            {"DF9609EDE95A1F89F7A39A08778CC3B8", "Disney Tron - Evolution (Europe) (Pl,Cs)"},
            {"B8698C7C05D7F9E049DC038B9868FCF7", "Disney TRON - Evolution (Russia) (En,Ru)"},
            {"0D5800F94643633CD3F025CFFD968DF2", "Dragon Age II (Europe)+(USA) - PC executable"},
            {"3F1AFA4783F9001AACF0379A2A432A13", "Dragon Age II (Europe)+(USA) - Mac executable"},
            {"530A3EB454570EEE5519ABE6BAE0187C", "Far Cry 2 (Europe)+(USA) (En,Fr,De,Es,It)"},
            {"4B3B130A70F3711BFA8AF06195FE4250", "FIFA 12 (Europe)"},
            {"D079D0302824335AF1D1AB0465267948", "FIFA 12 (Unknown Variant)"}, // Not sure where the RC executable came from
            {"F43F777696B0FAD3A331298C48104B31", "FIFA 13 (Europe)"},
            {"1DF0E096068839C12E4B353AC50E41FA", "Grand Theft Auto - Episodes from Liberty City (Russia)"},
            {"F3ADC6D08BEC42FB988F2F62B5C731FA", "Grand Theft Auto - Episodes from Liberty City (USA)"},
            {"5B90D42A650A8F08095984AEE3D961B9", "Grand Theft Auto IV (Europe, Asia)+(Europe)+(Latin America)+(USA) (Rev 1)"},
            {"4510F0BDD58D30D072952E225E294F9B", "Grand Theft Auto IV (USA)"},
            {"2AC9616A7FE46D142F653D798EAA07FD", "Harry Potter and the Deathly Hallows Part 2 (GFWM)"},
            {"AE144755FB12062780E4E4CCD29B5296", "Kingdoms of Amalur - Reckoning (Germany)"},
            {"6E4AB6416D91F85954150BC50D02688E", "Kingdoms of Amalur - Reckoning (USA) (En,Fr,Es,It,Nl)"},
            {"935103B1600F1C743AF892A0DD761913", "Mass Effect 2 (GFWM)"},
            {"EEB2AE163AEEF6BE54C5A9BDD38C600E", "Mass Effect 3 (Europe, Australia)+(USA)"},
            {"2D08B73217B722A4F9E01523F07E118E", "Mass Effect 3 (UK)"},
            {"4EA3CE0670DECD0A74FA312714C22025", "Need for Speed - The Run (Europe)"},
            {"88AB0D4A4EE7867F740AD063400FCDB5", "Need for Speed - The Run (Russia)"},
            {"EAD8E224D0F44706BA92BD9B27FEBA7D", "Need for Speed - The Run (USA)"},
            {"90919AAA29AC678D49FB2BEDC6B795EF", "Need for Speed - The Run (Unknown Alt)"}, // Not sure where the RC executable came from
            {"316FF217BD129F9EEBD05A321A8FBE60", "Syndicate (USA)+(Europe) (En,Fr,De,Es,It,Ru)"},
        };

        /// <summary>
        /// If hash isn't currently known, check size and pathname of the encrypted executable
        /// to determine if alt or entirely missing
        /// </summary>
        private static readonly Dictionary<uint, string> MatroschkaSizeFilenameDictionary = new()
        {
            {4646091, "hp8.aec"},
            {5124592, "output\\LaunchGTAIV.aec"},
            {5445032, "output\\Crysis.aec"},
            {5531004, "output\\FarCry2.aec"},
            {6716108, "LaunchEFLC.aec"},
            {6728396, "./Bioshock2Launcher.aec"},
            {6732492, "./BioShock2Launcher.aec"},
            {7150283, "GridGameLauncher.aec"},
            {7154379, "GridGameLauncher.aec"},
            {8705763, "temp0.aec"},
            {12137051, "dragonage2.aec"},
            {12896904, "output\\crysis.aec"},
            {12917384, "output\\crysis.aec"},
            {12925576, "output\\crysis.aec"},
            {16415836, "output\\MassEffect2.aec"},
            {17199339, "AliceMadnessReturns.aec"},
            {22357747, "MassEffect3.aec"},
            {23069931, "fifa.aec"},
            {25409907, "Arcania.aec"},
            {25410419, "Arcania.aec"},
            {25823091, "Arcania.aec"},
            {27564780, "output\\BFBC2Game.aec"},
            {30470419, "temp0.aec"},
            {32920811, "temp0.aec"},
            {35317996, "output\\ShippingPC-WillowGame-SecuROM.aec"},
            {35610875, "temp0.aec"},
            {37988075, "temp0.aec"},
            {43612419, "BatmanAC.aec"},
            {45211355, "BatmanAC.aec"},
            {48093043, "deadspace_f.aec"},
        };
    }
}
