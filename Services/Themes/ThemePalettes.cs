using Gremlins.Services;

namespace Gremlins.Services.Themes;

/// <summary>Maps every <see cref="AppThemePreference"/> (except <see cref="AppThemePreference.System"/>) to colors.</summary>
public static class ThemePalettes
{
    /// <summary>Original Gremlins dark palette.</summary>
    public static readonly ThemeColors Dark = new(
        BgDeep: "#0D0D0F",
        BgCard: "#16161A",
        CardHover: "#1E1E24",
        AccentGreen: "#39FF14",
        AccentGreenDim: "#1A7A00",
        AccentRed: "#FF3A3A",
        TextPrimary: "#E8E8E8",
        TextMuted: "#6B6B7A",
        Border: "#2A2A32",
        WindowBg: "#0D0D0F",
        BorderOuter: "#2A2A32",
        TitleBar: "#111114",
        TitleAccent: "#39FF14",
        FooterBg: "#111114",
        CloseButtonFg: "#6B6B7A",
        ComboDropBg: "#15151C",
        ComboDropBorder: "#3D3D4A",
        ComboItemHover: "#252A30",
        ComboItemSelected: "#1E3D28",
        ComboMultiHighlight: "#254530",
        ComboChromeBg: "#1E1E26",
        ComboHoverBorder: "#4A5A4E",
        ColorPrimaryGlow: "#FF39FF14",
        ActionPrimaryBg: "#1A2A1C",
        ActionPrimaryBgHover: "#23382A",
        ActionPrimaryBgPressed: "#152018",
        ActionSecondaryBg: "#1C1C22",
        ActionSecondaryHover: "#26262E",
        ActionSecondaryBorderHover: "#5A4A4A",
        ActionSecondaryPressed: "#141418");

    /// <summary>Original Gremlins light palette.</summary>
    public static readonly ThemeColors Light = new(
        BgDeep: "#EEEEF2",
        BgCard: "#FFFFFF",
        CardHover: "#E6E8EF",
        AccentGreen: "#0A7A1A",
        AccentGreenDim: "#8BCB98",
        AccentRed: "#D92B2B",
        TextPrimary: "#12121A",
        TextMuted: "#5C5C6C",
        Border: "#C6C6D2",
        WindowBg: "#EEEEF2",
        BorderOuter: "#B8B8C8",
        TitleBar: "#E2E2EA",
        TitleAccent: "#0A7A1A",
        FooterBg: "#E2E2EA",
        CloseButtonFg: "#5A5A6A",
        ComboDropBg: "#FFFFFF",
        ComboDropBorder: "#B0B0C0",
        ComboItemHover: "#E8EEF1",
        ComboItemSelected: "#C8E6D0",
        ComboMultiHighlight: "#A8D4B8",
        ComboChromeBg: "#FAFAFC",
        ComboHoverBorder: "#5A8A6A",
        ColorPrimaryGlow: "#FF0A7A1A",
        ActionPrimaryBg: "#C8E8D0",
        ActionPrimaryBgHover: "#A8DDB4",
        ActionPrimaryBgPressed: "#90D0A0",
        ActionSecondaryBg: "#E4E4EC",
        ActionSecondaryHover: "#D6D6E0",
        ActionSecondaryBorderHover: "#8A8A9A",
        ActionSecondaryPressed: "#C8C8D4");

    private static readonly ThemeColors _dracula = Dark with
    {
        BgDeep = "#282a36", BgCard = "#44475a", CardHover = "#6272a4", AccentGreen = "#50fa7b",
        AccentGreenDim = "#3d9c56", AccentRed = "#ff5555", TextPrimary = "#f8f8f2", TextMuted = "#6272a4",
        Border = "#44475a", WindowBg = "#282a36", BorderOuter = "#44475a", TitleBar = "#21222c",
        TitleAccent = "#bd93f9", FooterBg = "#21222c", CloseButtonFg = "#6272a4", ComboDropBg = "#282a36",
        ComboDropBorder = "#6272a4", ComboItemHover = "#44475a", ComboItemSelected = "#2d4a32",
        ComboMultiHighlight = "#2f5238", ComboChromeBg = "#303241", ComboHoverBorder = "#bd93f9",
        ColorPrimaryGlow = "#FF50FA7B", ActionPrimaryBg = "#2a2d3a", ActionPrimaryBgHover = "#3a3d4f",
        ActionPrimaryBgPressed = "#1e1f28", ActionSecondaryBg = "#21222c", ActionSecondaryHover = "#2e303e",
        ActionSecondaryBorderHover = "#bd93f9", ActionSecondaryPressed = "#1a1b22",
    };

    private static readonly ThemeColors _nord = Dark with
    {
        BgDeep = "#2e3440", BgCard = "#3b4252", CardHover = "#434c5e", AccentGreen = "#88c0d0",
        AccentGreenDim = "#5e81ac", AccentRed = "#bf616a", TextPrimary = "#eceff4", TextMuted = "#d8dee9",
        Border = "#4c566a", WindowBg = "#2e3440", BorderOuter = "#4c566a", TitleBar = "#272c36",
        TitleAccent = "#88c0d0", FooterBg = "#272c36", CloseButtonFg = "#d8dee9", ComboDropBg = "#2e3440",
        ComboDropBorder = "#4c566a", ComboItemHover = "#434c5e", ComboItemSelected = "#2e4a4a",
        ComboMultiHighlight = "#335050", ComboChromeBg = "#323945", ComboHoverBorder = "#8fbcbb",
        ColorPrimaryGlow = "#FF88C0D0", ActionPrimaryBg = "#2a3440", ActionPrimaryBgHover = "#344050",
        ActionPrimaryBgPressed = "#232b36", ActionSecondaryBg = "#272c36", ActionSecondaryHover = "#323845",
        ActionSecondaryBorderHover = "#88c0d0", ActionSecondaryPressed = "#1f242d",
    };

