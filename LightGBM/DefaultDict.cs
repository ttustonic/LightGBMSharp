using System.Collections.Generic;

namespace LightGBM
{
    public class DefaultDict<TK, TV>: Dictionary<TK, TV> where TV : new()
    {
        public new TV this[TK key]
        {
            get
            {
                TV val;
                if (!TryGetValue(key, out val))
                {
                    val = new TV();
                    Add(key, val);
                }
                return val;
            }
            set { base[key] = value; }
        }
    }
}
