using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// ============================================================================
// MIDTERM PROJECT: THE ARENA TOURNAMENT
// ============================================================================
// WHAT YOU ARE BUILDING
//   Eight fighters enter one arena and brawl until a single champion remains.
//   The whole tournament plays out on its own inside Start() and is reported
//   in the Console Window using Debug.Log. Attach this script to an empty 
//   GameObject in your Scene and press Play; the entire fight should appear
//   in the Console.
//
// THE FIGHTERS' DATA
//   Each fighter is described by six stats. The stats are in six arrays
//   (one array per stat), all the same length, lined up by index. The stats
//   for fighter at index i would be names[i], health[i], armorClass[i], etc.
//   The stats are:
//     Name - the fighter's name
//     Health - hit points; at 0 or below the fighter is out
//     ArmorClass - how hard the fighter is to hit
//     AttackProficiency - the bonus added to this fighter's attack roll
//     AttackStrength - how much damage this fighter deals on a hit
//     AttackFrequency - how often this fighter attacks (see rule 2)
//
// HOW THE TOURNAMENT WORKS
//   1. Every fighter is in the arena at once, and the tournament runs in
//      numbered turns until only one fighter is left alive.
//   2. On each turn, a fighter acts only when the turn number matches its
//      AttackFrequency: 1 means it acts every turn, 2 means every second turn
//      (turns 2, 4, 6, ...), 3 means every third turn (turns 3, 6, 9, ...).
//      Use the turn number and the % (remainder) operator to decide.
//   3. When a fighter acts, it attacks ONE other living fighter chosen at
//      random. A fighter never attacks itself.
//   4. To resolve an attack, roll a 20-sided die (a whole number from 1 to 20)
//      and add the attacker's AttackProficiency. If that total is greater than
//      or equal to the target's ArmorClass, the attack HITS; otherwise it
//      MISSES.
//   5. Rolling a natural 20 (the die shows 20 before anything is added) is a
//      CRITICAL HIT. A critical hit always lands and deals DOUBLE the
//      attacker's AttackStrength in damage.
//   6. A normal hit deals damage equal to the attacker's AttackStrength.
//      Subtract the damage from the target's Health.
//   7. A fighter whose Health reaches 0 or below is out of the tournament and
//      can no longer act or be attacked.
//   8. Keep taking turns until exactly one fighter is still alive. That
//      fighter is the champion.
//
// WHAT TO PRINT (Debug.Log)
//   For every attack, make the Console clearly show:
//     who is attacking whom,
//     whether the attack HIT or MISSED,
//     whether it was a CRITICAL HIT,
//     and how much damage was dealt.
//   Announce when a fighter is defeated, and announce the champion at the end.
//
// CONCEPTS YOUR PROGRAM MUST USE
//   Variables and data types (int, string, bool)
//   Arithmetic operators (+ and *) and the % operator
//   Logical operators (&& and ||)
//   if / else if / else
//   Loops (for, while, and foreach)
//   Arrays (and foreach over an array)
//   Methods, including at least one method that returns a value and one pair
//   of overloaded methods
//
// Method stubs are provided below Start to show the pieces your program needs.
// Fill in each one. You may add more helper methods if you want.
// ============================================================================
public class Midterm_ArenaTournament_OLEVY : MonoBehaviour
{
    // ----- THE ROSTER: one array per stat -----
    // The arrays below have data for your 8 fighters. 
    // Every array should have the same length and be lined up by index.
    // The comments say WHAT each array should hold and a sensible range of
    // values to pick from. The exact names and numbers are up to you.

    private string[] names = {"Kratos", "Lara Croft", "Sonic", "Zagreus", "Tracer", "Samus", "Madeline", "Issac"};             // 8 fighter names (text)
    private int[] health = { 38, 28, 22, 30, 22, 36, 30, 34 };         // starting hit points per fighter (range: 20 to 40)
    private int[] armorClass = { 14, 15, 19, 15, 18, 16, 17, 18 };     // how hard each fighter is to hit; an attack hits when d20 + proficiency reaches this (range: 10 to 20)
    private int[] attackProficiency = { 6, 8, 7, 6, 8, 6, 6, 8 };      // bonus added to a fighter's d20 attack roll (range: 2 to 9)
    private int[] attackStrength = { 8, 6, 4, 6, 4, 8, 4, 4 };         // damage dealt on a hit (range: 4 to 9)
    private int[] attackFrequency = { 2, 1, 1, 1, 1, 2, 1, 1 };        // how often a fighter attacks: 1 = every turn, 2 = every second turn, 3 = every third turn (range: 1 to 3)
    int critValue = 21;
    int critModifier = 2;
    int turn = 1;


    void Start()
    {
        do
        {
            Debug.Log("Turn " + turn);

            for (int attacker = 0; attacker < names.Length; attacker++)
            {
                int target = ChooseTarget(attacker);
                if (target == -1)
                {
                    break;
                }
                else
                {
                    if ((turn % attackFrequency[attacker]) == 0)
                    {
                        Attack(attacker, target);
                    }
                }
            }
            turn++;
        }
        while (CountAlive() > 1);
        AnnounceWinner();
        // TODO: Run the tournament.
        //   Keep taking turns while more than one fighter is alive. On each
        //   turn, let every living fighter act on its scheduled turns and
        //   attack a random living opponent. When one fighter remains,
        //   announce the winner.
    }

