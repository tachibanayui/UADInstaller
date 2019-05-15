namespace UADInstaller.Jsons
{
    public class UADSettingsData
    {
        public string SaveLocation { get; set; }
        public string AnimeLibraryLocation { get; set; }
        public string ScreenShotLocation { get; set; }
        public bool IsDarkTheme { get; set; }
        public object PrimaryColorTheme { get; set; }
        public object AccentColorTheme { get; set; }
        public bool DisableAnimation { get; set; }
        public int AnimationFrameRate { get; set; }
        public bool UseVirtalizingWrapPanel { get; set; }
        public int PreferedPlayer { get; set; }
        public bool PlayMediaFullScreen { get; set; }
        public float PlaybackVolume { get; set; }
        public bool IsDrawingEnabled { get; set; }
        public bool IsSneakyWatcherEnabled { get; set; }
        public bool IsSneakyWatcherBorderEnabled { get; set; }
        public string PrimaryPenColor { get; set; }
        public float PrimaryBurshThickness { get; set; }
        public string SecondaryPenColor { get; set; }
        public float SecondaryBurshThickness { get; set; }
        public string HighlighterPenColor { get; set; }
        public float HighlighterBurshThickness { get; set; }
        public string BlockerToggleHotKeys { get; set; }
        public string AppCrashToggleHotKeys { get; set; }
        public string BgPlayerToggleHotKeys { get; set; }
        public bool IsPauseWhenSneakyWactherActive { get; set; }
        public string BlockerColor { get; set; }
        public bool IsBlockerImageEnabled { get; set; }
        public string BlockerImageLocation { get; set; }
        public int BlockerStretchMode { get; set; }
        public bool MakeWindowTopMost { get; set; }
        public bool DisableAltF4 { get; set; }
        public bool IsEnableMasterPassword { get; set; }
        public string SneakyWatcherMasterPassword { get; set; }
        public bool IsRandomizePasswordBox { get; set; }
        public bool ChangeAppIconWhenSneakyWatcherActive { get; set; }
        public bool IsOnlyLoadWhenHostShow { get; set; }
        public bool IsLoadPageInBackground { get; set; }
        public object Notification { get; set; }
        public object Download { get; set; }
        public object UserInterest { get; set; }
    }
}
