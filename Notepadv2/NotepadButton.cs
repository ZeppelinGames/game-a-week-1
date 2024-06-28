using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class NotepadButton : NotepadActor {

        public NotepadButton() {
            SetText("CLICK ME");
            SetSize(300, 150);

            SetPosition(960, 540);
        }

        public override void HandleMouse(MouseEventData mouseData) {
            base.HandleMouse(mouseData);

            if (mouseData.X < this.PositionX + this.Width && mouseData.X > this.PositionX &&
                mouseData.Y < this.PositionY + this.Height && mouseData.Y > this.PositionY) {

                SetText("CLICKED!");
            }
        }
    }
}