    private static readonly ThemeColors _gruvDark = Dark with
    {
        BgDeep = "#282828", BgCard = "#3c3836", CardHover = "#504945", AccentGreen = "#b8bb26",
        AccentGreenDim = "#79740e", AccentRed = "#fb4934", TextPrimary = "#ebdbb2", TextMuted = "#a89984",
        Border = "#504945", WindowBg = "#282828", BorderOuter = "#504945", TitleBar = "#1d2021",
        TitleAccent = "#fabd2f", FooterBg = "#1d2021", CloseButtonFg = "#a89984", ComboDropBg = "#282828",
        ComboDropBorder = "#665c54", ComboItemHover = "#504945", ComboItemSelected = "#3d4220",
        ComboMultiHighlight = "#4a4f22", ComboChromeBg = "#32302f", ComboHoverBorder = "#b8bb26",
        ColorPrimaryGlow = "#FFB8BB26", ActionPrimaryBg = "#32361f", ActionPrimaryBgHover = "#3d4228",
        ActionPrimaryBgPressed = "#252819", ActionSecondaryBg = "#1d2021", ActionSecondaryHover = "#282828",
        ActionSecondaryBorderHover = "#fabd2f", ActionSecondaryPressed = "#141617",
    };

    private static readonly ThemeColors _gruvLight = Light with
    {
        BgDeep = "#fbf1c7", BgCard = "#ebdbb2", CardHover = "#d5c4a1", AccentGreen = "#79740e",
        AccentGreenDim = "#bdae93", AccentRed = "#cc241d", TextPrimary = "#3c3836", TextMuted = "#665c54",
        Border = "#bdae93", WindowBg = "#fbf1c7", BorderOuter = "#bdae93", TitleBar = "#ebdbb2",
        TitleAccent = "#79740e", FooterBg = "#ebdbb2", CloseButtonFg = "#665c54", ComboDropBg = "#fbf1c7",
        ComboDropBorder = "#bdae93", ComboItemHover = "#d5c4a1", ComboItemSelected = "#c6d5a8",
        ComboMultiHighlight = "#b8c99a", ComboChromeBg = "#f2e5bc", ComboHoverBorder = "#79740e",
        ColorPrimaryGlow = "#FF79740E", ActionPrimaryBg = "#d5e5c0", ActionPrimaryBgHover = "#c5d5b0",
        ActionPrimaryBgPressed = "#b5c9a0", ActionSecondaryBg = "#ebdbb2", ActionSecondaryHover = "#d5c4a1",
        ActionSecondaryBorderHover = "#665c54", ActionSecondaryPressed = "#d0c3a3",
    };

    private static readonly ThemeColors _solDark = Dark with
    {
        BgDeep = "#002b36", BgCard = "#073642", CardHover = "#586e75", AccentGreen = "#2aa198",
        AccentGreenDim = "#134e52", AccentRed = "#dc322f", TextPrimary = "#839496", TextMuted = "#586e75",
        Border = "#073642", WindowBg = "#002b36", BorderOuter = "#073642", TitleBar = "#001f27",
        TitleAccent = "#268bd2", FooterBg = "#001f27", CloseButtonFg = "#586e75", ComboDropBg = "#002b36",
        ComboDropBorder = "#586e75", ComboItemHover = "#073642", ComboItemSelected = "#16404a",
        ComboMultiHighlight = "#1a4a52", ComboChromeBg = "#0a3942", ComboHoverBorder = "#2aa198",
        ColorPrimaryGlow = "#FF2AA198", ActionPrimaryBg = "#053844", ActionPrimaryBgHover = "#0a4a58",
        ActionPrimaryBgPressed = "#042a34", ActionSecondaryBg = "#001f27", ActionSecondaryHover = "#073642",
        ActionSecondaryBorderHover = "#268bd2", ActionSecondaryPressed = "#00141a",
    };

    private static readonly ThemeColors _solLight = Light with
    {
        BgDeep = "#fdf6e3", BgCard = "#eee8d5", CardHover = "#e8e2d0", AccentGreen = "#859900",
        AccentGreenDim = "#b58900", AccentRed = "#dc322f", TextPrimary = "#657b83", TextMuted = "#93a1a1",
        Border = "#d9d2c2", WindowBg = "#fdf6e3", BorderOuter = "#d9d2c2", TitleBar = "#eee8d5",
        TitleAccent = "#268bd2", FooterBg = "#eee8d5", CloseButtonFg = "#93a1a1", ComboDropBg = "#fdf6e3",
        ComboDropBorder = "#d9d2c2", ComboItemHover = "#e8e2d0", ComboItemSelected = "#c8e6c0",
        ComboMultiHighlight = "#b8ddb0", ComboChromeBg = "#f5efd8", ComboHoverBorder = "#859900",
        ColorPrimaryGlow = "#FF859900", ActionPrimaryBg = "#e0f0d8", ActionPrimaryBgHover = "#d0e8c8",
        ActionPrimaryBgPressed = "#c0e0b8", ActionSecondaryBg = "#eee8d5", ActionSecondaryHover = "#e8e2d0",
        ActionSecondaryBorderHover = "#93a1a1", ActionSecondaryPressed = "#dcd6cc",
    };

