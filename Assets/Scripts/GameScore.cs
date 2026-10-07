using UnityEngine;

public static class GameScore
{
    public static int Berries { get; private set; }
    public static int Coins { get; private set; }
    public static int Total => (Berries * 50) + (Coins * 10);

    // score at the start of the current level, used for retries
    private static int berriesAtLevelStart;
    private static int coinsAtLevelStart;

    public static void AddBerry() => Berries++;
    public static void AddCoin() => Coins++;

    // call when a level begins
    public static void MarkLevelStart()
    {
        berriesAtLevelStart = Berries;
        coinsAtLevelStart = Coins;
    }

    // call when the player retries the current level
    public static void RestoreLevelStart()
    {
        Berries = berriesAtLevelStart;
        Coins = coinsAtLevelStart;
    }

    // call when starting a brand new game (e.g. from the main menu)
    public static void ResetAll()
    {
        Berries = Coins = 0;
        berriesAtLevelStart = coinsAtLevelStart = 0;
    }

    // clears stale values when entering play mode in the editor
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void EditorReset() => ResetAll();
}