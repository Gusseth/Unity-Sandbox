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
}

[Serializable]
public class Easy : Difficulty
{
    const string name = "Easy";
    public override int ParryTime => 1000;
    public override string Name => name;
}

[Serializable]
public class Normal : Difficulty
{
    const string name = "Normal";
    public override int ParryTime => 450;
    public override string Name => name;
}

[Serializable]
public class Hard : Difficulty
{
    const string name = "Hard";
    public override int ParryTime => 450;
    public override string Name => name;
}

[Serializable]
public class Lunatic : Difficulty
{
    const string name = "Lunatic";
    public override int ParryTime => 450;
    public override string Name => name;
}