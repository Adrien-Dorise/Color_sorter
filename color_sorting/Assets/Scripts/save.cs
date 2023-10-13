using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// Class <c>Save</c> 
/// This class reference all ketword used with payerprefs
/// </summary>
public class save
{
    static public bool debugDev = false; // Set true to have access to all levels
    static public bool debugLevel = false; // Set true to remove tube animations during levels
    static public bool debugPower = false; // Set true to always access powers
    static public int maxAvailableLevels = 203; //Change when increase the number of levels in the game
    static public int resetLevel = 30; //Number of available levels whe hitting the reset button


    static public string robotColor = "Robot Color Per Level"; //Robot colors for each cleared level. The states are represented by characters in a string. ex: " 0 3" -> means level one start with color '0' and level two starts witg color '3'
    static public string availableLevels = "Available Levels"; //Number of levels available 
    static public string currentLevel = "Current Level"; //Store the level number the user is playing
    static public string playedLevel = "Played level";
    static public string colors = "Color";
    static public string powerToken = "Power token"; //Number of tokens available to the player
    static public string mainMenuMusicState = "Main menu music state";
    static public string levelMusicState = "Level music state";
    static public string musicVolume = "Music volume"; //Set the volume of music
    static public string soundVolume = "Sound volume"; //Set the volume of sound effects
    static public string bannerClick = "Banner click";
    static public string ad_strike = "Ad Strike"; //Number of consecutive ad watched by the player
    static public string localisation = "Localisation";
}
