using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR

namespace Template_P3
{
    class Game
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        const float PI = 3.1415926535f;         // PI
        Stopwatch timer;                        // timer for measuring frame duration
        Shader shader;                          // shader to use for rendering
        Shader postproc;                        // shader to use for post processing
        Texture wood;                           // texture to use for rendering
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = true;

        float a;

        sceneGraph scene;
        public Vector3 camPos = new Vector3(0, -5, -20); //positie camera
        public int RotateX, RotateY, RotateZ; //rotatie in graden
        Matrix4 toWorld;

        // initialize
        public void Init()
        {
            scene = new sceneGraph();

            //load meshes met (id, filepath, positie, optionele parent id (default: ""))
            //scene.loadMesh("Teapot", "../../assets/teapot.obj", new Vector3(0, 0, 0));
            scene.loadMesh("Floor", "../../assets/floor.obj", new Vector3(0, 0, 0));
            scene.loadMesh("Car", "../../assets/car.obj", new Vector3(0, 0, 0));
            scene.loadMesh("wheels", "../../assets/wheel.obj", new Vector3(0, -0.2f, -1.3f), "Car");

            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../assets/wood.jpg");
            // create the render target
            target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            //set the light
            int lightID = GL.GetUniformLocation(shader.programID, "lightPos");
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            GL.UseProgram(shader.programID);
            GL.Uniform3(lightID, 5.0f, 12.0f, 5.0f);
            GL.Uniform3(ambientID, 0f, 0f, 0f);

        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // prepare matrix for vertex shader
            Matrix4 transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
            toWorld = transform;
            Matrix4 Rotation = Matrix4.CreateRotationY(RotateY * PI / 180) * Matrix4.CreateRotationX(RotateX * PI / 180) *
                Matrix4.CreateRotationZ(RotateZ * PI / 180);
            transform *= Rotation;
            transform *= Matrix4.CreateTranslation(camPos);

            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            if (useRenderTarget)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                scene.Render(shader, transform, toWorld, wood);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
            }
        }
    }

} // namespace Template_P3