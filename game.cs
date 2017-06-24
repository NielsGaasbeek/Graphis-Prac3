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

        Vector4 lightPos;

        public Matrix4 camMatrix;
        Matrix4 test;

        int cPosID;
        int lightID;

        // initialize
        public void Init()
        {
            scene = new sceneGraph();
            Vector4 cameraStart = new Vector4(0f, -5f, -10f, 1f);
            lightPos = new Vector4(0.0f, 10.0f, 0.0f, 1f);

            camMatrix =  Matrix4.CreateTranslation(cameraStart.Xyz);
            test = Matrix4.CreateTranslation(cameraStart.Xyz);

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
            lightID = GL.GetUniformLocation(shader.programID, "lightPos");
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            cPosID = GL.GetUniformLocation(shader.programID, "cameraPos");

            GL.UseProgram(shader.programID);

            GL.Uniform3(lightID, lightPos.Xyz);
            GL.Uniform3(ambientID, 0f, 0f, 0f);
            //GL.Uniform4(cPosID, cameraStart);
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
        }


        float a,b;

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

            Matrix4 transform = camMatrix;
            Matrix4 toWorld = transform;

            Vector4 cameraPos = new Vector4(0f,0f,0f,1f) * Matrix4.Invert(camMatrix);
            GL.Uniform3(cPosID, cameraPos.Xyz);

            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            lightPos = new Vector4(0.0f, 10.0f, 0.0f, 1f) * camMatrix;
            GL.Uniform3(lightID, lightPos.Xyz);

            //Console.WriteLine(lightPos.X + " " + lightPos.Y + " " + lightPos.Z);

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
                scene.Render(shader, transform, toWorld);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
        }
    }
} // namespace Template_P3