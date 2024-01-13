// Animals: HingeBeasts
// by Mike Evans 7/1/2024
// C# port of my processing code of 17/2/2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Collision;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System.Numerics;
using System.Windows.Threading;

namespace HingeBeasts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public float DAMPING = 0.0f;
        public float RESTITUTION = 0.6f;
        public float FRICTION = 0.9f;
        public float GRAVITY = 9.5f;
        public int NAnimals = 14;
        private float scaleFactor = 12f;
        private float angleCorrection = 0.5f;
        private Vector2 offset;
        private DispatcherTimer gameLoopTimer;
        private World myWorld;

        public MainWindow()
        {
            InitializeComponent();
            gameLoopTimer = new DispatcherTimer();
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameLoopTimer.Tick += GameLoop;  // Here += acts as an event subscription operator, adding a function to the list of event handlers.
            SetUp();
            gameLoopTimer.Start();
        }
        private void SetUp()
        {
            BodyDef def;
            Body aBody;
            Vector2[] vertices;
            //List<Animal> creach;

            offset = new Vector2((float)(0.5*myCanvas.Width / scaleFactor),1f);

            // Create a world with gravity
            Vector2 gravity = new Vector2(0, -GRAVITY);
            myWorld = new World(gravity);

            //Create 4 edges for the world:
            float leftX = -0.5f * (float)myCanvas.Width / scaleFactor;
            float rightX = 0.5f * (float)myCanvas.Width / scaleFactor;
            float bottomY = 0;
            float topY = 0.5f * (float)myCanvas.Height / scaleFactor;
            float centreX = 0.5f * (leftX + rightX);
            float centreY = 0.5f * (topY+bottomY);
            // Ground:
            def = new BodyDef();
            def.Position.Set(centreX, bottomY-0.5f);
            aBody = myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[0] = new Vector2(-centreX + rightX +1, 0.5f);
            vertices[1] = new Vector2(-centreX + leftX -1, 0.5f);
            vertices[2] = new Vector2(-centreX + leftX -1, -0.5f);
            vertices[3] = new Vector2(-centreX + rightX +1, -0.5f);
            affixPolygon(aBody, vertices, 0, Brushes.Green);
            //Left wall:
            def = new BodyDef();
            def.Position.Set(leftX-0.5f, centreY);
            aBody = myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[0] = new Vector2(0.5f, -centreY + bottomY);
            vertices[1] = new Vector2(0.5f, -centreY + topY * 2);
            vertices[2] = new Vector2(-0.5f, -centreY + topY * 2);
            vertices[3] = new Vector2(-0.5f, -centreY + bottomY);
            affixPolygon(aBody, vertices, 0, Brushes.Green);
            Console.WriteLine("Angle = " + aBody.GetAngle() + " = "+ (57.29577951 * aBody.GetAngle()) +" degrees");
            // Right wall:
            def = new BodyDef();
            def.Position.Set(rightX+0.5f, centreY);
            aBody = myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[0] = new Vector2(-0.5f, -centreY+topY * 2);
            vertices[1] = new Vector2(-0.5f, -centreY + bottomY);
            vertices[2] = new Vector2(0.5f, -centreY + bottomY);
            vertices[3] = new Vector2(0.5f, -centreY + topY * 2);
            affixPolygon(aBody, vertices, 0, Brushes.Green);
            // Ceiling:
            def = new BodyDef();
            def.Position.Set(centreX, 2*topY+0.5f);
            aBody = myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[1] = new Vector2(-centreX+leftX - 1, -0.5f);
            vertices[0] = new Vector2(-centreX+rightX + 1, -0.5f);
            vertices[3] = new Vector2(-centreX+rightX + 1, 0.5f);
            vertices[2] = new Vector2(-centreX+leftX - 1, 0.5f);
            affixPolygon(aBody, vertices, 0, Brushes.Green);

            // Create a dynamic body and set its position
            def = new BodyDef();
            def.BodyType = BodyType.DynamicBody;
            def.Position.Set(0, 10);
            aBody = myWorld.CreateBody(def);
            // Affix a shape to the body
            vertices = new Vector2[4];
            vertices[0] = new Vector2(-1f, -1f);
            vertices[1] = new Vector2(1f, -1f);
            vertices[2] = new Vector2(1f, 1f);
            vertices[3] = new Vector2(-1f, 1f);
            affixPolygon(aBody, vertices, 1, Brushes.Red);
            // Set its velocity
            aBody.SetLinearVelocity(new Vector2(7.2f, 23f));
            aBody.SetAngularVelocity(-17.5f);

            // Create another dynamic body and set its position
            def = new BodyDef();
            def.BodyType = BodyType.DynamicBody;
            def.Position.Set(12, 35);
            aBody = myWorld.CreateBody(def);
            // Affix a shape to the body
            vertices = new Vector2[5];
            vertices[0] = new Vector2(-1f, -1f);
            vertices[1] = new Vector2(1f, -1f);
            vertices[2] = new Vector2(1.4f, 0.9f);
            vertices[3] = new Vector2(0.0f, 1.8f);
            vertices[4] = new Vector2(-1.4f, 0.9f);
            affixPolygon(aBody, vertices, 1, Brushes.Blue);
            // Affix another shape to the same body
            vertices = new Vector2[3];
            vertices[0] = new Vector2(0.5f, 0f);
            vertices[1] = new Vector2(-2f, 2.5f);
            vertices[2] = new Vector2(-1.9f, 1.1f);
            affixPolygon(aBody, vertices, 1, Brushes.AliceBlue);
            //Set its velocity
            aBody.SetAngularVelocity(0.5f);

            // Create another dynamic body and set its position
            def = new BodyDef();
            def.BodyType = BodyType.DynamicBody;
            def.Position.Set(18, 35);
            aBody = myWorld.CreateBody(def);
            // Affix a shape to the body
            affixCircle(aBody, 1.5f, 1, Brushes.BurlyWood);
            //Set its velocity
            aBody.SetAngularVelocity(-15f);
        }
        private void affixPolygon(Body bod, Vector2[] vertices, float density, SolidColorBrush colour)
        {
            PolygonShape myBox2DPolygon = new PolygonShape();
            myBox2DPolygon.Set(vertices);
            Fixture fix = bod.CreateFixture(myBox2DPolygon, density);
            fix.Friction = FRICTION;
            fix.Restitution = RESTITUTION;
            fix.UserData = new MyShapeInfo(new Polygon { Fill = colour, Points = Vector2ListToPointCollection(vertices) });
        }

        private void affixCircle(Body bod, float radius, float density, SolidColorBrush colour)
        {
            CircleShape myBox2DCircle = new CircleShape();
            myBox2DCircle.Radius = radius;
            Fixture fix = bod.CreateFixture(myBox2DCircle, density);
            fix.Friction = FRICTION;
            fix.Restitution = RESTITUTION;
            fix.UserData = new MyShapeInfo(new Ellipse { Fill = colour, Width = 2 * radius, Height = 2 * radius });
        }

        public PointCollection Vector2ListToPointCollection(Vector2[] v)
        {
            PointCollection output = new PointCollection();
            for (int i = 0; i < v.Length; i++)
            {
                output.Add(new Point(v[i].X, v[i].Y));
            }
            return output;
        }
        private void GameLoop(object sender, EventArgs e)
        {
            UpdateGame();
            renderGame();
        }

        private void UpdateGame()
        {
            myWorld.Step(1 / 60f, 6, 2);
        }

        private void renderGame()
        {
            myCanvas.Children.Clear();
            foreach (Body body in myWorld.BodyList)
            {
                //Console.WriteLine(body.GetPosition());
                renderBody(body);
            }
        }

        private void renderBody(Body bod)
        {
            System.Windows.Shapes.Shape rendering;
            MyShapeInfo theShapeInfo;

            foreach (Fixture currentFixture in bod.FixtureList)
            {
                if (!(currentFixture.UserData == null))
                {
                    theShapeInfo = currentFixture.UserData as MyShapeInfo;
                    rendering = theShapeInfo.theShape;
                    rendering.StrokeThickness = 0.1;
                    rendering.Stroke = Brushes.Black;
                    TransformGroup myTransformation = new TransformGroup();
                    if (currentFixture.Shape is CircleShape) myTransformation.Children.Add(new TranslateTransform(-0.5 * rendering.Width, -0.5 * rendering.Height));
                    myTransformation.Children.Add(new RotateTransform(57.29577951 * bod.GetAngle()));
                    myTransformation.Children.Add(new TranslateTransform(bod.GetPosition().X+offset.X, bod.GetPosition().Y+offset.Y));
                    myTransformation.Children.Add(new ScaleTransform(scaleFactor, -scaleFactor));
                    myTransformation.Children.Add(new TranslateTransform(0.0, myCanvas.Height));
                    myTransformation.Children.Add(new RotateTransform(angleCorrection));
                    rendering.RenderTransform = myTransformation;
                    myCanvas.Children.Add(rendering);
                }
            }
        }
    }
}
