using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class AnimationFrame {
        public bool changePosition = false;
        public int x;
        public int y;

        public bool changeWindowSize = false;
        public int width = 0;
        public int height = 0;

        public bool changeTitle = false;
        public string title = "";

        public bool changeText = false;
        public string text = "";

        public float lifetime;

        public AnimationFrame(float lifetime = 0) {
            this.lifetime = lifetime;
        }
    }

    public class Animation {

        private List<AnimationFrame> _frames = new List<AnimationFrame>();
        private int _currentFrame = 0;

        private Action? _completeCallback = null;
        private NotepadActor _target;

        public Animation(NotepadActor target) {
            _target = target;
        }

        public void SetTitle(string title) {
            AnimationFrame frame = new AnimationFrame();
            frame.title = title;
            _frames.Add(frame);
        }

        public void TypeTitle(string title, float speed = 0.05f) {
            AnimationFrame frame = new AnimationFrame(title.Length * speed);
            frame.title = title;
            frame.changeTitle = true;
            _frames.Add(frame);
        }

        public void TypeText(string text, float speed = 0.05f) {
            AnimationFrame frame = new AnimationFrame(text.Length * speed);
            frame.text = text;
            frame.changeText = true;
            _frames.Add(frame);
        }

        public void AddDelay(float time) {
            AnimationFrame frame = new AnimationFrame(time); ;
            _frames.Add(frame);
        }

        float time = 0;
        public void Play() {
            Timer timer = new Timer(Update, null, 16, 16);
        }
        void Update(object? state) {
            time += 0.016f;

            if (_frames[_currentFrame].changeText) {
                string frameText = _frames[_currentFrame].text;
                _target.SetText(frameText.Substring(0, (int)(Math.Clamp(time, 0, _frames[_currentFrame].lifetime) / _frames[_currentFrame].lifetime * frameText.Length)));
            }
        }

        public void OnComplete(Action? callback) {
            this._completeCallback = callback;
        }
    }

    public class AnimatedTextNotepad : NotepadActor {

        public AnimatedTextNotepad() {
            SetPosition(200, 100);

            Animation a = new Animation(this);
            a.TypeText("TESTING...");

            a.Play();
        }
    }
}
