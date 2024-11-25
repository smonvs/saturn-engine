using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaturnEngine.Engine.Core;
using SaturnEngine.Engine.Structs;
using SDL2;

using SDL_Renderer = nint;

namespace SaturnEngine.Engine.Components
{
    public class MeshRenderer : Renderer3D
    {

        public bool DrawWireframe { get; set; } = false;

        private Mesh _mesh;
        private Matrix4x4 _projectionMatrix;
        private float _fTheta = 0.0f;
        private Vector3 _camera = Vector3.Zero;
        private Vector3 _lookDir = new Vector3(0, 0, 1);
        private float _deltaTime = 0.0f;
        private float _yaw = 0.0f;

        public override void OnInit()
        {
            _mesh = ResourceLoader.Load<Mesh>("c:\\Users\\Dev\\Downloads\\test2.obj");
            
            _projectionMatrix = Matrix4x4.MakeProjection(90.0f, Window.Size.X / Window.Size.Y, 0.1f, 1000.0f);
        }

        public override void OnUpdate(float deltaTime)
        {
            _deltaTime = deltaTime;
        }

        internal override void OnRender(SDL_Renderer renderer)
        {
            if (Input.IsKeyPressed(KeyCode.W))
            {
                _camera.Y -= 4.0f * _deltaTime;
            }
            if (Input.IsKeyPressed(KeyCode.S))
            {
                _camera.Y += 4.0f * _deltaTime;
            }
            if(Input.IsKeyPressed(KeyCode.A))
            {
                _camera.X -= 4.0f * _deltaTime;
            }
            if(Input.IsKeyPressed(KeyCode.D))
            {
                _camera.X += 4.0f * _deltaTime;
            }

            Vector3 forward = _lookDir * 8.0f * _deltaTime;

            if(Input.IsKeyPressed(KeyCode.ArrowUp))
            {
                _camera -= forward;
            }
            if (Input.IsKeyPressed(KeyCode.ArrowDown))
            {
                _camera += forward;
            }
            if (Input.IsKeyPressed(KeyCode.ArrowLeft))
            {
                _yaw += 2.0f * _deltaTime;
            }
            if (Input.IsKeyPressed(KeyCode.ArrowRight))
            {
                _yaw -= 2.0f * _deltaTime;
            }

            List<Triangle> trianglesToRaster = new List<Triangle>();

            //_fTheta += 1.0f * _deltaTime;
            Matrix4x4 matRotZ = Matrix4x4.MakeRotationZ(_fTheta * 0.5f);
            Matrix4x4 matRotX = Matrix4x4.MakeRotationX(_fTheta);

            Matrix4x4 matTrans = Matrix4x4.MakeTranslation(0.0f, 0.0f, 5.0f);

            Matrix4x4 matWorld = Matrix4x4.Identity;
            matWorld = Matrix4x4.MultiplyMatrix(matRotZ, matRotX);
            matWorld = Matrix4x4.MultiplyMatrix(matWorld, matTrans);

            Vector3 up = new Vector3(0, 1, 0);
            Vector3 target = new Vector3(0, 0, 1);
            
            Matrix4x4 matCameraRot = Matrix4x4.MakeRotationY(_yaw);
            _lookDir = Matrix4x4.MultiplyVector(matCameraRot, target);
            target = _camera + _lookDir;

            Matrix4x4 matCamera = Matrix4x4.PointAt(_camera, target, up);
            Matrix4x4 matView = Matrix4x4.QuickInverse(matCamera);

            foreach (Triangle triangle in _mesh.Triangles)
            {
                Triangle triangleTransformed = new Triangle();
                triangleTransformed.Vertices[0] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[0]);
                triangleTransformed.Vertices[1] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[1]);
                triangleTransformed.Vertices[2] = Matrix4x4.MultiplyVector(matWorld, triangle.Vertices[2]);

                Vector3 normal = new Vector3();
                Vector3 line1 = new Vector3();
                Vector3 line2 = new Vector3();

                line1 = triangleTransformed.Vertices[1] - triangleTransformed.Vertices[0];
                line2 = triangleTransformed.Vertices[2] - triangleTransformed.Vertices[0];

                normal = Vector3.Cross(line1, line2);
                normal.Normalize();

                Vector3 cameraRay = triangleTransformed.Vertices[0] - _camera;

