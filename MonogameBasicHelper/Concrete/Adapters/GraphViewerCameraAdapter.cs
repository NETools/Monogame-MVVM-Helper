using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameBasicHelper.Attributes;
using MonogameBasicHelper.MVVM;
using System;

namespace MonogameBasicHelper.Concrete.Adapters
{
    public class GraphViewerCameraAdapter : IAdapter
    {
        /// <summary>
        /// Braucht man allgemein oft, deshalb verwenden wir diese.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }
        /// <summary>
        /// Hier steht am Ende die Projektionsmatrix
        /// </summary>
        public Matrix ProjectionMatrix { get; private set; }

        /// <summary>
        /// Hier steht am Ende die Viewmatrix
        /// </summary>
        public Matrix ViewMatrix { get; private set; }

        /// <summary>
        /// Die aktuelle Kameraposition - wir brauchen das, weil die Kamera eigentlich immer bei (0, 0, 0) ist. Was passiert ist, dass die Objekte sich von der Kamera wegbewegen. Das machen wir so, weil dadurch Rundungsfehler vermieden werden.
        /// </summary>
        public Vector3 CameraPosition { get; private set; } = new Vector3(0, 0, 500);
        /// <summary>
        /// Der Vektor gibt die rotierte Kameraposition an.
        /// </summary>
        public Vector3 RotatedCameraPosition { get; private set; }

        /// <summary>
        /// Selbstsprechend
        /// </summary>
        public double MouseSensity { get; set; }
        /// <summary>
        /// Gibt an, wie schnell man mit der Maus Objekt hin -und her bewegen kann.
        /// </summary>
        public double TranslationVelocity { get; set; }

        private Vector3 _currentZoomDirection;

        /// <summary>
        /// Yaw gibt die momentane Differenz der X-Komponente zwischen Startpunk der Maus und Endpunkt der Maus in einem frame.
        /// </summary>
        private double _yaw;

        /// <summary>
        /// Yaw gibt die momentane Differenz der Y-Komponente zwischen Startpunk der Maus und Endpunkt der Maus in einem frame.
        /// </summary>
        private double _pitch;

        /// <summary>
        /// Gibt die X-Komponente des aktuellen Zentrums an
        /// </summary>
        private double _currentCenterX;

        /// <summary>
        /// Gibt die Y-Komponente des aktuellen Zentrums an. Wenn der User auf das Fenster klickt, ist das worauf er geklickt hat das Zentrum
        /// </summary>
        private double _currentCenterY;

        /// <summary>
        /// Allgemeines flag um sicherzustellen, dass der Nutzer .BeginTranslation aufgerufen hat.
        /// </summary>
        private bool _translationStarted;

        /// <summary>
        /// Allgemeines flag um sicherzustellen, dass der Nutzer .BeginRotation aufgerufen hat.
        /// </summary>
        private bool _rotationStarted;

        /// <summary>
        /// Gibt die letzte Rotation zurück, am Anfang ist das Identity, also gar keine Rotation
        /// </summary>
        private Matrix _lastRotation = Matrix.Identity;

        public Vector3 _zoomTarget;

        [OnBuiltUp]
        public void OnInitialize(GraphicsDevice graphicsDevice, double mouseSensity, double translationVelocity, float aspectRatio)
        {
            GraphicsDevice = graphicsDevice;

            MouseSensity = mouseSensity;
            TranslationVelocity = translationVelocity;

            RotatedCameraPosition = new Vector3(0, 0, 500); // Initialer Wert: Hier steht die Kamera
            CameraPosition = new Vector3(0, 0, 500);

            // Einfach Methode benutzen, für mehr Info siehe Projektionsmatrix Wikipedia.
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 10000.0f);

            // Konstruktion unserer View-Matrix: Jedes Mal wenn der Nutzer mit der Maus spielt, wird eine neue View-Matrix berechnet biidhnIllah
            ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
        }

        private void CalculateReferencePoint()
        {
            Vector3 nearPoint = new Vector3(Mouse.GetState().X, Mouse.GetState().Y, 0);
            Vector3 farPoint = new Vector3(Mouse.GetState().X, Mouse.GetState().Y, 1);

            nearPoint = GraphicsDevice.Viewport.Unproject(nearPoint, ProjectionMatrix, ViewMatrix, Matrix.CreateTranslation(RotatedCameraPosition));
            farPoint = GraphicsDevice.Viewport.Unproject(farPoint, ProjectionMatrix, ViewMatrix, Matrix.CreateTranslation(RotatedCameraPosition));

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            _zoomTarget = RotatedCameraPosition + direction * MathF.Abs(RotatedCameraPosition.Y / direction.Y);

        }

        /// <summary>
        /// Methode um den aktuellen Zentrum zu ermitteln: Wenn der Nutzer auf (200, 500) klickt (also irgendwo auf dem Fenster) dann ist das Zentrum 200, 500.
        /// </summary>
        private void FixCenters()
        {
            _currentCenterX = Mouse.GetState().X;
            _currentCenterY = Mouse.GetState().Y;
        }

        /// <summary>
        /// Die Methode MUSS aufgerufen werden: Hier wird VOR der Rotation das aktuelle Zentrum gesetzt.
        /// </summary>
        public void BeginRotation()
        {
            if (_rotationStarted)
                return;

            FixCenters();

            _rotationStarted = true;
        }

