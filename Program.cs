using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Raylib_CsLo;
namespace Orbital
{
    public class SpaceObject
    {

        private float mass_;
        private Vector2 position_;
        private Vector2 velocity_;
        private Vector2 acceleration_;
        public SpaceObject(float mass)
        {
            mass_ = mass;
        }
        public SpaceObject(float mass, Vector2 position)
        {
            mass_ = mass;
            position_ = position;
        }
        public float mass
        {
            get => mass_;
            set => mass_ = value;
        }
        public Vector2 position
        {
            get => position_;
            set => position_ = value;
        }
        public Vector2 velocity
        {
            get => velocity_;
            set => velocity_ = value;
        }
        public Vector2 acceleration
        {
            get => acceleration_;
            set => acceleration_ = value;
        }
        public void ApplyForce(Vector2 force)
        {
            acceleration = force / mass;
        }
        public void TickPhysics()
        {
            //v = at
            velocity += acceleration * Raylib.GetFrameTime();
            //d = vt
            position += velocity;
            //Console.WriteLine("Velocity: " + velocity + " Position: " + position);
        }
    }
    public static class Program
    {
        public static void Main(string[] args)
        {
            int windowWidth = 1280;
            int windowHeight = 720;
            float simTime = 0;

            Camera2D camera;
            camera.target = Vector2.Zero;
            camera.offset = new Vector2(windowWidth / 2.0f, windowHeight / 2.0f);
            camera.rotation = 0.0f;
            camera.zoom = 0.000005f;
            bool drawDebug = false;

            float G = 6.6743f * MathF.Pow(10, -11);

            Raylib.InitWindow(windowWidth, windowHeight, "ORBTL");
            Raylib.SetTargetFPS(500);

            SpaceObject earth = new(5.972f * MathF.Pow(10, 4));
            SpaceObject moon = new(7.34767309f * MathF.Pow(10, 2), new Vector2(384400000f, 0));
            moon.velocity = Vector2.UnitY * -300000f;

            SpaceObject satellite = new(20, new Vector2(12756000 * 1.5f, 0));
            satellite.velocity = Vector2.UnitY * -85000f;

            SpaceObject satellite2 = new(20, new Vector2(-12756000 * 1.2f, 0));
            satellite2.velocity = Vector2.UnitY * 67000f;
            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(255, 255, 242, 255));
                Raylib.BeginMode2D(camera);
                // F = G(m1*m2/r^2)
                if (Vector2.Distance(earth.position, moon.position) != 0)
                {
                    float force = G * (earth.mass * moon.mass / MathF.Pow(Vector2.Distance(earth.position, moon.position), 2));
                    force *= MathF.Pow(10, 19);
                    //Console.WriteLine(Vector2.Distance(earth.position, moon.position));
                    // F = ma
                    // a = F/m
                    //earth.ApplyForce(force * (moon.position - earth.position));
                    moon.ApplyForce(force * (earth.position - moon.position));
                    satellite.ApplyForce(force * (earth.position - satellite.position));
                    satellite2.ApplyForce(force * (earth.position - satellite2.position));
                }
                earth.TickPhysics();
                moon.TickPhysics();
                satellite.TickPhysics();
                satellite2.TickPhysics();
                Raylib.DrawCircleV(earth.position, 12756000f, Raylib.BLUE);
                Raylib.DrawCircleV(moon.position, 3474800f, Raylib.GRAY);
                Raylib.DrawCircleV(satellite.position, 700000f, Raylib.GOLD);
                Raylib.DrawCircleV(satellite2.position, 700000f, Raylib.GOLD);
                Raylib.EndMode2D();
                Raylib.DrawFPS(10, 10);
                Raylib.EndDrawing();
                camera.zoom += (float)Raylib.GetMouseWheelMove() * 0.0000002f;
                camera.zoom = Math.Clamp(camera.zoom, 0.0000001f, 10f);
                if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    camera.target -= Raylib.GetMouseDelta() / camera.zoom;
                }
                simTime += (int)(Raylib.GetFrameTime() * 1000);
            }
            Raylib.CloseWindow();
        }
    }
}