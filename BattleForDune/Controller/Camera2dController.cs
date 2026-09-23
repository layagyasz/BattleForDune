using Cardamom.Graphics.Camera;
using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BattleForDune.Controller
{
    public class Camera2dController : IElementController
    {
        public EventHandler<EventArgs>? Changed { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }

        public float KeySensitivity { get; set; } = 1f;
        public float MouseWheelSensitivity { get; set; } = 1f;
        public Interval DistanceRange { get; set; } = Interval.Unbounded;
        public Interval XRange { get; set; } = Interval.Unbounded;
        public Interval YRange { get; set; } = Interval.Unbounded;

        private readonly SubjectiveCamera3d _camera;

        public Camera2dController(SubjectiveCamera3d camera)
        {
            _camera = camera;
        }

        public void Bind(object @object) { }

        public void Unbind() { }

        public bool HandleKeyDown(IUiContext context, KeyDownEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Left:
                    ChangeFocus(_camera.Distance * new Vector3(-KeySensitivity * e.TimeDelta, 0, 0));
                    return true;
                case Keys.Right:
                    ChangeFocus(_camera.Distance * new Vector3(KeySensitivity * e.TimeDelta, 0, 0));
                    return true;
                case Keys.Up:
                    ChangeFocus(_camera.Distance * new Vector3(0, 0, -KeySensitivity * e.TimeDelta));
                    return true;
                case Keys.Down:
                    ChangeFocus(_camera.Distance * new Vector3(0, 0, KeySensitivity * e.TimeDelta));
                    return true;
            }
            return false;
        }

        public void ChangeFocus(Vector3 delta)
        {
            var newFocus = _camera.Focus + delta;
            newFocus.X = XRange.Clamp(newFocus.X);
            newFocus.Y = YRange.Clamp(newFocus.Y);
            _camera.SetFocus(newFocus);
        }

        public bool HandleTextEntered(IUiContext context, TextEnteredEventArgs e)
        {
            return false;
        }

        public bool HandleMouseEntered(IUiContext context)
        {
            return false;
        }

        public bool HandleMouseLeft(IUiContext context)
        {
            return false;
        }

        public bool HandleMouseButtonClicked(IUiContext context, MouseButtonClickEventArgs e)
        {
            return false;
        }

        public bool HandleMouseButtonDragged(IUiContext context, MouseButtonDragEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                ChangeFocus(
                    2 * _camera.Distance * new Vector3(-_camera.AspectRatio * e.NdcDelta.X, 0, e.NdcDelta.Y));
                return true;
            }
            MouseDragged?.Invoke(this, e);
            return false;
        }

        public bool HandleMouseWheelScrolled(IUiContext context, MouseWheelEventArgs e)
        {
            _camera.SetDistance(DistanceRange.Clamp(_camera.Distance - MouseWheelSensitivity * e.OffsetY));
            return true;
        }

        public bool HandleMouseLingered(IUiContext context)
        {
            return false;
        }

        public bool HandleMouseLingerBroken(IUiContext context)
        {
            return false;
        }

        public bool HandleFocusEntered(IUiContext context)
        {
            return false;
        }

        public bool HandleFocusLeft(IUiContext context)
        {
            return false;
        }
    }
}
