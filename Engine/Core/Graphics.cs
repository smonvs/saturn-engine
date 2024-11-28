﻿using SaturnEngine.Engine.Components;
using SaturnEngine.Engine.Structs;
using SDL2;
using System.Reflection.Metadata.Ecma335;

using SDL_Renderer = System.IntPtr;
using SDL_Texture = System.IntPtr;

namespace SaturnEngine.Engine.Core
{
    internal static class Graphics
    {

        private static SDL_Renderer _renderer = IntPtr.Zero; 
        private static SDL_Texture _texture = IntPtr.Zero;
        private static uint[] _pixels;

        internal static void Init(SDL_Renderer renderer)
        {
            _renderer = renderer;
            _texture = SDL.SDL_CreateTexture
            (
                renderer, 
                SDL.SDL_PIXELFORMAT_ARGB8888, 
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, 
                (int)Window.Size.X, 
                (int)Window.Size.Y
            );
            _pixels = new UInt32[(int)Window.Size.X * (int)Window.Size.Y];
        }

        internal static Matrix4x4 CalculateWorldMatrix(Vector3 position, Quaternion rotation)
        {
            Vector3 rotationRadiants = rotation.ToEuler(); 

            Matrix4x4 matRotZ = Matrix4x4.MakeRotationZ(rotationRadiants.Z);
            Matrix4x4 matRotY = Matrix4x4.MakeRotationY(rotationRadiants.Y);
            Matrix4x4 matRotX = Matrix4x4.MakeRotationX(rotationRadiants.X);

            Matrix4x4 matTrans = Matrix4x4.MakeTranslation(position.X, position.Y, position.Z);

            Matrix4x4 matWorld = Matrix4x4.Identity;
            matWorld = matWorld * matRotZ;
            matWorld = matWorld * matRotY;
            matWorld = matWorld * matRotX;
            matWorld = matWorld * matTrans;

            return matWorld;
        }

        internal static Matrix4x4 CalculateCameraMatrix(Vector3 position, Quaternion rotation)
        {
            Vector3 rotationRadiants = rotation.ToEuler();

            Matrix4x4 matRotZ = Matrix4x4.MakeRotationZ(rotationRadiants.Z);
            Matrix4x4 matRotY = Matrix4x4.MakeRotationY(rotationRadiants.Y);
            Matrix4x4 matRotX = Matrix4x4.MakeRotationX(rotationRadiants.X);

            Matrix4x4 matCamera = Matrix4x4.Identity;
            matCamera = matCamera * matRotZ;
            matCamera = matCamera * matRotY;
            matCamera = matCamera * matRotX;

            Vector3 lookDir = matCamera * new Vector3(0, 0, 1);
            Vector3 target = position + lookDir;

            matCamera = Matrix4x4.PointAt(position, target, Vector3.Up);

            return matCamera;
        }