    private static readonly ThemeColors _tokyo = Dark with
    {
        BgDeep = "#1a1b26", BgCard = "#24283b", CardHover = "#414868", AccentGreen = "#7aa2f7",
        AccentGreenDim = "#3d59a1", AccentRed = "#f7768e", TextPrimary = "#c0caf5", TextMuted = "#565f89",
        Border = "#3b4261", WindowBg = "#1a1b26", BorderOuter = "#3b4261", TitleBar = "#16161e",
        TitleAccent = "#bb9af7", FooterBg = "#16161e", CloseButtonFg = "#565f89", ComboDropBg = "#1a1b26",
        ComboDropBorder = "#545c7e", ComboItemHover = "#292e42", ComboItemSelected = "#2a3655",
        ComboMultiHighlight = "#2f3f5a", ComboChromeBg = "#1f2335", ComboHoverBorder = "#7aa2f7",
        ColorPrimaryGlow = "#FF7AA2F7", ActionPrimaryBg = "#1f2740", ActionPrimaryBgHover = "#2a3250",
        ActionPrimaryBgPressed = "#1a2135", ActionSecondaryBg = "#16161e", ActionSecondaryHover = "#222436",
        ActionSecondaryBorderHover = "#bb9af7", ActionSecondaryPressed = "#13141a",
    };

    private static readonly ThemeColors _catMocha = Dark with
    {
        BgDeep = "#1e1e2e", BgCard = "#313244", CardHover = "#45475a", AccentGreen = "#a6e3a1",
        AccentGreenDim = "#5e9362", AccentRed = "#f38ba8", TextPrimary = "#cdd6f4", TextMuted = "#6c7086",
        Border = "#45475a", WindowBg = "#1e1e2e", BorderOuter = "#45475a", TitleBar = "#181825",
        TitleAccent = "#cba6f7", FooterBg = "#181825", CloseButtonFg = "#6c7086", ComboDropBg = "#1e1e2e",
        ComboDropBorder = "#585b70", ComboItemHover = "#313244", ComboItemSelected = "#2a3f30",
        ComboMultiHighlight = "#2f4a35", ComboChromeBg = "#252536", ComboHoverBorder = "#a6e3a1",
        ColorPrimaryGlow = "#FFA6E3A1", ActionPrimaryBg = "#242e32", ActionPrimaryBgHover = "#2e3c42",
        ActionPrimaryBgPressed = "#1c2528", ActionSecondaryBg = "#181825", ActionSecondaryHover = "#252536",
        ActionSecondaryBorderHover = "#cba6f7", ActionSecondaryPressed = "#14141c",
    };

    private static readonly ThemeColors _catLatte = Light with
    {
        BgDeep = "#eff1f5", BgCard = "#e6e9ef", CardHover = "#dce0e8", AccentGreen = "#40a02b",
        AccentGreenDim = "#95c08d", AccentRed = "#d20f39", TextPrimary = "#4c4f69", TextMuted = "#6c6f85",
        Border = "#ccd0da", WindowBg = "#eff1f5", BorderOuter = "#ccd0da", TitleBar = "#e6e9ef",
        TitleAccent = "#8839ef", FooterBg = "#e6e9ef", CloseButtonFg = "#6c6f85", ComboDropBg = "#eff1f5",
        ComboDropBorder = "#ccd0da", ComboItemHover = "#dce0e8", ComboItemSelected = "#c8e6d0",
        ComboMultiHighlight = "#b8ddb8", ComboChromeBg = "#f4f6fa", ComboHoverBorder = "#40a02b",
        ColorPrimaryGlow = "#FF40A02B", ActionPrimaryBg = "#d8f0d4", ActionPrimaryBgHover = "#c8e8c4",
        ActionPrimaryBgPressed = "#b8e0b4", ActionSecondaryBg = "#e6e9ef", ActionSecondaryHover = "#dce0e8",
        ActionSecondaryBorderHover = "#6c6f85", ActionSecondaryPressed = "#d0d4dc",
    };

    private static readonly ThemeColors _oneDark = Dark with
    {
        BgDeep = "#282c34", BgCard = "#2c323c", CardHover = "#3e4451", AccentGreen = "#98c379",
        AccentGreenDim = "#5f8248", AccentRed = "#e06c75", TextPrimary = "#abb2bf", TextMuted = "#5c6370",
        Border = "#3e4451", WindowBg = "#282c34", BorderOuter = "#3e4451", TitleBar = "#21252b",
        TitleAccent = "#61afef", FooterBg = "#21252b", CloseButtonFg = "#5c6370", ComboDropBg = "#282c34",
        ComboDropBorder = "#4b5263", ComboItemHover = "#323842", ComboItemSelected = "#2a4030",
        ComboMultiHighlight = "#2f4a35", ComboChromeBg = "#2d3139", ComboHoverBorder = "#98c379",
        ColorPrimaryGlow = "#FF98C379", ActionPrimaryBg = "#2a3228", ActionPrimaryBgHover = "#344030",
        ActionPrimaryBgPressed = "#232a20", ActionSecondaryBg = "#21252b", ActionSecondaryHover = "#2c313a",
        ActionSecondaryBorderHover = "#61afef", ActionSecondaryPressed = "#1b1e24",
    };

    private static readonly ThemeColors _monokai = Dark with
    {
        BgDeep = "#272822", BgCard = "#3e3d32", CardHover = "#49483e", AccentGreen = "#a6e22e",
        AccentGreenDim = "#6a8018", AccentRed = "#f92672", TextPrimary = "#f8f8f2", TextMuted = "#75715e",
        Border = "#49483e", WindowBg = "#272822", BorderOuter = "#49483e", TitleBar = "#1e1f1c",
        TitleAccent = "#fd971f", FooterBg = "#1e1f1c", CloseButtonFg = "#75715e", ComboDropBg = "#272822",
        ComboDropBorder = "#665e54", ComboItemHover = "#3e3d32", ComboItemSelected = "#3a4a20",
        ComboMultiHighlight = "#425522", ComboChromeBg = "#30312c", ComboHoverBorder = "#a6e22e",
        ColorPrimaryGlow = "#FFA6E22E", ActionPrimaryBg = "#323520", ActionPrimaryBgHover = "#3e4228",
        ActionPrimaryBgPressed = "#282a1c", ActionSecondaryBg = "#1e1f1c", ActionSecondaryHover = "#2d2e28",
        ActionSecondaryBorderHover = "#fd971f", ActionSecondaryPressed = "#181916",
    };

