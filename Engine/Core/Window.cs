using SaturnEngine.Engine.Structs;
using System.Runtime.InteropServices;

namespace SaturnEngine.Engine.Core
{
    public class Window
    {

        #region Properties

        public static string Title { get; private set; }
        public static Vector2 WindowSize { get; private set; }
        public static Vector2 BufferSize { get; private set; }

        #endregion

        #region Fields

        private Application _application;
        private byte[] _buffer;
        private IntPtr _hWnd;
        private IntPtr _hdc;
        private IntPtr _hdcMem;
        private IntPtr _bitmap;
        private BITMAPINFO _bmi;

        #endregion
        
        #region Constructors

        internal Window(Application application, string title, int width, int height, float renderingScale = 1.0f)
        {
            float scale = Math.Clamp(renderingScale, 0.1f, 1.0f);

            Title = title;
            WindowSize = new Vector2(width, height);
            BufferSize = new Vector2(width * scale, height * scale);

            _application = application;
            _buffer = new byte[(int)BufferSize.X * (int)BufferSize.Y * 4];

            Init();
        }

        #endregion

        #region Methods
        #region Private

        private void Init()
        {
            _wndProcDelegate = new WndProc(WindowProc);

            WNDCLASS wc = new WNDCLASS
            {
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate),
                hInstance = IntPtr.Zero,
                lpszClassName = "SaturnEngineWindow"
            };

            RegisterClass(ref wc);

            _hWnd = CreateWindowEx
                (
                    0, 
                    wc.lpszClassName, 
                    Title, 
                    WS_OVERLAPPEDWINDOW, 
                    CW_USEDEFAULT, 
                    CW_USEDEFAULT, 
                    (int)WindowSize.X, 
                    (int)WindowSize.Y, 
                    IntPtr.Zero, 
                    IntPtr.Zero, 
                    IntPtr.Zero, 
                    IntPtr.Zero
                );

            _hdc = GetDC(_hWnd);
            _hdcMem = CreateCompatibleDC(_hdc);
            _bitmap = CreateCompatibleBitmap(_hdc, (int)WindowSize.X, (int)WindowSize.Y);
            
