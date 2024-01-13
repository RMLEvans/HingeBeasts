using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Numerics;
using Box2DSharp.Common;
using Box2DSharp.Collision;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using Box2DSharp.Dynamics.Joints;

namespace HingeBeasts
{
    class Animal
    {
        public List<Body> bodypart;
        public int NParts;
        public RevoluteJoint axle;
        private float targetMotorSpeed;

        public Animal(MainWindow MW, float xpos, float ypos)
        {
            BodyDef def;
            Vector2[] vertices;
            Body mouth, limb;
            targetMotorSpeed = 25f;
            float limbLength = 3.2f;
            float limbWidth = 0.46f;

            SolidColorBrush col = new SolidColorBrush(System.Windows.Media.Color.FromRgb(190, (Byte)MW.RNG.Next(5, 127), (Byte)MW.RNG.Next(50, 255)));
            bodypart = new List<Body>();

            def = new BodyDef();
            def.BodyType = BodyType.DynamicBody;
            def.Position.Set(xpos, ypos);
            mouth = MW.myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[0] = new Vector2(-limbLength / 2, -limbWidth / 2);
            vertices[1] = new Vector2(limbLength / 2, -limbWidth / 2);
            vertices[2] = new Vector2(limbLength / 2, limbWidth / 2);
            vertices[3] = new Vector2(-limbLength / 2, limbWidth / 2);
            MW.affixPolygon(mouth, vertices, 1, col);
            mouth.IsSleepingAllowed = false;
            bodypart.Add(mouth);
            NParts = 1;

            def = new BodyDef();
            def.BodyType = BodyType.DynamicBody;
            def.Position.Set(xpos, ypos + limbWidth);
            limb = MW.myWorld.CreateBody(def);
            vertices = new Vector2[4];
            vertices[0] = new Vector2(-limbLength / 2, -limbWidth / 2);
            vertices[1] = new Vector2(limbLength / 2, -limbWidth / 2);
            vertices[2] = new Vector2(limbLength / 2, limbWidth / 2);
            vertices[3] = new Vector2(-limbLength / 2, limbWidth / 2);
            MW.affixPolygon(limb, vertices, 1, col);
            bodypart.Add(limb);
            NParts++;

            Vector2 axleLocation = new Vector2(xpos + limbLength / 2, ypos + limbWidth / 2);
            RevoluteJointDef revDef = new RevoluteJointDef
            {
                BodyA = mouth,
                BodyB = limb,
                LocalAnchorA = mouth.GetLocalPoint(axleLocation),
                LocalAnchorB = limb.GetLocalPoint(axleLocation),
                EnableMotor = true,
                MaxMotorTorque = 150.0f,
                MotorSpeed = targetMotorSpeed,
                LowerAngle = -2.0f,
                UpperAngle = 0,
                ReferenceAngle = 0,
                EnableLimit = true
            };
            axle = MW.myWorld.CreateJoint(revDef) as RevoluteJoint;
        }

        public void control(Random RNG)
        {
            if ( RNG.NextDouble() < 0.03)
            {
                targetMotorSpeed = -targetMotorSpeed;
                axle.SetMotorSpeed(targetMotorSpeed);
            }
        }
    }
}