    private static readonly ThemeColors _rosePine = Dark with
    {
        BgDeep = "#191724", BgCard = "#26233a", CardHover = "#403d52", AccentGreen = "#ebbcba",
        AccentGreenDim = "#a87685", AccentRed = "#eb6f92", TextPrimary = "#e0def4", TextMuted = "#6e6a86",
        Border = "#403d52", WindowBg = "#191724", BorderOuter = "#403d52", TitleBar = "#16141f",
        TitleAccent = "#c4a7e7", FooterBg = "#16141f", CloseButtonFg = "#6e6a86", ComboDropBg = "#191724",
        ComboDropBorder = "#524f67", ComboItemHover = "#26233a", ComboItemSelected = "#3a2f40",
        ComboMultiHighlight = "#423548", ComboChromeBg = "#201e2b", ComboHoverBorder = "#ebbcba",
        ColorPrimaryGlow = "#FFEBBCBA", ActionPrimaryBg = "#2d2535", ActionPrimaryBgHover = "#3a3045",
        ActionPrimaryBgPressed = "#251f2c", ActionSecondaryBg = "#16141f", ActionSecondaryHover = "#221f30",
        ActionSecondaryBorderHover = "#c4a7e7", ActionSecondaryPressed = "#12101a",
    };

    private static readonly ThemeColors _everforest = Dark with
    {
        BgDeep = "#2d353b", BgCard = "#343f44", CardHover = "#475258", AccentGreen = "#a7c080",
        AccentGreenDim = "#6f8950", AccentRed = "#e67e80", TextPrimary = "#d3c6aa", TextMuted = "#859289",
        Border = "#475258", WindowBg = "#2d353b", BorderOuter = "#475258", TitleBar = "#252f34",
        TitleAccent = "#7fbbb3", FooterBg = "#252f34", CloseButtonFg = "#859289", ComboDropBg = "#2d353b",
        ComboDropBorder = "#56635f", ComboItemHover = "#3d484d", ComboItemSelected = "#354a38",
        ComboMultiHighlight = "#3d5540", ComboChromeBg = "#323c41", ComboHoverBorder = "#a7c080",
        ColorPrimaryGlow = "#FFA7C080", ActionPrimaryBg = "#2f3d34", ActionPrimaryBgHover = "#3a4d40",
        ActionPrimaryBgPressed = "#28332c", ActionSecondaryBg = "#252f34", ActionSecondaryHover = "#343f44",
        ActionSecondaryBorderHover = "#7fbbb3", ActionSecondaryPressed = "#1f272b",
    };

    private static readonly ThemeColors _ayu = Dark with
    {
        BgDeep = "#1f2430", BgCard = "#191e2a", CardHover = "#2f3440", AccentGreen = "#ffcc66",
        AccentGreenDim = "#b88a30", AccentRed = "#f28779", TextPrimary = "#cbccc6", TextMuted = "#707a8c",
        Border = "#33415e", WindowBg = "#1f2430", BorderOuter = "#33415e", TitleBar = "#1a1e28",
        TitleAccent = "#74c0e8", FooterBg = "#1a1e28", CloseButtonFg = "#707a8c", ComboDropBg = "#1f2430",
        ComboDropBorder = "#3d4f6f", ComboItemHover = "#2a3040", ComboItemSelected = "#4a4020",
        ComboMultiHighlight = "#524825", ComboChromeBg = "#252b38", ComboHoverBorder = "#ffcc66",
        ColorPrimaryGlow = "#FFFFCC66", ActionPrimaryBg = "#3a3220", ActionPrimaryBgHover = "#4a4028",
        ActionPrimaryBgPressed = "#302a1a", ActionSecondaryBg = "#1a1e28", ActionSecondaryHover = "#262c38",
        ActionSecondaryBorderHover = "#74c0e8", ActionSecondaryPressed = "#151820",
    };

    private static readonly ThemeColors _nightOwl = Dark with
    {
        BgDeep = "#011627", BgCard = "#0b253a", CardHover = "#1d3b53", AccentGreen = "#7fdbca",
        AccentGreenDim = "#3a8a7a", AccentRed = "#ef5350", TextPrimary = "#d6deeb", TextMuted = "#637777",
        Border = "#1d3b53", WindowBg = "#011627", BorderOuter = "#1d3b53", TitleBar = "#010e1a",
        TitleAccent = "#82aaff", FooterBg = "#010e1a", CloseButtonFg = "#637777", ComboDropBg = "#011627",
        ComboDropBorder = "#2a4a68", ComboItemHover = "#0b253a", ComboItemSelected = "#1a4a45",
        ComboMultiHighlight = "#1f5550", ComboChromeBg = "#051f30", ComboHoverBorder = "#7fdbca",
        ColorPrimaryGlow = "#FF7FDBCA", ActionPrimaryBg = "#0d3040", ActionPrimaryBgHover = "#154050",
        ActionPrimaryBgPressed = "#0a2835", ActionSecondaryBg = "#010e1a", ActionSecondaryHover = "#0b253a",
        ActionSecondaryBorderHover = "#82aaff", ActionSecondaryPressed = "#010810",
    };

