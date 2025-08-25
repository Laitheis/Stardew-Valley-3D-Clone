using System;
using System.Collections.Generic;
using System.Linq;

public class StatContainter
{
    private List<Stat> _statContainer;

    public StatContainter(List<Stat> stats)
    {
        _statContainer = stats;
    }

    public List<Stat> Container => _statContainer;

    public Stat GetStat(StatTypes name)
    {
        var stat = _statContainer.FirstOrDefault(item => item.Name == name);
        return stat;
    }
}

