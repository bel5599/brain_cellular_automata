using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public abstract class Behavior
    {

    }

    public class TumorCellBehavior: Behavior
    {
        public TumorCellBehavior()
        {

        }
    }

    public class StemCellBehavior: Behavior
    {
        public StemCellBehavior()
        {

        }
    }
    public class TumorStemCellBehavior: Behavior
    {
        public TumorStemCellBehavior()
        {

        }
    }

    public class Astrocyte: Behavior
    {

    }
}