    private static readonly ThemeColors _purple = Dark with
    {
        BgDeep = "#2d2b55", BgCard = "#342d58", CardHover = "#443c72", AccentGreen = "#fad000",
        AccentGreenDim = "#a08a00", AccentRed = "#ff6188", TextPrimary = "#a599ff", TextMuted = "#7870a3",
        Border = "#52489c", WindowBg = "#2d2b55", BorderOuter = "#52489c", TitleBar = "#252347",
        TitleAccent = "#ff6188", FooterBg = "#252347", CloseButtonFg = "#7870a3", ComboDropBg = "#2d2b55",
        ComboDropBorder = "#5a5098", ComboItemHover = "#3a3468", ComboItemSelected = "#4a4520",
        ComboMultiHighlight = "#524d28", ComboChromeBg = "#322d58", ComboHoverBorder = "#fad000",
        ColorPrimaryGlow = "#FFFAD000", ActionPrimaryBg = "#3a3528", ActionPrimaryBgHover = "#4a4535",
        ActionPrimaryBgPressed = "#302c22", ActionSecondaryBg = "#252347", ActionSecondaryHover = "#342d58",
        ActionSecondaryBorderHover = "#ff6188", ActionSecondaryPressed = "#1e1c3d",
    };

    private static readonly ThemeColors _synth = Dark with
    {
        BgDeep = "#241b2f", BgCard = "#2f2140", CardHover = "#432374", AccentGreen = "#ff7edb",
        AccentGreenDim = "#a05090", AccentRed = "#ff5454", TextPrimary = "#f8f8f2", TextMuted = "#a599c8",
        Border = "#5a4a78", WindowBg = "#241b2f", BorderOuter = "#5a4a78", TitleBar = "#1c1426",
        TitleAccent = "#00e5ff", FooterBg = "#1c1426", CloseButtonFg = "#a599c8", ComboDropBg = "#241b2f",
        ComboDropBorder = "#6a5888", ComboItemHover = "#362050", ComboItemSelected = "#502858",
        ComboMultiHighlight = "#583060", ComboChromeBg = "#2a1f38", ComboHoverBorder = "#ff7edb",
        ColorPrimaryGlow = "#FFFF7EDB", ActionPrimaryBg = "#402040", ActionPrimaryBgHover = "#502858",
        ActionPrimaryBgPressed = "#351a38", ActionSecondaryBg = "#1c1426", ActionSecondaryHover = "#2f2140",
        ActionSecondaryBorderHover = "#00e5ff", ActionSecondaryPressed = "#16101e",
    };

    private static readonly ThemeColors _matrix = Dark with
    {
        BgDeep = "#000510", BgCard = "#001a0a", CardHover = "#003018", AccentGreen = "#00ff41",
        AccentGreenDim = "#007020", AccentRed = "#ff0044", TextPrimary = "#c8ffc8", TextMuted = "#3a8550",
        Border = "#004020", WindowBg = "#000510", BorderOuter = "#004020", TitleBar = "#000408",
        TitleAccent = "#00ff41", FooterBg = "#000408", CloseButtonFg = "#3a8550", ComboDropBg = "#000510",
        ComboDropBorder = "#005030", ComboItemHover = "#002818", ComboItemSelected = "#004020",
        ComboMultiHighlight = "#005028", ComboChromeBg = "#001208", ComboHoverBorder = "#00ff41",
        ColorPrimaryGlow = "#FF00FF41", ActionPrimaryBg = "#002010", ActionPrimaryBgHover = "#003820",
        ActionPrimaryBgPressed = "#001810", ActionSecondaryBg = "#000408", ActionSecondaryHover = "#001a10",
        ActionSecondaryBorderHover = "#00cc33", ActionSecondaryPressed = "#000304",
    };

    private static readonly ThemeColors _amber = Dark with
    {
        BgDeep = "#1a0f00", BgCard = "#2a1805", CardHover = "#3d2408", AccentGreen = "#ffb000",
        AccentGreenDim = "#805000", AccentRed = "#ff4444", TextPrimary = "#ffe8c8", TextMuted = "#a07040",
        Border = "#4a3010", WindowBg = "#1a0f00", BorderOuter = "#4a3010", TitleBar = "#140b00",
        TitleAccent = "#ffcc33", FooterBg = "#140b00", CloseButtonFg = "#a07040", ComboDropBg = "#1a0f00",
        ComboDropBorder = "#5a4020", ComboItemHover = "#2a1808", ComboItemSelected = "#4a3818",
        ComboMultiHighlight = "#524018", ComboChromeBg = "#221308", ComboHoverBorder = "#ffb000",
        ColorPrimaryGlow = "#FFFFB000", ActionPrimaryBg = "#3a2808", ActionPrimaryBgHover = "#4a3410",
        ActionPrimaryBgPressed = "#2a1c08", ActionSecondaryBg = "#140b00", ActionSecondaryHover = "#221308",
        ActionSecondaryBorderHover = "#ffcc33", ActionSecondaryPressed = "#0f0800",
    };

    private static readonly ThemeColors _hotDog = Light with
    {
        BgDeep = "#ffff00", BgCard = "#fff44f", CardHover = "#ffe135", AccentGreen = "#ff0000",
        AccentGreenDim = "#aa0000", AccentRed = "#0000ff", TextPrimary = "#1a1a1a", TextMuted = "#444444",
        Border = "#cc0000", WindowBg = "#ffff00", BorderOuter = "#cc0000", TitleBar = "#ffee00",
        TitleAccent = "#ff0000", FooterBg = "#ffee00", CloseButtonFg = "#444444", ComboDropBg = "#ffffff",
        ComboDropBorder = "#cc0000", ComboItemHover = "#ffe8e8", ComboItemSelected = "#ffc8c8",
        ComboMultiHighlight = "#ffb0b0", ComboChromeBg = "#fff8dc", ComboHoverBorder = "#0000ff",
        ColorPrimaryGlow = "#FFFF0000", ActionPrimaryBg = "#ffd0d0", ActionPrimaryBgHover = "#ffb8b8",
        ActionPrimaryBgPressed = "#ffa0a0", ActionSecondaryBg = "#eeee00", ActionSecondaryHover = "#dddd00",
        ActionSecondaryBorderHover = "#0000aa", ActionSecondaryPressed = "#cccc00",
    };

