using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICheckExclusives
{
    public void AddExcludable(object excludable);
    public void DeleteExcludable(object excludable);
    public bool ExcludableExists(object excludable);
}