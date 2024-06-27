using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class Player : NotepadActor {

        public Player() {
            SetSize(100, 200);
            SetWindowTitle("PLAYER");
            UpdateFont(100);
            SetText(":)");
        }
    }
}