        public void Rotate(double elapsedMs)
        {
            if (!_rotationStarted)
                throw new Exception("You must begin rotation procedure first!");

            double dx = Mouse.GetState().X - _currentCenterX; // Wenn der Nutzer auf eine Position geklickt hat, und nun mit der Maus 10 Pixel
                                                              // nach rechts geht, dann ist dx = 10.
            double dy = Mouse.GetState().Y - _currentCenterY; // Selbes Prinzip

            _yaw += -dx * MouseSensity * elapsedMs; // Wenn der Nutzer jetzt die ganze Zeit nach rechts geht, steigt die Anzahl der Grade um die Y-Achse zu rotieren entsprechend
            _pitch += -dy * MouseSensity * elapsedMs; // Selbes Prinzip

            _pitch = MathHelper.Clamp((float)_pitch, -1.5f, 1.5f); // Der Pitch muss beschränkt werden, andernfalls Gefahr Gimbal-Lock, oder das die Vorzeichen sich ändern und die Steuerung plötzlich invertiert ist.

            _lastRotation = Matrix.CreateRotationX((float)_pitch) * Matrix.CreateRotationY((float)_yaw); // Hier wird die Rotationsmatrix
                                                                                                         // gebildet. Wenn der Nutzer bei (200,
                                                                                                         // 500) angefangen hat mit der Maus,
                                                                                                         // und am Ende bei (300, 500) ist, dann
                                                                                                         // wird 100 * MouseSensity um die Y-
                                                                                                         // Achse rotiert. Siehe Yaw-Pitch-Roll


            var rotatedReferene = Vector3.Transform(Vector3.Forward, _lastRotation); // Hier wird die Richtung in der die Kamera guckt (Standard: Nach vorne) mit der Rotationsmatrix transformiert: Am Ende guckt die Kamera also nicht mehr nach vorne, sondern zB um 80 Grad nach rechts

            ViewMatrix = Matrix.CreateLookAt(Vector3.Zero, rotatedReferene, Vector3.Up); // Hier wird die neue View-Matrix mit der neu berechneten Richtung berechnet biidhnIllah

            RotatedCameraPosition = Vector3.Transform(CameraPosition, _lastRotation); // Die Kamera-Position wird jetzt entsprechend der Rotationsmatrix transformiert. So wecken wir den Anschein als ob die Kamera sich um das Objekt dreht

            // Am Ende müssen die Centers wieder gefixt werden.
            FixCenters();
            CalculateReferencePoint();
            _currentZoomDirection = _zoomTarget - RotatedCameraPosition;
        }



        /// <summary>
        /// Beendet die Rotation
        /// </summary>
        public void EndRotation()
        {
            _rotationStarted = false;
        }

        /// <summary>
        /// Beginnt die Translation
        /// </summary>
        public void BeginTranslation()
        {
            if (_translationStarted)
                return;

            // Selbes Prinzip wie bei BeginRotation
            FixCenters();
            _translationStarted = true;
        }

        public void Translate(double elapsedMs)
        {
            if (!_translationStarted)
                throw new Exception("You must begin translation procedure first!");

            double dx = Mouse.GetState().X - _currentCenterX; // Selbes Prinzip wie bei Rotation
            double dy = Mouse.GetState().Y - _currentCenterY;

            CameraPosition +=
                Vector3.Up * (float)(dy * elapsedMs * TranslationVelocity) +
                Vector3.Right * (float)(-dx * elapsedMs * TranslationVelocity); // Hier wird statt Rotation, die Translation,
                                                                                // also die Bewegung berechnet
                                                                                // Wenn der Nutzer auf (200, 500) klickt, und bei (400, 500) endet, muss die Kamera also um 200 nach rechts verschoben werden.

            RotatedCameraPosition = Vector3.Transform(CameraPosition, _lastRotation);
            FixCenters();
            CalculateReferencePoint();
            _currentZoomDirection = _zoomTarget - RotatedCameraPosition;
        }

        public void EndTranslation()
        {
            _translationStarted = false;
        }

        /// <summary>
        /// Mit der Methode kann man sich dem Nullpunkt nähern/vom Nullpunkt sich entfernen, also quasi rein -und rauszoomen
        /// </summary>
        /// <param name="ds"></param>
        public void Scroll(float ds)
        {
            // Erst ermitteln wir den Richtungsvektor von der Kameraposition zum Nullpunkt
            var v = _currentZoomDirection;

            // Dann normieren wir den Vektor, um die reine Richtung des Vektors zu erhalten
            v.Normalize();

            // Wir berechnen den nächsten Punkt nach dem Scrollen: Das ist also nichts anderes als:
            // KameraPosition + Richtung * ZoomFactor (Geradengleichung)

            Vector3 current = RotatedCameraPosition + ds * v;


            //// Wenn der Abstand von der aktuellen Kameraposition zum Nullpunkt kleiner als 10 ist, wird die Position nicht aktualisiert:
            //// Es gibt also eine maximale Zoomstärke.
            //if ((current - _zoomTarget).Length() <= 50)
            //    return;

            RotatedCameraPosition = current;
            CameraPosition = Vector3.Transform(RotatedCameraPosition, Matrix.Invert(_lastRotation));


        }
    }
}
