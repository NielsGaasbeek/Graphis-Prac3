using System.Diagnostics;
using System;
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
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = true;

        sceneGraph scene;
        public Vector3 cameraPos;
        Vector4 cameraStart;
        public Matrix4 cameraMovs, toWorld;
        float a,b;
        int cPosID;

        // initialize
        public void Init()
        {
            scene = new sceneGraph();
            cameraPos = new Vector3(0f, -5f, -10f);
            cameraStart = new Vector4(cameraPos, 1f);
            //transform = Matrix4.CreateTranslation(cameraPos);
            //cameraMovs = Matrix4.CreateTranslation(cameraPos);
            cameraMovs = Matrix4.Identity;
            //Matrix4 test = Matrix4.CreateTranslation(new Vector3(1, 0, 0));

            //load meshes met (id, filepath, positie, texture filepath, optionele parent id (default: ""))
            //in het geval van een child is de positie t.o.v de parent
            scene.loadMesh("Teapot", "../../assets/teapot.obj", new Vector3(-7, 0, 0), "../../assets/wit.jpg");
            scene.loadMesh("Floor", "../../assets/floor.obj", new Vector3(0, 0, 0), "../../assets/wood.jpg");
            scene.loadMesh("Car", "../../assets/car.obj", new Vector3(5, 0, 0), "../../assets/wood.jpg", "Floor");
            scene.loadMesh("wheelsF", "../../assets/wheel.obj", new Vector3(0, -0.2f, -1.3f), "../../assets/wit.jpg", "Car");
            scene.loadMesh("wheelsR", "../../assets/wheel.obj", new Vector3(0, -0.2f, 2.75f), "../../assets/wit.jpg", "Car");

            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // create the render target
            target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            //set the light
            int lightID = GL.GetUniformLocation(shader.programID, "lightPos");
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            cPosID = GL.GetUniformLocation(shader.programID, "cameraPos");
            GL.UseProgram(shader.programID);
            GL.Uniform3(lightID, 3.0f, 5.0f, 3.0f);
            GL.Uniform3(ambientID, 0f, 0f, 0f);
            GL.Uniform4(cPosID, cameraStart);
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

            a += 0.01f * frameDuration;
            b += 0.001f * frameDuration;
            if (a > 2 * PI) { a -= 2 * PI; b -= 2 * PI; }

            // prepare matrix for vertex shader
            Matrix4 transform = Matrix4.Identity;
            //Matrix4 toWorld = Matrix4.Identity;
            transform = cameraMovs;
            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            Vector4 test2 = transform * cameraStart;
            Vector4 test = cameraMovs * cameraStart;
            GL.Uniform4(cPosID, cameraMovs * cameraStart);
            Console.WriteLine(test.X + " " + test.Y + " " + test.Z);

            //scene.children["Car"].modelMatrix = Matrix4.CreateRotationY(b);
            //scene.graph["Floor"].modelMatrix = Matrix4.CreateRotationX(b);
            scene.children["wheelsF"].modelMatrix = Matrix4.CreateRotationX(-a);
            scene.children["wheelsF"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(0, -0.2f, -1.3f));
            scene.children["wheelsR"].modelMatrix = Matrix4.CreateRotationX(-a);
            scene.children["wheelsR"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(0, -0.2f, 2.75f));


            if (useRenderTarget)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                scene.Render(shader, transform);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
        }
    }
} // namespace Template_P3