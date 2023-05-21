using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotPower : MonoBehaviour
{
    /*
    On some levels, the player has a last color objective. A bonus goal and a malus color goal.
    Each color gives a special power up detailed in this class
    When the players finishes on either a bonus or malus color, the effect activates for the next level
    */

    public enum powerEnum {neutral, powerup, powerdown};
    //Contains whether a level is powered up or down. 
    static List<powerEnum> poweredLevel = new List<powerEnum>();


    private void savePoweredLevels()
    {

    }

    private void loadPoweredLevels()
    {
        //0 for neutral, 1 for powered up and 2 powered down
        if(!PlayerPrefs.HasKey(save.poweredLevel)) //If first time playing the game
        {
            string poweredSave = "";
            for(int i = 0; i < save.maxAvailableLevels; i++)
            {
                poweredSave += "0";
                poweredLevel.Add(powerEnum.neutral);
            }
        }
        else
        {
            string poweredSave = PlayerPrefs.GetString(save.poweredLevel);
        }
    }

    // !!! Bonus !!!
    /// <summary>
    /// Method <c>rollBackOne</c> allows the player to cancel is last moves.
    /// The game comes bakc entirely at the state before the last action
    /// </summary>
    private void rollBackOne()
    {

    }

    /// <summary>
    /// Method <c>addTube</c> adds an empty tube in the level.
    /// </summary>
    private void addTube()
    {
        
    }

    /// <summary>
    /// Method <c>deelteColor</c> removes one color entirely from the current level.
    /// </summary>
    private void deleteColor()
    {
        
    }


    // !!! Malus !!!

    /// <summary>
    /// Method <c>addTube</c> removes a tube from the level.
    /// </summary>
    private void removeTube()
    {
        
    }

    /// <summary>
    /// Method <c>timeAttack</c> set a timer that launch a game over when finished.
    /// The player has to finish the level in the given time.
    /// </summary>
    private void timeAttack()
    {

    }

    /// <summary>
    /// Method <c>addColorLayer</c> adds a color layer in the level.
    /// Instead of generating a new level, an already generated level from higher difficulty is taken.
    /// </summary>
    private void addColorLayer()
    {

    }

    /// <summary>
    /// Method <c>shadowLevel</c> set the next level in black background instead of light
    /// </summary>
    private void shadowLevel()
    {

    }


}
