using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class Difficulty
{
    public abstract string Name { get; }
    public abstract int ParryTime { get; }
    public abstract int KegareTimeout { get; }
    public abstract float GrazeRadius { get; }
    public abstract float ScoreMultiplier { get; }
    public virtual bool InstantDeath { get => false; }
    public virtual bool NoSaving { get => false; }
}

[Serializable]
public class Easy : Difficulty
{
    const string name = "Easy";
    public override int ParryTime => 1000;
    public override int KegareTimeout => 3000;
    public override string Name => name;
    public override float GrazeRadius => 3.0f;
    public override float ScoreMultiplier => 0.5f;
}

[Serializable]
public class Normal : Difficulty
{
    const string name = "Normal";
    public override int ParryTime => 450;
    public override int KegareTimeout => 5000;
    public override string Name => name;
    public override float GrazeRadius => 2.0f;
    public override float ScoreMultiplier => 1;
}

[Serializable]
public class Hard : Difficulty
{
    const string name = "Hard";
    public override int ParryTime => 450;
    public override int KegareTimeout => 5000;
    public override string Name => name;
    public override float GrazeRadius => 2.0f;
    public override float ScoreMultiplier => 2;
}

[Serializable]
public class Lunatic : Difficulty
{
    const string name = "Lunatic";
    public override int ParryTime => 450;
    public override int KegareTimeout => 5000;
    public override string Name => name;
    public override float GrazeRadius => 1.75f;
    public override float ScoreMultiplier => 5;
}

/// <summary>
/// The ultimate challenge. Unlocked after completing the game in Lunatic difficulty.
/// </summary>
[Serializable]
public class LunaticOneCC : Lunatic
{
    const string name = "Lunatic 1cc";
    public override float ScoreMultiplier => 20;
    public override string Name => name;
    public override bool InstantDeath => true;
    public override bool NoSaving => true;
}