            _bmi = new BITMAPINFO
            {
                bmiHeader = new BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                    biWidth = (int)BufferSize.X,
                    biHeight = -(int)BufferSize.Y,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0,
                    biSizeImage = 0,
                    biXPelsPerMeter = 0,
                    biYPelsPerMeter = 0,
                    biClrUsed = 0,
                    biClrImportant = 0
                }
            };
        }
        
        private IntPtr WindowProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {
                case WM_DESTROY:
                    Environment.Exit(0);
                    break;
                case WM_KEYDOWN:
                    Input.KeyDown((KeyCode)wParam);
                    break;
                case WM_KEYUP:
                    Input.KeyUp((KeyCode)wParam);
                    break;
                case WM_MOUSEMOVE:
                    int x = lParam.ToInt32() & 0xFFFF;
                    int y = lParam.ToInt32() >> 16;
                    Input.MousePosition = new Vector2(x, y);
                    break;
            }
            return DefWindowProc(hwnd, uMsg, wParam, lParam);
        }

        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        private void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        private void Swap(ref Vector2 v1, ref Vector2 v2)
        {
            Vector2 temp = v1;
            v1 = v2;
            v2 = temp;
        }

        private void Swap(ref Vector3 v1, ref Vector3 v2)
        {
            Vector3 temp = v1; 
            v1 = v2; 
            v2 = temp;
        }

        #endregion

        #region Internal

        internal bool Open()
        {
            if (_hWnd == IntPtr.Zero)
            {
                Log.Error("Failed to create window");
                return false;
            }

            ShowWindow(_hWnd, SW_SHOW);
            UpdateWindow(_hWnd);

            return true;
        }

        internal void ShowFpsInTitle(int fps, float frametime)
        {
            SetWindowText(_hWnd, $"{Title} - FPS: {fps} - Frametime: {frametime}ms");
        }

        internal void ProcessMessages()
        {
            MSG msg;
            while (PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        internal void RenderDraw()
        {
            StretchDIBits(_hdc, 0, 0, (int)WindowSize.X, (int)WindowSize.Y, 0, 0, (int)BufferSize.X, (int)BufferSize.Y, _buffer, ref _bmi, 0, SRCCOPY);

            Array.Clear(_buffer, 0, _buffer.Length);
        }

        internal void RenderClear()
        {

        }

        #endregion

        #region Public

        public void DrawPixel(float x, float y, Color color)
        {
            if (x >= 0 && x < BufferSize.X && y >= 0 && y < BufferSize.Y)
            {
                int index = ((int)y * (int)BufferSize.X + (int)x) * 4;

                if (color.A < 0xFF)
                {
                    float alpha = color.A / 255.0f;
                    float invAlpha = 1.0f - alpha;

                    _buffer[index] = (byte)((color.B * alpha) + (_buffer[index] * invAlpha));
                    _buffer[index + 1] = (byte)((color.G * alpha) + (_buffer[index + 1] * invAlpha));
                    _buffer[index + 2] = (byte)((color.R * alpha) + (_buffer[index + 2] * invAlpha));
                    _buffer[index + 3] = (byte)((color.A * alpha) + (_buffer[index + 3] * invAlpha));
                }
                else
                {
                    _buffer[index] = color.B;
                    _buffer[index + 1] = color.G;
                    _buffer[index + 2] = color.R;
                    _buffer[index + 3] = color.A;
                }
            }
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            int ix1 = (int)Math.Round(x1);
            int iy1 = (int)Math.Round(y1);
            int ix2 = (int)Math.Round(x2);
            int iy2 = (int)Math.Round(y2);

            int dx = Math.Abs(ix2 - ix1);
            int dy = Math.Abs(iy2 - iy1);
            int sx = ix1 < ix2 ? 1 : -1;
            int sy = iy1 < iy2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (ix1 >= 0 && ix1 < BufferSize.X && iy1 >= 0 && iy1 < BufferSize.Y)
                {
                    int index = (iy1 * (int)BufferSize.X + ix1) * 4;

                    if (color.A < 0xFF)
                    {
                        float alpha = color.A / 255.0f;
                        float invAlpha = 1.0f - alpha;

                        _buffer[index] = (byte)((color.B * alpha) + (_buffer[index] * invAlpha));
                        _buffer[index + 1] = (byte)((color.G * alpha) + (_buffer[index + 1] * invAlpha));
                        _buffer[index + 2] = (byte)((color.R * alpha) + (_buffer[index + 2] * invAlpha));
                        _buffer[index + 3] = (byte)((color.A * alpha) + (_buffer[index + 3] * invAlpha));
                    }
                    else
                    {
                        _buffer[index] = color.B;
                        _buffer[index + 1] = color.G;
                        _buffer[index + 2] = color.R;
                        _buffer[index + 3] = color.A;
                    }
                }

                if (ix1 == ix2 && iy1 == iy2) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    ix1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    iy1 += sy;
                }
            }
        }

        public void DrawHorizontalLine(float x1, float x2, float y, Color color)
        {
            if (x1 > x2)
            {
                Swap(ref x1, ref x2);
            }

            for (int x = (int)x1; x <= x2; x++)
            {
                if (x >= 0 && x < BufferSize.X && y >= 0 && y < BufferSize.Y)
                {
                    int index = (int)(y * BufferSize.X + x) * 4;

                    if (color.A < 0xFF)
                    {
                        float alpha = color.A / 255.0f;
                        float invAlpha = 1.0f - alpha;

                        _buffer[index] = (byte)((color.B * alpha) + (_buffer[index] * invAlpha));
                        _buffer[index + 1] = (byte)((color.G * alpha) + (_buffer[index + 1] * invAlpha));
                        _buffer[index + 2] = (byte)((color.R * alpha) + (_buffer[index + 2] * invAlpha));
                        _buffer[index + 3] = (byte)((color.A * alpha) + (_buffer[index + 3] * invAlpha));
                    }
                    else
                    {
                        _buffer[index] = color.B;
                        _buffer[index + 1] = color.G;
                        _buffer[index + 2] = color.R;
                        _buffer[index + 3] = color.A;
                    }
                }
            }
        }

        public void DrawRect(float x, float y, float width, float height, Color color)
        {
            DrawLine(x, y, x + width, y, color);
            DrawLine(x, y, x, y + height, color);
            DrawLine(x + width, y, x + width, y + height, color);
            DrawLine(x, y + height, x + width, y + height, color);
        }

        public void DrawFilledRect(float x, float y, float width, float height, Color color)
        {
            for(int i = 0; i < height; i++)
            {
                DrawHorizontalLine(x, x + width, y + i, color);
            }
        }

        public void DrawTriangle(Triangle triangle, Color? color = null)
        {
            if(color != null)
            {
                DrawLine(triangle.Vertices[0].X, triangle.Vertices[0].Y, triangle.Vertices[1].X, triangle.Vertices[1].Y, (Color)color);
                DrawLine(triangle.Vertices[1].X, triangle.Vertices[1].Y, triangle.Vertices[2].X, triangle.Vertices[2].Y, (Color)color);
                DrawLine(triangle.Vertices[2].X, triangle.Vertices[2].Y, triangle.Vertices[0].X, triangle.Vertices[0].Y, (Color)color);
            }
            else
            {
                DrawLine(triangle.Vertices[0].X, triangle.Vertices[0].Y, triangle.Vertices[1].X, triangle.Vertices[1].Y, triangle.Color);
                DrawLine(triangle.Vertices[1].X, triangle.Vertices[1].Y, triangle.Vertices[2].X, triangle.Vertices[2].Y, triangle.Color);
                DrawLine(triangle.Vertices[2].X, triangle.Vertices[2].Y, triangle.Vertices[0].X, triangle.Vertices[0].Y, triangle.Color);
            }
        }

        public void DrawFilledTriangle(Triangle triangle, Color? color = null)
        {
            Color c = color != null ? Color.White : triangle.Color;

            float x1 = triangle.Vertices[0].X;
            float y1 = triangle.Vertices[0].Y;
            float x2 = triangle.Vertices[1].X;
            float y2 = triangle.Vertices[1].Y;
            float x3 = triangle.Vertices[2].X;
            float y3 = triangle.Vertices[2].Y;

            if (y1 > y2)
            {
                Swap(ref y1, ref y2);
                Swap(ref x1, ref x2);
            }
            if (y1 > y3)
            {
                Swap(ref y1, ref y3);
                Swap(ref x1, ref x3);
            }
            if (y2 > y3)
            {
                Swap(ref y2, ref y3);
                Swap(ref x2, ref x3);
            }

            float dx1 = (x2 - x1) / (y2 - y1);
            float dx2 = (x3 - x1) / (y3 - y1);
            float dx3 = (x3 - x2) / (y3 - y2);

            float sx = x1;
            float ex = x1;

            for (float y = y1; y <= y2; y++)
            {
                DrawLine(sx, y, ex, y, c);
                sx += dx1;
                ex += dx2;
            }

            sx = x2;
            ex = x1 + dx2 * (y2 - y1);

            for (float y = y2; y <= y3; y++)
            {
                DrawLine(sx, y, ex, y, c);
                sx += dx3;
                ex += dx2;
            }
        }

        public void DrawTexturedTriangle(Triangle triangle, Texture texture)
        {
            int x1 = (int)triangle.Vertices[0].X;
            int y1 = (int)triangle.Vertices[0].Y;
            float u1 = triangle.TexCoords[0].X;
            float v1 = triangle.TexCoords[0].Y;
            float w1 = triangle.TexCoords[0].W;

            int x2 = (int)triangle.Vertices[1].X;
            int y2 = (int)triangle.Vertices[1].Y;
            float u2 = triangle.TexCoords[1].X;
            float v2 = triangle.TexCoords[1].Y;
            float w2 = triangle.TexCoords[1].W;

            int x3 = (int)triangle.Vertices[2].X;
            int y3 = (int)triangle.Vertices[2].Y;
            float u3 = triangle.TexCoords[2].X;
            float v3 = triangle.TexCoords[2].Y;
            float w3 = triangle.TexCoords[2].W;

            if (y2 < y1)
            {
                Swap(ref y1, ref y2);
                Swap(ref x1, ref x2);
                Swap(ref u1, ref u2);
                Swap(ref v1, ref v2);
                Swap(ref w1, ref w2);
            }

            if (y3 < y1)
            {
                Swap(ref y1, ref y3);
                Swap(ref x1, ref x3);
                Swap(ref u1, ref u3);
                Swap(ref v1, ref v3);
                Swap(ref w1, ref w3);
            }

            if (y3 < y2)
            {
                Swap(ref y2, ref y3);
                Swap(ref x2, ref x3);
                Swap(ref u2, ref u3);
                Swap(ref v2, ref v3);
                Swap(ref w2, ref w3);
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;
            float dv1 = v2 - v1;
            float du1 = u2 - u1;
            float dw1 = w2 - w1;

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;
            float dv2 = v3 - v1;
            float du2 = u3 - u1;
            float dw2 = w3 - w1;

            float tex_u, tex_v, tex_w;

            float dax_step = 0, dbx_step = 0,
                du1_step = 0, dv1_step = 0,
                du2_step = 0, dv2_step = 0,
                dw1_step = 0, dw2_step = 0;

            if (dy1 != 0) dax_step = dx1 / (float)Math.Abs(dy1);
            if (dy2 != 0) dbx_step = dx2 / (float)Math.Abs(dy2);

            if (dy1 != 0) du1_step = du1 / (float)Math.Abs(dy1);
            if (dy1 != 0) dv1_step = dv1 / (float)Math.Abs(dy1);
            if (dy1 != 0) dw1_step = dw1 / (float)Math.Abs(dy1);

            if (dy2 != 0) du2_step = du2 / (float)Math.Abs(dy2);
            if (dy2 != 0) dv2_step = dv2 / (float)Math.Abs(dy2);
            if (dy2 != 0) dw2_step = dw2 / (float)Math.Abs(dy2);

            if (dy1 != 0)
            {
                for (int i = y1; i <= y2; i++)
                {
                    int ax = x1 + (int)((i - y1) * dax_step);
                    int bx = x1 + (int)((i - y1) * dbx_step);

                    float tex_su = u1 + (i - y1) * du1_step;
                    float tex_sv = v1 + (i - y1) * dv1_step;
                    float tex_sw = w1 + (i - y1) * dw1_step;

                    float tex_eu = u1 + (i - y1) * du2_step;
                    float tex_ev = v1 + (i - y1) * dv2_step;
                    float tex_ew = w1 + (i - y1) * dw2_step;

                    if (ax > bx)
                    {
                        Swap(ref ax, ref bx);
                        Swap(ref tex_su, ref tex_eu);
                        Swap(ref tex_sv, ref tex_ev);
                        Swap(ref tex_sw, ref tex_ew);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;
                    tex_w = tex_sw;

                    float tstep = 1.0f / (bx - ax);
                    float t = 0.0f;

                    for (int j = ax; j < bx; j++)
                    {
                        tex_u = (1.0f - t) * tex_su + t * tex_eu;
                        tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                        tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                        int tex_x = (int)((tex_u / tex_w) * (texture.Width - 1));
                        int tex_y = (int)((tex_v / tex_w) * (texture.Height - 1));

                        DrawPixel(j, i, texture.GetPixelColor(tex_x, tex_y));

                        t += tstep;
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            dv1 = v3 - v2;
            du1 = u3 - u2;
            dw1 = w3 - w2;

            if (dy1 != 0) dax_step = dx1 / (float)Math.Abs(dy1);
            if (dy2 != 0) dbx_step = dx2 / (float)Math.Abs(dy2);

            du1_step = 0; dv1_step = 0;
            if (dy1 != 0) du1_step = du1 / (float)Math.Abs(dy1);
            if (dy1 != 0) dv1_step = dv1 / (float)Math.Abs(dy1);
            if (dy1 != 0) dw1_step = dw1 / (float)Math.Abs(dy1);

            if (dy1 != 0)
            {
                for (int i = y2; i <= y3; i++)
                {
                    int ax = x2 + (int)((i - y2) * dax_step);
                    int bx = x1 + (int)((i - y1) * dbx_step);

                    float tex_su = u2 + (i - y2) * du1_step;
                    float tex_sv = v2 + (i - y2) * dv1_step;
                    float tex_sw = w2 + (i - y2) * dw1_step;

                    float tex_eu = u1 + (i - y1) * du2_step;
                    float tex_ev = v1 + (i - y1) * dv2_step;
                    float tex_ew = w1 + (i - y1) * dw2_step;

                    if (ax > bx)
                    {
                        Swap(ref ax, ref bx);
                        Swap(ref tex_su, ref tex_eu);
                        Swap(ref tex_sv, ref tex_ev);
                        Swap(ref tex_sw, ref tex_ew);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;
                    tex_w = tex_sw;

                    float tstep = 1.0f / (bx - ax);
                    float t = 0.0f;

                    for (int j = ax; j < bx; j++)
                    {
                        tex_u = (1.0f - t) * tex_su + t * tex_eu;
                        tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                        tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                        int tex_x = (int)((tex_u / tex_w) * (texture.Width - 1));
                        int tex_y = (int)((tex_v / tex_w) * (texture.Height - 1));

                        DrawPixel(j, i, texture.GetPixelColor(tex_x, tex_y));

                        t += tstep;
                    }
                }
            }
        }

        public void DrawTexture(Texture texture, float x, float y)
        {            
            for (int j = 0; j < texture.Height; j++)
            {
                for (int i = 0; i < texture.Width; i++)
                {
                    int index = (j * texture.Width + i) * 4;
                    byte r = texture.Pixels[index];
                    byte g = texture.Pixels[index + 1];
                    byte b = texture.Pixels[index + 2];
                    byte a = texture.Pixels[index + 3];

                    DrawPixel((int)(x + i), (int)(y + j), new Color(r, g, b, a));
                }
            }
        }

        #endregion
        #endregion

        #region WinAPI
        #region Constants

        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);
        private const uint WM_DESTROY = 0x0002;
        private const uint WM_MOUSEMOVE = 0x0200; 
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint PM_REMOVE = 0x0001;
        private const int SW_SHOW = 5;  
        private const int SRCCOPY = 0x00CC0020;

        #endregion

        #region Delegates & Events

        private delegate IntPtr WndProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        // dont remove this, it will cause a crash
        private WndProc _wndProcDelegate;

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct WNDCLASS
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public uint bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        #endregion

        #region DLLs

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle, string lpClassName, string lpWindowName,
            uint dwStyle, int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr RegisterClass(ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern int StretchDIBits(IntPtr hdc, int xDest, int yDest, int DestWidth, int DestHeight, int xSrc, int ySrc, int SrcWidth, int SrcHeight, byte[] lpBits, ref BITMAPINFO lpBitsInfo, uint iUsage, uint dwRop);

        #endregion
        #endregion

    }
}
