using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Enums;
using SaturnEngine.Engine.Structs;

using SDL_Renderer = nint;
using SDL_Texture = nint;

namespace SaturnEngine.Engine.Components
{
    public class MeshRenderer : Renderer3D
    {

        [Export] public RenderMode3D RenderMode { get; set; } = RenderMode3D.Solid;
        [Export] public Mesh Mesh { get; set; }
        [Export] public Texture Texture { get { return _texture; } set { _texture = value; RenderMode = RenderMode3D.Textured; } }

        private Texture _texture;

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

            switch (RenderMode)
            {
                case RenderMode3D.Wireframe:
                    foreach (Triangle tri in triangles)
                    {
                        window.DrawTriangle(tri, Color.Orange);
                    }
                    break;
                case RenderMode3D.WireframeTextured:
                    if (Texture != null)
                    {
                        foreach (Triangle tri in triangles)
                        {
                            window.DrawTexturedTriangle(tri, Texture);
                        }
                        foreach (Triangle tri in triangles)
                        {
                            window.DrawTriangle(tri, Color.Orange);
                        }
                    }
                    else
                    {
                        Log.Warning(this, $"RenderMode is set to \"WireframeTextured\", but Texture is null");
                    }
                    break;
                case RenderMode3D.Solid:
                    foreach (Triangle tri in triangles)
                    {
                        window.DrawFilledTriangle(tri);
                    }
                    break;
                case RenderMode3D.Textured:
                    if (Texture != null)
                    {
                        foreach (Triangle tri in triangles)
                        {
                            window.DrawTexturedTriangle(tri, Texture);
                        }
                    }
                    else
                    {
                        Log.Warning(this, $"RenderMode is set to \"Textured\", but Texture is null");
                    }
                    break;
            }
        }
    }
}