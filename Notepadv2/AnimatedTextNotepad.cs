using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepadv2 {
    public class NotepadValues {
        public int x;
        public int y;

        public int width = 0;
        public int height = 0;

        public string title = "";
        public string text = "";

        public NotepadValues() { }
        public NotepadValues(NotepadActor actor) {
            this.x = actor.PositionX;
            this.y = actor.PositionY;
            this.width = actor.Width;
            this.height = actor.Height;
            this.title = actor.Title;
            this.text = actor.Text;
        }
    }

    public class AnimationFrame {
        public bool changePosition = false;
        public bool changeWindowSize = false;
        public bool changeTitle = false;
        public bool changeText = false;

        public NotepadValues values;

        public float lifetime;

        public Action? func = null;

        public AnimationFrame(float lifetime = 0) {
            this.lifetime = lifetime;
            this.values = new NotepadValues();
        }
    }

    public class Animation {

        private List<AnimationFrame> _frames = new List<AnimationFrame>();
        private int _currentFrame = 0;

        private Action? _completeCallback = null;
        private NotepadActor _target;

        private bool animationComplete = false;

        public Animation(NotepadActor target) {
            _target = target;
        }

        public void SetTitle(string title) {
            AnimationFrame frame = new AnimationFrame();
            frame.values.title = title;
            _frames.Add(frame);
        }

        public void TypeTitle(string title, float speed = 0.05f) {
            AnimationFrame frame = new AnimationFrame(title.Length * speed);
            frame.values.title = title;
            frame.changeTitle = true;
            _frames.Add(frame);
        }

        public void SetText(string text) {
            AnimationFrame frame = new AnimationFrame();
            frame.values.text = text;
            _frames.Add(frame);
        }

        public void TypeText(string text, float speed = 0.05f) {
            AnimationFrame frame = new AnimationFrame(text.Length * speed);
            frame.values.text = text;
            frame.changeText = true;
            _frames.Add(frame);
        }

        public void AddDelay(float time) {
            AnimationFrame frame = new AnimationFrame(time); ;
            _frames.Add(frame);
        }

        public void AddFunction(Action func) {
            AnimationFrame frame = new AnimationFrame();
            frame.func = func;
            _frames.Add(frame);
        }

        public void AnimateSize(int width, int height, float time = 1f) {
            AnimationFrame frame = new AnimationFrame(time);
            frame.values.width = width;
            frame.values.height = height;
            frame.changeWindowSize = true;
            _frames.Add(frame);
        }

        public void AnimatePosition(int x, int y, float time = 1f) {
            AnimationFrame frame = new AnimationFrame(time);
            frame.values.x = x;
            frame.values.y = y;
            frame.changePosition = true;
            _frames.Add(frame);
        }

        NotepadValues previousFrameValues;
        float time = 0;
        public void Play() {
            if (_target == null) return;

            previousFrameValues = new NotepadValues(_target);
            animationComplete = false;
            time = 0;
            Timer timer = new Timer(Update, null, 16, 16);
        }

        void Update(object? state) {
            if (animationComplete) {
                return;
            }

            time += 0.016f;
            AnimationFrame current = _frames[_currentFrame];

            float scaledT = Math.Clamp(time, 0, current.lifetime) / current.lifetime;
            if (current.changeText) {
                string frameText = current.values.text;
                _target.SetText(frameText.Substring(0, (int)(scaledT * frameText.Length)));
            }

            if (current.changeTitle) {
                string frameText = current.values.title;
                _target.SetWindowTitle(frameText.Substring(0, (int)(scaledT * frameText.Length)));
            }

            if (current.changeWindowSize) {
                float dx = (current.values.width - previousFrameValues.width) / (current.lifetime / 0.016f);
                float dy = (current.values.height - previousFrameValues.height) / (current.lifetime / 0.016f);
                _target.SetSize(_target.Width + (int)Math.Ceiling(dx), _target.Height + (int)Math.Ceiling(dy));
            }

            if (current.func != null) {
                current.func?.Invoke();
            }

            if (time >= _frames[_currentFrame].lifetime) {
                time = 0;
                _currentFrame++;
                previousFrameValues = new NotepadValues(_target);

                if (_currentFrame >= _frames.Count) {
                    animationComplete = true;
                    return;
                }
            }
        }

        public void OnComplete(Action? callback) {
            this._completeCallback = callback;
        }
    }

    public class AnimatedTextNotepad : NotepadActor {

        public AnimatedTextNotepad() {
            SetPosition(200, 100);
            SetSize(400, 200);
            SetFont(32);
            SetWindowTitle("NOTEPAD");

            Animation a = new Animation(this);
            a.TypeText("HI :)");
            a.AddDelay(1f);
            a.SetText("");
            a.AnimateSize(700, 250);
            a.TypeText("WHAT...\nYES THIS IS NOTEPAD");
            a.AddDelay(1f);
            a.TypeText("THE CLOSE BUTTON?");
            a.AddDelay(1f);
            a.TypeText("YOU WONT BE NEEDING THAT");
            a.AddDelay(0.5f);
            a.TypeText("LET ME JUST...");
            a.AddDelay(0.5f);
            a.AddFunction(() => { this.SetStyle(WindowStyle.WM_SYSMENU); });
            a.TypeText("THAT'S BETTER");
            a.AddDelay(1f);
            a.TypeText("...");
            a.TypeTitle("EVIL NOTEPAD >:)", 0.2f);
            a.TypeText(">:)", 0.5f);

            a.Play();
        }
    }
}
