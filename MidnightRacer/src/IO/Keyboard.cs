using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MidnightRacer.Engine
{
    static class Keyboard
    {
        public static Dictionary<Keys, bool> Pressed = new Dictionary<Keys, bool>();

        static Keyboard()
        {
            foreach (var entry in Enum.GetValues(typeof(Keys)))
            {
                try
                {
                    Pressed.Add((Keys)entry, false);
                }catch(Exception ex) { }
            }
        }
    }
}
