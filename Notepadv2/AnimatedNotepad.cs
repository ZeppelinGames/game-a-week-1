using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class AnimatedNotepad : NotepadActor {
        public string[] frames = new string[] {
            "o",
            "\no",
            "\n\no",
            "\n\n\no",
            "\n\n\n\no",
            "\n\n\n\n\no",
            "\n\n\n\no",
            "\n\n\no",
            "\n\no",
            "\no",
            "o",
        };

        private const float framerate = 8;
        private int currentFrame = 0;
        private float time = 0;

        public AnimatedNotepad() {
            SetSize(100, 525);
        }

        public override void Update(float dt) {
            base.Update(dt);

            time += dt;

            if (time > 1f / framerate) {
                time = 0;
                SetText(frames[currentFrame]);
                currentFrame++;
                if (currentFrame >= frames.Length) {
                    currentFrame = 0;
                }
            }
        }
    }
}
