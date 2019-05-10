using System;
using System.Collections.Generic;
using System.Text;

namespace RenamerConsole.Menus {
    public class GradientOptions {
        public int rStart { get; set; }
        public int gStart { get; set; }
        public int bStart { get; set; }
        public int rOffsetPerLoop { get; set; }
        public int gOffsetPerLoop { get; set; }
        public int bOffsetPerLoop { get; set; }
        public int rEnd { get; set; }
        public int gEnd { get; set; }
        public int bEnd { get; set; }
        public bool pauseBetweenLetters { get; set; }
        public int pauseDelayInMilliseconds { get; set; }

    }
}