                if (Vector3.Dot(normal, cameraRay) < 0.0f)
                {
                    Vector3 lightDirection = new Vector3(0, 0, -1);
                    lightDirection.Normalize();

                    float dp = MathF.Max(0.1f, Vector3.Dot(lightDirection, normal));

                    Triangle triangleViewed = new Triangle
                    (
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[0]),
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[1]),
                        Matrix4x4.MultiplyVector(matView, triangleTransformed.Vertices[2])
                    );

                    Triangle triangleProjected = new Triangle
                    (
                        Matrix4x4.MultiplyVector(_projectionMatrix, triangleViewed.Vertices[0]),
                        Matrix4x4.MultiplyVector(_projectionMatrix, triangleViewed.Vertices[1]),
                        Matrix4x4.MultiplyVector(_projectionMatrix, triangleViewed.Vertices[2])
                    );

                    triangleProjected.Vertices[0] = triangleProjected.Vertices[0] / triangleProjected.Vertices[0].W;
                    triangleProjected.Vertices[1] = triangleProjected.Vertices[1] / triangleProjected.Vertices[1].W;
                    triangleProjected.Vertices[2] = triangleProjected.Vertices[2] / triangleProjected.Vertices[2].W;

                    Vector3 offsetView = new Vector3(1, 1, 0);
                    triangleProjected.Vertices[0] = triangleProjected.Vertices[0] + offsetView;
                    triangleProjected.Vertices[1] = triangleProjected.Vertices[1] + offsetView;
                    triangleProjected.Vertices[2] = triangleProjected.Vertices[2] + offsetView;    

                    triangleProjected.Vertices[0].X *= 0.5f * Window.Size.X;
                    triangleProjected.Vertices[0].Y *= 0.5f * Window.Size.Y;
                    triangleProjected.Vertices[1].X *= 0.5f * Window.Size.X;
                    triangleProjected.Vertices[1].Y *= 0.5f * Window.Size.Y;
                    triangleProjected.Vertices[2].X *= 0.5f * Window.Size.X;
                    triangleProjected.Vertices[2].Y *= 0.5f * Window.Size.Y;

                    triangleProjected.color = new Color((byte)(255 * dp), (byte)(255 * dp), (byte)(255 * dp), 255);
                    trianglesToRaster.Add(triangleProjected);
                }

            }

            trianglesToRaster.Sort((tri1, tri2) =>
            {
                float z1 = (tri1.Vertices[0].Z + tri1.Vertices[1].Z + tri1.Vertices[2].Z) / 3.0f;
                float z2 = (tri2.Vertices[0].Z + tri2.Vertices[1].Z + tri2.Vertices[2].Z) / 3.0f;
                return z2.CompareTo(z1);
            });
            int faces = 0;
            foreach (Triangle tri in trianglesToRaster)
            {
                faces++;
                SDL.SDL_Color color = tri.color.ToSDL_Color();
                SDL.SDL_Vertex[] vertices = new SDL.SDL_Vertex[3];
                vertices[0] = new SDL.SDL_Vertex
                {
                    position = new SDL.SDL_FPoint { x = tri.Vertices[0].X, y = tri.Vertices[0].Y },
                    color = color
                };
                vertices[1] = new SDL.SDL_Vertex
                {
                    position = new SDL.SDL_FPoint { x = tri.Vertices[1].X, y = tri.Vertices[1].Y },
                    color = color
                };
                vertices[2] = new SDL.SDL_Vertex
                {
                    position = new SDL.SDL_FPoint { x = tri.Vertices[2].X, y = tri.Vertices[2].Y },
                    color = color
                };

                int[] indices = { 0, 1, 2 };

                SDL.SDL_RenderGeometry(renderer, IntPtr.Zero, vertices, vertices.Length, indices, indices.Length);

                if (DrawWireframe)
                {
                    SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xAA, 0x00, 0xFF);
                    SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[0].X, (int)tri.Vertices[0].Y, (int)tri.Vertices[1].X, (int)tri.Vertices[1].Y);
                    SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[1].X, (int)tri.Vertices[1].Y, (int)tri.Vertices[2].X, (int)tri.Vertices[2].Y);
                    SDL.SDL_RenderDrawLine(renderer, (int)tri.Vertices[2].X, (int)tri.Vertices[2].Y, (int)tri.Vertices[0].X, (int)tri.Vertices[0].Y);
                    SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                }
            }
            Log.Debug(faces.ToString());
            faces = 0;

        }

    }
}