    /// <summary>
    /// Output every fighter's name and all of their stats to the console.
    /// Should be output like the following
    /// Name: Kratos
    /// Health: 38
    /// Armor Class: 14
    /// Attack Proficiency: 6
    /// Attack Strength: 8
    /// Attack Frequency: 2
    /// </summary>
    void PrintRoster()
    {
        for(int index = 0; index < names.Length; index++)
        {
            Debug.Log("Name: " + names[index]);
            Debug.Log("Health: " + health[index]);
            Debug.Log("Armor Class: " + armorClass[index]);
            Debug.Log("Attack Proficiency: " + attackProficiency[index]);
            Debug.Log("Attack Strength: " + attackStrength[index]);
            Debug.Log("Attack Frequency: " + attackFrequency[index]);
        }
    }

    /// <summary>
    /// Returns a single roll of a 20-sided die (a whole number from 1 to 20).
    /// </summary>
    /// <returns></returns>
    int RollD20()
    {
        int diceRoll = Random.Range(1, critValue);
        return diceRoll;
    }

    /// <summary>
    /// Returns the damage for an attack (does not account for critical hits)
    /// </summary>
    /// <param name="attackerIndex">Integer index of the attacking fighter</param>
    /// <returns>Total damage of the attack</returns>
    int CalculateDamage(int attackerIndex)
    {
        int damage = attackStrength[attackerIndex];
        return damage;
    }

    /// <summary>
    /// Returns the damage for an attack and handles the potential for a critical hit
    /// </summary>
    /// <param name="attackerIndex">Integer index of the attacking fighter</param>
    /// <param name="isCritical">Boolean indicating if the attack was a critical hit</param>
    /// <returns>Total damage of the attack</returns>
    int CalculateDamage(int attackerIndex, bool isCritical)
    {
        int damage = attackStrength[attackerIndex];
        if (isCritical) 
        {
            damage = (attackStrength[attackerIndex] * critModifier);
        }
        return damage;
    }

    /// <summary>
    /// Returns the number of alive fighters
    /// </summary>
    /// <returns>integer number of living fighters</returns>
    int CountAlive()
    {
        int aliveCount = 0;
        foreach (int healthValue in health)
        {
            if(healthValue > 0)
            {
                aliveCount++;
            }
        }
        return aliveCount;
    }

    /// <summary>
    /// Finds the index of a random living opponent of the attacker, or 
    /// returns -1 if there are no living opponents remaining
    /// </summary>
    /// <param name="attackerIndex">Integer index of the attacking fighter</param>
    /// <returns>a randomly chosen target index to attack</returns>
    int ChooseTarget(int attackerIndex)
    {
        int target = attackerIndex;
        if (CountAlive() == 1)
        {
            target = -1;
        }
        else
        {
            while (target == attackerIndex)
            {
                target = Random.Range(0, health.Length);
                if (health[target] <= 0)
                {
                    target = attackerIndex;
                }
            }
        }
        return target;
    }

    // TODO: 
    //       Roll to hit, check for a hit/miss and a critical, apply any damage
    //       to the target's health, and Debug.Log what happened.
    /// <summary>
    /// Carries out one attack from the fighter at the attackerIndex against the 
    /// fighter at the targetIndex. Rolls to hit, checks for a hit/miss and, if
    /// it hits, checks for a critical, then apply damage to the fighter at the
    /// targetIndex's health. Output what happens to the console window.
    /// 
    /// E.g. Output for a Normal Hit
    /// Kratos hits Sonic for 8 damage.
    /// 
    /// E.g. Output for a Miss:
    /// Samus misses Issac.
    /// 
    /// E.g. Output for a Critical Hit
    /// Lara Croft lands a Critical Hit on Sonic for 12 damage.
    /// </summary>
    /// <param name="attackerIndex">Integer index of the attacking fighter</param>
    /// <param name="targetIndex">Integer index of the targeted fighter</param>
    void Attack(int attackerIndex, int targetIndex)
    {
        int roll = 0;
        bool crit = false;
        int damage = 0;
        roll = RollD20();
        if(roll == 20)
        {
            crit = true;
        }
        damage = CalculateDamage(attackerIndex, crit);
        if (roll + attackProficiency[attackerIndex] >= armorClass[targetIndex])
        {
            health[targetIndex] -= damage;
            if (crit)
            {
                Debug.Log(names[attackerIndex] + " lands a critical hit on " + names[targetIndex] + " for " + damage + ".");
            }
            else
            {
                Debug.Log(names[attackerIndex] + " hits " + names[targetIndex] + " for " + damage + ".");
            }
            if (health[targetIndex] <= 0)
            {
                Debug.Log(names[targetIndex] + " is defeated!");
            }
        }
        else
        {
            Debug.Log(names[attackerIndex] + " misses " + names[targetIndex] + ".");
        }
    }

    /// <summary>
    /// Finds the surviving fighter and outputs their name to the console window
    /// </summary>
    void AnnounceWinner()
    {
        for(int index = 0; index<health.Length; index++)
        {
            if (health[index] > 0)
            {
                Debug.Log("The winner is " + names[index] + "!");
                break;
            }
        }
    }
}