    private static readonly ThemeColors _vapor = Light with
    {
        BgDeep = "#ffe4f0", BgCard = "#ffd6ec", CardHover = "#ffc8e8", AccentGreen = "#ff71ce",
        AccentGreenDim = "#c05098", AccentRed = "#01cdfe", TextPrimary = "#2d1b3d", TextMuted = "#7a5a8a",
        Border = "#e8b8d8", WindowBg = "#ffe4f0", BorderOuter = "#e8b8d8", TitleBar = "#ffd6ec",
        TitleAccent = "#01cdfe", FooterBg = "#ffd6ec", CloseButtonFg = "#7a5a8a", ComboDropBg = "#fff0f8",
        ComboDropBorder = "#e0a8d0", ComboItemHover = "#ffe0f4", ComboItemSelected = "#e0f8ff",
        ComboMultiHighlight = "#d0f0ff", ComboChromeBg = "#fff5fb", ComboHoverBorder = "#ff71ce",
        ColorPrimaryGlow = "#FFFF71CE", ActionPrimaryBg = "#ffe0f8", ActionPrimaryBgHover = "#ffd0f0",
        ActionPrimaryBgPressed = "#ffc0e8", ActionSecondaryBg = "#ffd6ec", ActionSecondaryHover = "#ffc8e8",
        ActionSecondaryBorderHover = "#7a5a8a", ActionSecondaryPressed = "#f0c8e0",
    };

    private static readonly ThemeColors _paper = Light with
    {
        BgDeep = "#f5f5f0", BgCard = "#ffffff", CardHover = "#ebebe5", AccentGreen = "#1a1a1a",
        AccentGreenDim = "#666666", AccentRed = "#b30000", TextPrimary = "#111111", TextMuted = "#666666",
        Border = "#cccccc", WindowBg = "#f5f5f0", BorderOuter = "#b0b0b0", TitleBar = "#eaeae5",
        TitleAccent = "#111111", FooterBg = "#eaeae5", CloseButtonFg = "#666666", ComboDropBg = "#ffffff",
        ComboDropBorder = "#cccccc", ComboItemHover = "#f0f0ec", ComboItemSelected = "#e0e0dc",
        ComboMultiHighlight = "#d8d8d4", ComboChromeBg = "#fafaf8", ComboHoverBorder = "#333333",
        ColorPrimaryGlow = "#FF111111", ActionPrimaryBg = "#e8e8e4", ActionPrimaryBgHover = "#dcdcd8",
        ActionPrimaryBgPressed = "#d0d0cc", ActionSecondaryBg = "#eaeae5", ActionSecondaryHover = "#deded8",
        ActionSecondaryBorderHover = "#888888", ActionSecondaryPressed = "#d4d4d0",
    };

    private static readonly ThemeColors _glitch = Dark with
    {
        BgDeep = "#0d0221", BgCard = "#1a0a3a", CardHover = "#2a1050", AccentGreen = "#ff00ff",
        AccentGreenDim = "#880088", AccentRed = "#00ffff", TextPrimary = "#f0f0ff", TextMuted = "#8888cc",
        Border = "#6600ff", WindowBg = "#0d0221", BorderOuter = "#6600ff", TitleBar = "#080118",
        TitleAccent = "#00ffff", FooterBg = "#080118", CloseButtonFg = "#8888cc", ComboDropBg = "#120428",
        ComboDropBorder = "#aa00ff", ComboItemHover = "#220a48", ComboItemSelected = "#402060",
        ComboMultiHighlight = "#502878", ComboChromeBg = "#160830", ComboHoverBorder = "#ff00ff",
        ColorPrimaryGlow = "#FFFF00FF", ActionPrimaryBg = "#301050", ActionPrimaryBgHover = "#401868",
        ActionPrimaryBgPressed = "#280840", ActionSecondaryBg = "#080118", ActionSecondaryHover = "#180828",
        ActionSecondaryBorderHover = "#00ffff", ActionSecondaryPressed = "#050410",
    };

    private static readonly ThemeColors _unicorn = Dark with
    {
        BgDeep = "#2a0a2e", BgCard = "#450a4a", CardHover = "#6a0f72", AccentGreen = "#ff44ff",
        AccentGreenDim = "#aa22aa", AccentRed = "#44ffff", TextPrimary = "#fff0ff", TextMuted = "#cc88dd",
        Border = "#aa44cc", WindowBg = "#2a0a2e", BorderOuter = "#aa44cc", TitleBar = "#200828",
        TitleAccent = "#ffff44", FooterBg = "#200828", CloseButtonFg = "#cc88dd", ComboDropBg = "#320a38",
        ComboDropBorder = "#cc66ee", ComboItemHover = "#501058", ComboItemSelected = "#604018",
        ComboMultiHighlight = "#705028", ComboChromeBg = "#3a0c40", ComboHoverBorder = "#ff44ff",
        ColorPrimaryGlow = "#FFFF44FF", ActionPrimaryBg = "#501858", ActionPrimaryBgHover = "#682070",
        ActionPrimaryBgPressed = "#401450", ActionSecondaryBg = "#200828", ActionSecondaryHover = "#380a40",
        ActionSecondaryBorderHover = "#44ffff", ActionSecondaryPressed = "#180620",
    };

