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

        internal override void OnRender(SDL_Renderer renderer, Matrix4x4 projectionMatrix, Camera3D camera)
        {
            Vector3 position = Transform.Position;
            Vector3 rotation = Transform.Rotation.ToEuler();

            Vector3 cameraPosition = camera.Transform.Position;
            Vector3 cameraRotation = camera.Transform.Rotation.ToEuler();

            Matrix4x4 matRotZ = Matrix4x4.MakeRotationZ(rotation.Z);
            Matrix4x4 matRotY = Matrix4x4.MakeRotationY(rotation.Y);
            Matrix4x4 matRotX = Matrix4x4.MakeRotationX(rotation.X);

            Matrix4x4 matTrans = Matrix4x4.MakeTranslation(position.X, position.Y, position.Z);

            Matrix4x4 matWorld = Matrix4x4.Identity;
            matWorld = Matrix4x4.MultiplyMatrix(matWorld, matRotZ);
            matWorld = Matrix4x4.MultiplyMatrix(matWorld, matRotY);
            matWorld = Matrix4x4.MultiplyMatrix(matWorld, matRotX);
            matWorld = Matrix4x4.MultiplyMatrix(matWorld, matTrans);

            Vector3 target = new Vector3(0, 0, 1);

            matRotZ = Matrix4x4.MakeRotationZ(cameraRotation.Z);
            matRotY = Matrix4x4.MakeRotationY(cameraRotation.Y);
            matRotX = Matrix4x4.MakeRotationX(cameraRotation.X);

            Matrix4x4 matCameraRot = Matrix4x4.Identity;
            matCameraRot = Matrix4x4.MultiplyMatrix(matCameraRot, matRotZ);
            matCameraRot = Matrix4x4.MultiplyMatrix(matCameraRot, matRotY);
            matCameraRot = Matrix4x4.MultiplyMatrix(matCameraRot, matRotX);

            Vector3 lookDir = Matrix4x4.MultiplyVector(matCameraRot, target);
            target = cameraPosition + lookDir;

            Matrix4x4 matCamera = Matrix4x4.PointAt(cameraPosition, target, Vector3.Up);
            Matrix4x4 matView = Matrix4x4.QuickInverse(matCamera);


            List<Triangle> trianglesToRaster = new List<Triangle>();

            foreach (Triangle triangle in Mesh.Triangles)
            {
                Triangle triangleTransformed = new Triangle();
                triangleTransformed.Vertices[0] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[0]);
                triangleTransformed.Vertices[1] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[1]);
                triangleTransformed.Vertices[2] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[2]);
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

                Vector3 cameraRay = triangleTransformed.Vertices[0] - cameraPosition;

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
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[0]),
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[1]),
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[2]),
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
                            Matrix4x4.MultiplyVector(projectionMatrix, clipped[n].Vertices[0]),
                            Matrix4x4.MultiplyVector(projectionMatrix, clipped[n].Vertices[1]),
                            Matrix4x4.MultiplyVector(projectionMatrix, clipped[n].Vertices[2]),
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

                if (RenderMode == RenderMode.Solid)
                {
                    if(Mesh.Texture != null)
                    {

                    }
                    else
                    {
                        foreach (Triangle tri in triangles)
                        {
                            SDL.SDL_Color Color = tri.Color.ToSDL_Color();
                            SDL.SDL_Vertex[] vertices = new SDL.SDL_Vertex[3];

                            vertices[0] = new SDL.SDL_Vertex
                            {
                                position = new SDL.SDL_FPoint { x = tri.Vertices[0].X, y = tri.Vertices[0].Y },
                                color = Color
                            };
                            vertices[1] = new SDL.SDL_Vertex
                            {
                                position = new SDL.SDL_FPoint { x = tri.Vertices[1].X, y = tri.Vertices[1].Y },
                                color = Color
                            };
                            vertices[2] = new SDL.SDL_Vertex
                            {
                                position = new SDL.SDL_FPoint { x = tri.Vertices[2].X, y = tri.Vertices[2].Y },
                                color = Color
                            };

                            int[] indices = { 0, 1, 2 };

                            SDL.SDL_RenderGeometry(renderer, IntPtr.Zero, vertices, vertices.Length, indices, indices.Length);
                        }
                    }
                }
                else
                {
                    foreach (Triangle tri in triangles)
                    {
                        SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xAA, 0x00, 0xFF);
                        SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[0].X, (int)tri.Vertices[0].Y, (int)tri.Vertices[1].X, (int)tri.Vertices[1].Y);
                        SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[1].X, (int)tri.Vertices[1].Y, (int)tri.Vertices[2].X, (int)tri.Vertices[2].Y);
                        SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[2].X, (int)tri.Vertices[2].Y, (int)tri.Vertices[0].X, (int)tri.Vertices[0].Y);
                        SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                    }
                }
            }

        }

    }
}