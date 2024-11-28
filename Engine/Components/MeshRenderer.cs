using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Enums;
using SaturnEngine.Engine.Structs;
using SDL2;

using SDL_Renderer = nint;
using SDL_Texture = nint;

namespace SaturnEngine.Engine.Components
{
    public class MeshRenderer : Renderer3D
    {

        [Export] public RenderMode RenderMode { get; set; } = RenderMode.Solid;
        [Export] public Mesh Mesh { get; set; }

        internal override void OnRender(SDL_Renderer renderer, Matrix4x4 matProj, Camera3D camera)
        {

            Matrix4x4 matCamera = Graphics.CalculateCameraMatrix(camera.Transform.Position, camera.Transform.Rotation);
            
            List<Triangle> triangles = Graphics.CalculateTriangles(matProj, matCamera, Transform.Position, Transform.Rotation, Mesh);

            if (RenderMode == RenderMode.Solid)
            {
                if(Mesh.Texture != null)
                {

                }
                else
                {
                    foreach (Triangle tri in triangles)
                    {
                        Graphics.DrawTriangle(tri);
                    }
                }
            }
            else
            {
                Color color = new Color(0xFF, 0xAA, 0x00, 0xFF);
                foreach (Triangle tri in triangles)
                {
                    Graphics.DrawLine((int)tri.Vertices[0].X, (int)tri.Vertices[0].Y, (int)tri.Vertices[1].X, (int)tri.Vertices[1].Y, color);
                    Graphics.DrawLine((int)tri.Vertices[1].X, (int)tri.Vertices[1].Y, (int)tri.Vertices[2].X, (int)tri.Vertices[2].Y, color);
                    Graphics.DrawLine((int)tri.Vertices[2].X, (int)tri.Vertices[2].Y, (int)tri.Vertices[0].X, (int)tri.Vertices[0].Y, color);
                }
            }
        }

    }
}