    private static readonly ThemeColors _beige = Light with
    {
        BgDeep = "#e8dcc8", BgCard = "#ddd0b8", CardHover = "#d4c4a8", AccentGreen = "#8b7355",
        AccentGreenDim = "#6a5a44", AccentRed = "#a0522d", TextPrimary = "#4a4035", TextMuted = "#7a6e5c",
        Border = "#c4b498", WindowBg = "#e8dcc8", BorderOuter = "#c4b498", TitleBar = "#ddd0b8",
        TitleAccent = "#8b4513", FooterBg = "#ddd0b8", CloseButtonFg = "#7a6e5c", ComboDropBg = "#f0e8d8",
        ComboDropBorder = "#b8a888", ComboItemHover = "#e0d4c0", ComboItemSelected = "#d8ccb8",
        ComboMultiHighlight = "#d0c4b0", ComboChromeBg = "#ede4d4", ComboHoverBorder = "#8b7355",
        ColorPrimaryGlow = "#FF8B7355", ActionPrimaryBg = "#dcd0b8", ActionPrimaryBgHover = "#d4c8b0",
        ActionPrimaryBgPressed = "#ccc0a8", ActionSecondaryBg = "#ddd0b8", ActionSecondaryHover = "#d4c4a8",
        ActionSecondaryBorderHover = "#6a5a4a", ActionSecondaryPressed = "#ccc4b0",
    };

    private static readonly ThemeColors _miami = Dark with
    {
        BgDeep = "#1a0a2e", BgCard = "#240046", CardHover = "#3d0066", AccentGreen = "#ff71ce",
        AccentGreenDim = "#a04088", AccentRed = "#01cdfe", TextPrimary = "#eaf2ff", TextMuted = "#8877aa",
        Border = "#5a30a0", WindowBg = "#1a0a2e", BorderOuter = "#5a30a0", TitleBar = "#140828",
        TitleAccent = "#01cdfe", FooterBg = "#140828", CloseButtonFg = "#8877aa", ComboDropBg = "#1e0a38",
        ComboDropBorder = "#7040b0", ComboItemHover = "#2e0858", ComboItemSelected = "#402858",
        ComboMultiHighlight = "#483868", ComboChromeBg = "#220a40", ComboHoverBorder = "#ff71ce",
        ColorPrimaryGlow = "#FFFF71CE", ActionPrimaryBg = "#381848", ActionPrimaryBgHover = "#482868",
        ActionPrimaryBgPressed = "#301040", ActionSecondaryBg = "#140828", ActionSecondaryHover = "#240838",
        ActionSecondaryBorderHover = "#01cdfe", ActionSecondaryPressed = "#0f0618",
    };

    private static readonly ThemeColors _corp = Light with
    {
        BgDeep = "#f5f5f5", BgCard = "#eeeeee", CardHover = "#e0e0e0", AccentGreen = "#0066cc",
        AccentGreenDim = "#4a90c8", AccentRed = "#cc3300", TextPrimary = "#333333", TextMuted = "#666666",
        Border = "#cccccc", WindowBg = "#f5f5f5", BorderOuter = "#bbbbbb", TitleBar = "#e8e8e8",
        TitleAccent = "#0066cc", FooterBg = "#e8e8e8", CloseButtonFg = "#666666", ComboDropBg = "#ffffff",
        ComboDropBorder = "#cccccc", ComboItemHover = "#f0f0f0", ComboItemSelected = "#d0e8ff",
        ComboMultiHighlight = "#c0e0ff", ComboChromeBg = "#fafafa", ComboHoverBorder = "#0066cc",
        ColorPrimaryGlow = "#FF0066CC", ActionPrimaryBg = "#d8e8f8", ActionPrimaryBgHover = "#c8ddf4",
        ActionPrimaryBgPressed = "#b8d0f0", ActionSecondaryBg = "#eeeeee", ActionSecondaryHover = "#e0e0e0",
        ActionSecondaryBorderHover = "#888888", ActionSecondaryPressed = "#d8d8d8",
    };

    private static readonly ThemeColors _zombie = Dark with
    {
        BgDeep = "#1a2218", BgCard = "#2a3328", CardHover = "#3a4438", AccentGreen = "#7cbd68",
        AccentGreenDim = "#4a7038", AccentRed = "#c48b7f", TextPrimary = "#d8e0d0", TextMuted = "#6a7a64",
        Border = "#4a5448", WindowBg = "#1a2218", BorderOuter = "#4a5448", TitleBar = "#141a12",
        TitleAccent = "#9acd32", FooterBg = "#141a12", CloseButtonFg = "#6a7a64", ComboDropBg = "#1a2218",
        ComboDropBorder = "#5a6458", ComboItemHover = "#2a3328", ComboItemSelected = "#2a4025",
        ComboMultiHighlight = "#324a30", ComboChromeBg = "#1e2818", ComboHoverBorder = "#7cbd68",
        ColorPrimaryGlow = "#FF7CBD68", ActionPrimaryBg = "#2a3825", ActionPrimaryBgHover = "#364830",
        ActionPrimaryBgPressed = "#222c1e", ActionSecondaryBg = "#141a12", ActionSecondaryHover = "#202820",
        ActionSecondaryBorderHover = "#9acd32", ActionSecondaryPressed = "#101410",
    };

    private static readonly ThemeColors _steam = Dark with
    {
        BgDeep = "#2b1f14", BgCard = "#3d2a1a", CardHover = "#4a3520", AccentGreen = "#cd853f",
        AccentGreenDim = "#8a5a28", AccentRed = "#c04040", TextPrimary = "#e8dcc8", TextMuted = "#a08060",
        Border = "#5c4030", WindowBg = "#2b1f14", BorderOuter = "#5c4030", TitleBar = "#231810",
        TitleAccent = "#daa520", FooterBg = "#231810", CloseButtonFg = "#a08060", ComboDropBg = "#2b1f14",
        ComboDropBorder = "#6a5040", ComboItemHover = "#3d2a1a", ComboItemSelected = "#4a3820",
        ComboMultiHighlight = "#524028", ComboChromeBg = "#322418", ComboHoverBorder = "#cd853f",
        ColorPrimaryGlow = "#FFCD853F", ActionPrimaryBg = "#3d3020", ActionPrimaryBgHover = "#4a3a28",
        ActionPrimaryBgPressed = "#322818", ActionSecondaryBg = "#231810", ActionSecondaryHover = "#322418",
        ActionSecondaryBorderHover = "#daa520", ActionSecondaryPressed = "#1a140c",
    };

