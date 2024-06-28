using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class BombEnemy : NotepadActor {
        public BombEnemy() {
            SetWindowTitle("BOMB!");
            SetText("O");
            SetFont(128);
            SetPosition(500, 500);
        }
    }
}
