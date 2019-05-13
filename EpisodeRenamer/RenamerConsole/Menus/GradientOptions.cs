using System;
using System.Collections.Generic;
using System.Text;

namespace RenamerConsole.Menus {
    public class GradientOptions {
        public int RStart { get; set; }
        public int GStart { get; set; }
        public int BStart { get; set; }
        public int REnd { get; set; }
        public int GEnd { get; set; }
        public int BEnd { get; set; }

        public string HexColorStart { get; set; }
        public string HexColorEnd { get; set; }


        public int intervals { get; set; }

        public bool pauseBetweenLetters { get; set; }
        public int pauseDelayInMilliseconds { get; set; }

    }
}