    private static readonly ThemeColors _clippy = Light with
    {
        BgDeep = "#dce8f8", BgCard = "#c8daf4", CardHover = "#b8d0f0", AccentGreen = "#316ac5",
        AccentGreenDim = "#2050a0", AccentRed = "#d93025", TextPrimary = "#1a1a2e", TextMuted = "#4a5688",
        Border = "#a0b8e0", WindowBg = "#dce8f8", BorderOuter = "#a0b8e0", TitleBar = "#d0e0f4",
        TitleAccent = "#316ac5", FooterBg = "#d0e0f4", CloseButtonFg = "#4a5688", ComboDropBg = "#eef4fc",
        ComboDropBorder = "#90a8d8", ComboItemHover = "#e0ecf8", ComboItemSelected = "#c8ddf0",
        ComboMultiHighlight = "#b8d4ec", ComboChromeBg = "#f0f4fc", ComboHoverBorder = "#316ac5",
        ColorPrimaryGlow = "#FF316AC5", ActionPrimaryBg = "#b8d4f0", ActionPrimaryBgHover = "#a8c8ec",
        ActionPrimaryBgPressed = "#98bce8", ActionSecondaryBg = "#c8daf4", ActionSecondaryHover = "#b8d0f0",
        ActionSecondaryBorderHover = "#4a5688", ActionSecondaryPressed = "#b0c8e8",
    };

    private static readonly IReadOnlyDictionary<AppThemePreference, ThemePaletteEntry> _map = BuildMap();

    static ThemePalettes()
    {
        foreach (AppThemePreference p in Enum.GetValues<AppThemePreference>())
        {
            if (p == AppThemePreference.System)
                continue;
            if (!_map.ContainsKey(p))
                throw new InvalidOperationException($"Missing palette for theme {p}.");
        }
    }

    private static IReadOnlyDictionary<AppThemePreference, ThemePaletteEntry> BuildMap()
    {
        var d = new Dictionary<AppThemePreference, ThemePaletteEntry>
        {
            [AppThemePreference.Dark] = new(Dark, true),
            [AppThemePreference.Light] = new(Light, false),
            [AppThemePreference.Dracula] = new(_dracula, true),
            [AppThemePreference.Nord] = new(_nord, true),
            [AppThemePreference.GruvboxDark] = new(_gruvDark, true),
            [AppThemePreference.GruvboxLight] = new(_gruvLight, false),
            [AppThemePreference.SolarizedDark] = new(_solDark, true),
            [AppThemePreference.SolarizedLight] = new(_solLight, false),
            [AppThemePreference.TokyoNight] = new(_tokyo, true),
            [AppThemePreference.CatppuccinMocha] = new(_catMocha, true),
            [AppThemePreference.CatppuccinLatte] = new(_catLatte, false),
            [AppThemePreference.OneDark] = new(_oneDark, true),
            [AppThemePreference.Monokai] = new(_monokai, true),
            [AppThemePreference.RosePine] = new(_rosePine, true),
            [AppThemePreference.Everforest] = new(_everforest, true),
            [AppThemePreference.AyuMirage] = new(_ayu, true),
            [AppThemePreference.NightOwl] = new(_nightOwl, true),
            [AppThemePreference.ShadesOfPurple] = new(_purple, true),
            [AppThemePreference.Synthwave84] = new(_synth, true),
            [AppThemePreference.MatrixGreen] = new(_matrix, true),
            [AppThemePreference.AmberCRT] = new(_amber, true),
            [AppThemePreference.HotDogStand] = new(_hotDog, false),
            [AppThemePreference.VaporwavePastel] = new(_vapor, false),
            [AppThemePreference.PaperWhite] = new(_paper, false),
            [AppThemePreference.GlitchCore] = new(_glitch, true),
            [AppThemePreference.UnicornAcid] = new(_unicorn, true),
            [AppThemePreference.BeigeNightmare] = new(_beige, false),
            [AppThemePreference.MiamiVice] = new(_miami, true),
            [AppThemePreference.CorpBland] = new(_corp, false),
            [AppThemePreference.ZombieMold] = new(_zombie, true),
            [AppThemePreference.Steampunk] = new(_steam, true),
            [AppThemePreference.ClippyAssist] = new(_clippy, false),
        };

        for (var i = 1; i <= 64; i++)
        {
            var name = $"Prismatic{i:D2}";
            var key = (AppThemePreference)Enum.Parse(typeof(AppThemePreference), name);
            d[key] = new ThemePaletteEntry(
                ThemeResourceDictionaryFactory.ChromaticPrismatic(i - 1, 64), IsDark: true);
        }

        return d;
    }

    public static bool TryGet(AppThemePreference preference, out ThemePaletteEntry entry)
    {
        if (preference == AppThemePreference.System)
        {
            entry = default;
            return false;
        }

        return _map.TryGetValue(preference, out entry);
    }

    public static bool IsDarkAppearance(AppThemePreference preference, bool windowsUsesLightApps)
    {
        if (preference == AppThemePreference.System)
            return !windowsUsesLightApps;
        return TryGet(preference, out var e) && e.IsDark;
    }
}
