using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Media3D;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Matrix3DExUniversalSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const int MaxElementCount = 80;
        private const int NearPlane = 1;
        private const int FarPlane = 4000;

        private List<PhotoElement> _elements;
        private PhotoElement _selectedElement;
        private Random _rand;
        private double _f;
        private Point? _lastPointerPosition;

        public double TranslateX { get; set; }
        public double TranslateY { get; set; }
        public double TranslateZ { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double ScaleZ { get; set; }
        public double RotateX { get; set; }
        public double RotateY { get; set; }
        public double RotateZ { get; set; }
        public double CameraX { get; set; }
        public double CameraY { get; set; }
        public double CameraZ { get; set; }
        public double CameraLookAtX { get; set; }
        public double CameraLookAtY { get; set; }
        public double CameraLookAtZ { get; set; }
        public double FieldOfView { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }

        private void Init()
        {
            // Vars
            TranslateX = TranslateY = TranslateZ = 0;
            ScaleX = ScaleY = ScaleZ = 2;
            RotateX = RotateY = RotateZ = 0;
            CameraX = CameraY = 0;
            CameraZ = -4000;
            CameraLookAtX = CameraLookAtY = 0;
            CameraLookAtZ = 1;
            FieldOfView = 60;
            _rand = new Random();
            _elements = new List<PhotoElement>(MaxElementCount);

            AddItems(5);

            // Let's go
            DataContext = this;
            CompositionTarget.Rendering += (s, e) => Update();
        }

        private void AddItems(int groupCount = 1)
        {
            // Add random elements
            var w = Viewport.ActualWidth;
            var h = Viewport.ActualHeight;
            _elements.Clear();
            for (var j = 0; j < groupCount; j++)
            {

                for (var i = 0; i < MaxElementCount; i++)
                {
                    var element = new Image
                    {
                        //CacheMode   = new BitmapCache(),
                        Source = new BitmapImage(new Uri(BaseUri, String.Format("Pics/{0}.jpg", i))),
                        Stretch = Stretch.None,
                    };
                    element.PointerReleased += (s, e) => ElementPicked(s);

                    double w2 = w * 2;
                    double h2 = h * 2;
                    _elements.Add(new PhotoElement
                    {
                        Element = element,
                        PositionX = _rand.NextDouble() * w2 * 2 - w2,
                        PositionY = _rand.NextDouble() * h2 * 2 - h2,
                        PositionZ = -_rand.Next(NearPlane, FarPlane),
                    });
                }
            }

            // Add UIElements to Viewport canvas in the right z-order
            // XAML rendering seems not to respect the z-buffer
            Viewport.Children.Clear();
            var sortedUiElems = from e in _elements
                                orderby e.PositionZ descending
                                select e.Element;
            foreach (var element in sortedUiElems)
            {
                Viewport.Children.Add(element);
            }
        }

        private void Update()
        {
            // Animation
            if (ChkAnimated.IsChecked != null && ChkAnimated.IsChecked.Value)
            {
                CameraZ = -Math.Abs(Math.Sin(_f)) * (FarPlane - NearPlane) * 1.5;
                CameraY = Math.Sin(_f * 10) * 1000;
                _f += 0.008;
            }

            // Create global transformations
            var vw = Viewport.ActualWidth;
            var vh = Viewport.ActualHeight;
            var invertYAxis = Matrix3DFactory.CreateScale(1, -1, 1);
            var translate = Matrix3DFactory.CreateTranslation(TranslateX, TranslateY, TranslateZ);
            var rotateX = Matrix3DFactory.CreateRotationX(MathHelper.ToRadians(RotateX));
            var rotateY = Matrix3DFactory.CreateRotationY(MathHelper.ToRadians(RotateY));
            var rotateZ = Matrix3DFactory.CreateRotationZ(MathHelper.ToRadians(RotateZ));
            var scale = Matrix3DFactory.CreateScale(ScaleX, ScaleY, ScaleZ);
            var lookAt = Matrix3DFactory.CreateLookAtLH(CameraX, CameraY, CameraZ, CameraLookAtX, CameraLookAtY, CameraLookAtZ);
            var viewport = Matrix3DFactory.CreateViewportTransformation(vw, vh);
            Matrix3D projectionMatrix;
            projectionMatrix = ChkPerspective.IsChecked != null && ChkPerspective.IsChecked.Value ? Matrix3DFactory.CreatePerspectiveFieldOfViewLH(MathHelper.ToRadians(FieldOfView), vw / vh, NearPlane, FarPlane) : Matrix3DFactory.CreateOrthographicLH(vw, vh, NearPlane, FarPlane);

            // Transform all elements
            var selectedMatrix = Matrix3D.Identity;
            foreach (var elem in _elements)
            {
                // The UIElement
                var e = elem.Element;

                // Create basic transformation matrices
                var centerAtOrigin = Matrix3DFactory.CreateTranslation(-e.ActualWidth * 0.5, -e.ActualHeight * 0.5, 0);
                var baseTranslate = Matrix3DFactory.CreateTranslation(elem.PositionX, elem.PositionY, elem.PositionZ);

                // Combine the transformation matrices
                var m = Matrix3D.Identity;
                m = m * centerAtOrigin;
                m = m * invertYAxis;

                // Apply the world transformation to the selected element
                if (elem == _selectedElement)
                {
                    m = m * scale;
                    m = m * rotateX * rotateY * rotateZ;
                    m = m * translate;

                    // Should the camera target be fixed at the selected element?
                    if (ChkLookAtSelected.IsChecked != null && ChkLookAtSelected.IsChecked.Value)
                    {
                        lookAt = Matrix3DFactory.CreateLookAtLH(CameraX, CameraY, CameraZ, elem.PositionX, elem.PositionY, elem.PositionZ);
                    }
                }

                // Calculate the final view projection matrix
                m = m * baseTranslate;
                m = Matrix3DFactory.CreateViewportProjection(m, lookAt, projectionMatrix, viewport);

                if (elem == _selectedElement)
                {
                    selectedMatrix = m;
                }

                // Apply the transformation to the UIElement
                e.Projection = new Matrix3DProjection { ProjectionMatrix = m };
            }

            // Trace
            TxtTrace1.Text = String.Format("{0} Elements. Matrix:\r\n{1}", _elements.Count, selectedMatrix.Dump());
        }


        private void ElementPicked(object sender)
        {
            _selectedElement = _elements.FirstOrDefault(e => e.Element == sender);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Viewport_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _lastPointerPosition = e.GetCurrentPoint(Viewport).Position;
        }

        private void Viewport_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_lastPointerPosition.HasValue)
            {
                var position = e.GetCurrentPoint(Viewport).Position;
                var diffX = position.X - _lastPointerPosition.Value.X;
                var diffY = position.Y - _lastPointerPosition.Value.Y;
                if (e.KeyModifiers.HasFlag(VirtualKeyModifiers.Control) || e.KeyModifiers.HasFlag(VirtualKeyModifiers.Shift))
                {
                    CameraZ += diffY;
                }
                else
                {
                    CameraX -= diffX;
                    CameraY += diffY;
                }
                _lastPointerPosition = position;
            }
        }

        private void Viewport_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _lastPointerPosition = null;
        }

    }
}