        internal static List<Triangle> CalculateTriangles(Matrix4x4 matProj, Matrix4x4 matCamera, Vector3 position, Quaternion rotation, Mesh mesh)
        {

            List<Triangle> result = new List<Triangle>();

            Matrix4x4 matWorld = CalculateWorldMatrix(position, rotation);
            Matrix4x4 matView = Matrix4x4.QuickInverse(matCamera);

            List<Triangle> trianglesToRaster = new List<Triangle>();

            foreach (Triangle triangle in mesh.Triangles)
            {
                Triangle triangleTransformed = new Triangle();
                triangleTransformed.Vertices[0] = matWorld * triangle.Vertices[0];
                triangleTransformed.Vertices[1] = matWorld * triangle.Vertices[1];
                triangleTransformed.Vertices[2] = matWorld * triangle.Vertices[2];
                triangleTransformed.TexCoords[0] = triangle.TexCoords[0];
                triangleTransformed.TexCoords[1] = triangle.TexCoords[2];
                triangleTransformed.TexCoords[1] = triangle.TexCoords[2];

                Vector3 normal = new Vector3();
                Vector3 line1 = new Vector3();
                Vector3 line2 = new Vector3();

                line1 = triangleTransformed.Vertices[1] - triangleTransformed.Vertices[0];
                line2 = triangleTransformed.Vertices[2] - triangleTransformed.Vertices[0];

                normal = Vector3.Cross(line1, line2);
                normal.Normalize();

                Vector3 cameraRay = triangleTransformed.Vertices[0] - Camera3D.Main.Transform.Position;

                if (Vector3.Dot(normal, cameraRay) < 0.0f)
                {
                    Vector3 lightDirection = new Vector3(0, 0, -1);
                    lightDirection.Normalize();

                    // Calculate color
                    float dp = MathF.Max(0.1f, Vector3.Dot(lightDirection, normal));
                    Color color = new Color((byte)(255 * dp), (byte)(255 * dp), (byte)(255 * dp), 255);

                    // Convert world space to view space
                    Triangle triangleViewed = new Triangle
                    (
                    matView * triangleTransformed.Vertices[0],
                    matView * triangleTransformed.Vertices[1],
                    matView * triangleTransformed.Vertices[2],
                        triangleTransformed.TexCoords[0],
                        triangleTransformed.TexCoords[1],
                        triangleTransformed.TexCoords[2]
                    );
                    triangleViewed.Color = color;

                    // Clip viewed triangle against near plane
                    int clippedTriangles = 0;
                    Triangle[] clipped = { new Triangle(), new Triangle() };
                    clippedTriangles = Triangle.ClipAgainstPlane(new Vector3(0, 0, 0.1f), new Vector3(0, 0, 1), ref triangleViewed, ref clipped[0], ref clipped[1]);

                    for (int n = 0; n < clippedTriangles; n++)
                    {
                        // Project triangles from 3D to 2D
                        Triangle triangleProjected = new Triangle
                        (
                            matProj * clipped[n].Vertices[0],
                            matProj * clipped[n].Vertices[1],
                            matProj * clipped[n].Vertices[2],
                            clipped[n].TexCoords[0],
                            clipped[n].TexCoords[1],
                            clipped[n].TexCoords[2]
                        );
                        triangleProjected.Color = clipped[n].Color;

                        triangleProjected.Vertices[0] = triangleProjected.Vertices[0] / triangleProjected.Vertices[0].W;
                        triangleProjected.Vertices[1] = triangleProjected.Vertices[1] / triangleProjected.Vertices[1].W;
                        triangleProjected.Vertices[2] = triangleProjected.Vertices[2] / triangleProjected.Vertices[2].W;

                        Vector3 offsetView = new Vector3(1, 1, 0);
                        triangleProjected.Vertices[0] = triangleProjected.Vertices[0] + offsetView;
                        triangleProjected.Vertices[1] = triangleProjected.Vertices[1] + offsetView;
                        triangleProjected.Vertices[2] = triangleProjected.Vertices[2] + offsetView;

                        float aspectRatio = Window.Size.X / Window.Size.Y;

                        triangleProjected.Vertices[0].X *= 0.5f * Window.Size.X;
                        triangleProjected.Vertices[0].Y *= 0.5f * Window.Size.Y;
                        triangleProjected.Vertices[1].X *= 0.5f * Window.Size.X;
                        triangleProjected.Vertices[1].Y *= 0.5f * Window.Size.Y;
                        triangleProjected.Vertices[2].X *= 0.5f * Window.Size.X;
                        triangleProjected.Vertices[2].Y *= 0.5f * Window.Size.Y;

                        trianglesToRaster.Add(triangleProjected);
                    }

                }

            }

            trianglesToRaster.Sort((tri1, tri2) =>
            {
                float z1 = (tri1.Vertices[0].Z + tri1.Vertices[1].Z + tri1.Vertices[2].Z) / 3.0f;
                float z2 = (tri2.Vertices[0].Z + tri2.Vertices[1].Z + tri2.Vertices[2].Z) / 3.0f;
                return z2.CompareTo(z1);
            });

            foreach (Triangle triangleToRaster in trianglesToRaster)
            {
                Triangle[] clipped = { new Triangle(), new Triangle() };
                List<Triangle> triangles = new List<Triangle>();
                triangles.Add(triangleToRaster);
                int newTriangles = 1;

                for (int p = 0; p < 4; p++)
                {
                    int trianglesToAdd = 0;

                    while (newTriangles > 0)
                    {
                        Triangle test = triangles.First();
                        triangles.RemoveAt(0);
                        newTriangles--;

                        switch (p)
                        {
                            case 0:
                                trianglesToAdd = Triangle.ClipAgainstPlane(new Vector3(0, 0, 0), new Vector3(0, 1, 0), ref test, ref clipped[0], ref clipped[1]);
                                break;
                            case 1:
                                trianglesToAdd = Triangle.ClipAgainstPlane(new Vector3(0, Window.Size.Y - 1, 0), new Vector3(0, -1, 0), ref test, ref clipped[0], ref clipped[1]);
                                break;
                            case 2:
                                trianglesToAdd = Triangle.ClipAgainstPlane(new Vector3(0, 0, 0), new Vector3(1, 0, 0), ref test, ref clipped[0], ref clipped[1]);
                                break;
                            case 3:
                                trianglesToAdd = Triangle.ClipAgainstPlane(new Vector3(Window.Size.X - 1, 0, 0), new Vector3(-1, 0, 0), ref test, ref clipped[0], ref clipped[1]);
                                break;
                        }

                        for (int w = 0; w < trianglesToAdd; w++)
                        {
                            triangles.Add(clipped[w]);
                        }
                    }
                    newTriangles = triangles.Count;
                }

                result.AddRange(triangles);
            }

            return result;
        }

        internal static void DrawLine(float x0, float y0, float x1, float y1, Color color)
        {
            SDL.SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL.SDL_RenderDrawLine(_renderer, (int)x0, (int)y0, (int)x1, (int)y1);
        }

        internal static void DrawTriangle(Triangle triangle)
        {
            SDL.SDL_Color Color = triangle.Color.ToSDL_Color();
            SDL.SDL_Vertex[] vertices = new SDL.SDL_Vertex[3];

            vertices[0] = new SDL.SDL_Vertex
            {
                position = new SDL.SDL_FPoint { x = triangle.Vertices[0].X, y = triangle.Vertices[0].Y },
                color = Color
            };
            vertices[1] = new SDL.SDL_Vertex
            {
                position = new SDL.SDL_FPoint { x = triangle.Vertices[1].X, y = triangle.Vertices[1].Y },
                color = Color
            };
            vertices[2] = new SDL.SDL_Vertex
            {
                position = new SDL.SDL_FPoint { x = triangle.Vertices[2].X, y = triangle.Vertices[2].Y },
                color = Color
            };
            int[] indices = { 0, 1, 2 };

            SDL.SDL_RenderGeometry(_renderer, IntPtr.Zero, vertices, vertices.Length, indices, indices.Length);
        }

    }
}