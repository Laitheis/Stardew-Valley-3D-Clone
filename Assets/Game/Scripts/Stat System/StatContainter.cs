using System;
using System.Collections.Generic;
using System.Linq;

public class StatContainter
{
    private List<Stat> _container;

    public StatContainter()
    {
        _container = new List<Stat>();
    }

    public List<Stat> Container => _container;

    public Stat GetStat(StatTypes name)
    {
        var stat = _container.FirstOrDefault(item => item.Name == name);
        return stat;
    }

    public Stat Add(StatTypes name, int maxValue)
    {
        if (_container.Any(item => item.Name == name))
            return null;

        var stat = new Stat(name, maxValue);

        _container.Add(stat);

        return stat;
    }
}

