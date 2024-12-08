using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Enums;
using SaturnEngine.Engine.Structs;

using SDL_Renderer = nint;
using SDL_Texture = nint;

namespace SaturnEngine.Engine.Components
{
    public class MeshRenderer : Renderer3D
    {

        [Export] public RenderMode RenderMode { get; set; } = RenderMode.Solid;
        [Export] public Mesh Mesh { get; set; }
        [Export] public Texture Texture { get; set; }

        public override void OnUpdate(float deltaTime)
        {
            Transform.RotateX(-30.0f * deltaTime);
            Transform.RotateY(100.0f * -deltaTime);
            Transform.RotateZ(50.0f * deltaTime);
        }

        internal override void OnRender(Window window, Matrix4x4 matProj, Camera3D camera)
        {

            Matrix4x4 matCamera = Graphics.CalculateCameraMatrix(camera.Transform.Position, camera.Transform.Rotation);
            
            List<Triangle> triangles = Graphics.CalculateTriangles(matProj, matCamera, Transform.Position, Transform.Rotation, Mesh);

            if (RenderMode == RenderMode.Solid)
            {
                if(Texture != null)
                {
                    foreach (Triangle tri in triangles)
                    {
                        window.DrawTexturedTriangle(tri, Texture);
                    }
                }
                else
                {
                    foreach (Triangle tri in triangles)
                    {
                        window.DrawFilledTriangle(tri);
                    }
                }
            }
            else
            {
                Color color = new Color(0xFF, 0xAA, 0x00);
                foreach (Triangle tri in triangles)
                {
                    window.DrawTriangle(tri, color);
                }
            }
        }

    }
}