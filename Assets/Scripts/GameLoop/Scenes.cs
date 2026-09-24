/// <summary>
/// Centralized scene-name constants for the whole game. NO raw scene-name
/// string may exist outside this class — always load via Scenes.X so the
/// Menu → Map → Legend → Market → Prototype → Credits pipeline stays in one
/// place.
/// </summary>
public static class Scenes
{
    public const string Menu = "Menu";
    public const string Map = "Map";
    public const string Legend = "Legend";
    public const string Market = "Market";
    public const string Prototype = "Prototype";
    public const string Credits = "Credits";
    public const string Tutorial = "Tutorial";
}