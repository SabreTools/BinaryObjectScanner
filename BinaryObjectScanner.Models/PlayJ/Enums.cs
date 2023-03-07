namespace BinaryObjectScanner.Models.PlayJ
{
    /// <see href="http://www.playj.com/static/genreindex_XX.html"/>
    public enum Genre
    {
        /// <summary>
        /// Blues/Folk/Country
        /// </summary>
        BluesFolkCountry = 1,

        /// <summary>
        /// Jazz
        /// </summary>
        Jazz = 5,

        /// <summary>
        /// Reggae
        /// </summary>
        Reggae = 10,

        /// <summary>
        /// Classical
        /// </summary>
        Classical = 12,

        /// <summary>
        /// Electronic
        /// </summary>
        Electronic = 13,

        /// <summary>
        /// Pop/Rock
        /// </summary>
        PopRock = 15,

        /// <summary>
        /// World
        /// </summary>
        World = 16,

        /// <summary>
        /// Urban
        /// </summary>
        Urban = 17,

        /// <summary>
        /// Latin
        /// </summary>
        Latin = 18,

        /// <summary>
        /// Soundtrack/Other
        /// </summary>
        SoundtrackOther = 20,

        /// <summary>
        /// New Age
        /// </summary>
        NewAge = 21,

        /// <summary>
        /// Spiritual
        /// </summary>
        Spiritual = 22,

        /// <summary>
        /// Sway & Tech
        /// </summary>
        SwayAndTech = 23,

        /// <summary>
        /// Jam Bands
        /// </summary>
        JamBands = 24,

        /// <summary>
        /// Comedy
        /// </summary>
        Comedy = 25,

        /// <summary>
        /// Brazilian
        /// </summary>
        Brazilian = 26,
    }

    // TODO: Fill out the remaining subgenres from the wayback machine
    /// <see href="http://playj.com/static/subgenre_XX.html"/>
    /// <remarks>Every subgenre is uniquely associated with a genre</remarks>
    public enum Subgenre : byte
    {
        /// <summary>
        /// Blues/Folk/Country > Blues (Modern/Electric)
        /// </summary>
        BluesModernElectric = 2,

        /// <summary>
        /// Blues/Folk/Country > Blues (Modern/Acoustic)
        /// </summary>
        BluesModernAcoustic = 3,

        /// <summary>
        /// Blues/Folk/Country > Blues (Traditional)
        /// </summary>
        BluesTraditional = 4,

        /// <summary>
        /// Blues/Folk/Country > Folk (Traditional)
        /// </summary>
        FolkTraditional = 5,

        /// <summary>
        /// Blues/Folk/Country > Folk (Contemporary)
        /// </summary>
        FolkContemporary = 6,

        /// <summary>
        /// Blues/Folk/Country > Folk (Jazz)
        /// </summary>
        FolkJazz = 7,
    }
}