using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RaceManager
{
    public enum RaceType
    {
        Easy, // 500m
        Normal, // 1000m
        Hard // 1500m
    }

    public RaceManager(RaceType type)
    {
        this.type = type;
    }

    public readonly RaceType type;



}