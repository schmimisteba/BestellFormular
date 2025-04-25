namespace BestellFormular.GUI
{
    /// <summary>
    /// A custom GraphicsView that supports binding and interactive drawing.
    /// </summary>
    public class BindableDrawingView : GraphicsView
    {
        /// <summary>
        /// Bindable property for the drawing path, allowing data binding to a PathF object.
        /// </summary>
        public static readonly BindableProperty DrawingPathProperty =
            BindableProperty.Create(
                nameof(DrawingPath),
                typeof(PathF),
                typeof(BindableDrawingView),
                new PathF(),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var view = (BindableDrawingView)bindable;
                    view.Invalidate(); // Redraw the view when the path changes
                });

        /// <summary>
        /// Gets or sets the drawing path.
        /// </summary>
        public PathF DrawingPath
        {
            get => (PathF)GetValue(DrawingPathProperty);
            set => SetValue(DrawingPathProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableDrawingView"/> class.
        /// Sets up the drawable and interaction event handlers.
        /// </summary>
        public BindableDrawingView()
        {
            Drawable = new DrawingDrawable(this);
            this.StartInteraction += OnStartInteraction;
            this.DragInteraction += OnDragInteraction;
        }

        /// <summary>
        /// Handles the start of a touch interaction, moving the drawing path to the touch point.
        /// </summary>
        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            var point = e.Touches[0];
            DrawingPath.MoveTo(point);
        }

        /// <summary>
        /// Handles the dragging interaction, drawing lines to the current touch point and invalidating the view.
        /// </summary>
        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            var point = e.Touches[0];
            DrawingPath.LineTo(point);
            Invalidate(); // Redraw the view to show the updated path
        }

        /// <summary>
        /// A drawable class that renders the drawing path on the canvas.
        /// </summary>
        private class DrawingDrawable : IDrawable
        {
            private readonly BindableDrawingView _view;

            /// <summary>
            /// Initializes a new instance of the <see cref="DrawingDrawable"/> class.
            /// </summary>
            /// <param name="view">The parent BindableDrawingView instance.</param>
            public DrawingDrawable(BindableDrawingView view)
            {
                _view = view;
            }

            /// <summary>
            /// Draws the current drawing path onto the provided canvas.
            /// </summary>
            /// <param name="canvas">The canvas to draw on.</param>
            /// <param name="dirtyRect">The area of the canvas that needs to be redrawn.</param>
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                try
                {
                    canvas.StrokeColor = Colors.Black;
                    canvas.StrokeSize = 2;
                    canvas.DrawPath(_view.DrawingPath);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
