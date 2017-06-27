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

        public Matrix4 camMatrix;
        int cPosID;

        // initialize
        public void Init()
        {
            scene = new sceneGraph();

            //load meshes met (id, filepath, positie, texture filepath, optionele parent id (default: ""))
            //in het geval van een child is de positie t.o.v de parent

            scene.loadMesh("Floor", "../../assets/floor.obj", new Vector3(0, 0, 0), "../../assets/wit.jpg");
            scene.loadMesh("Sun", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/sun.jpg");

            scene.loadMesh("Mercury", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/Mercury.jpg", "Sun");
            scene.loadMesh("Venus", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/Venus.jpg", "Sun");
            scene.loadMesh("Earth", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/earth.jpg", "Sun");
            scene.loadMesh("Moon", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/moon.jpg", "Earth");
            scene.loadMesh("Mars", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/mars.jpg", "Sun");
            scene.loadMesh("Jupiter", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/jupiter.jpg", "Sun");
            scene.loadMesh("Saturn", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/saturn.jpg", "Sun");
            scene.loadMesh("Uranus", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/uranus.jpg", "Sun");
            scene.loadMesh("Neptune", "../../assets/sphere.obj", new Vector3(0, 0, 0), "../../assets/neptune.jpg", "Sun");


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

            //base transformation to the camera-matrix, to set the camera in the start-position
            camMatrix = Matrix4.CreateTranslation(new Vector3(0, -5f, -10f));

            //set the light-position, camera-position and ambient-color
            int lightID = GL.GetUniformLocation(shader.programID, "lightPos");
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            cPosID = GL.GetUniformLocation(shader.programID, "cameraPos");

            GL.UseProgram(shader.programID);
            GL.Uniform3(lightID, new Vector3(0.0f, 0.0f, 0.0f));
            GL.Uniform3(ambientID, 0f, 0f, 0f);
            GL.Uniform4(cPosID, new Vector4(0f, -5f, -10f, 1f));
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);

            //transformaties van meshes
            scene.graph["Floor"].modelMatrix = Matrix4.CreateScale(7.0f);
            scene.graph["Sun"].modelMatrix = Matrix4.CreateRotationY(b);

            scene.children["Mercury"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Mercury"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(20, 0, 0));
            scene.children["Mercury"].modelMatrix *= Matrix4.CreateScale(0.15f);

            scene.children["Venus"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Venus"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(20, 0, -20));
            scene.children["Venus"].modelMatrix *= Matrix4.CreateScale(0.25f);

            scene.children["Earth"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Earth"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(0, 0, 25));
            scene.children["Earth"].modelMatrix *= Matrix4.CreateScale(0.35f);

            scene.children["Moon"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Moon"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(-15, 0, 0));
            scene.children["Moon"].modelMatrix *= Matrix4.CreateScale(0.3f);

            scene.children["Mars"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Mars"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(-40, 0, -40));
            scene.children["Mars"].modelMatrix *= Matrix4.CreateScale(0.3f);

            scene.children["Jupiter"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Jupiter"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(0, 0, -20));
            scene.children["Jupiter"].modelMatrix *= Matrix4.CreateScale(0.9f);

            scene.children["Saturn"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Saturn"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(35, 0, 0));
            scene.children["Saturn"].modelMatrix *= Matrix4.CreateScale(0.7f);


            scene.children["Uranus"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Uranus"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(60, 0, 60));
            scene.children["Uranus"].modelMatrix *= Matrix4.CreateScale(0.5f);

            scene.children["Neptune"].modelMatrix = Matrix4.CreateRotationY(b);
            scene.children["Neptune"].modelMatrix *= Matrix4.CreateTranslation(new Vector3(-60, 0, 60));
            scene.children["Neptune"].modelMatrix *= Matrix4.CreateScale(0.55f);

        }

        float b;


        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            b += 0.001f * frameDuration;
            if (b > 2 * PI) { b -= 2 * PI; }

            // prepare matrix for vertex shader
            Matrix4 transform = camMatrix;
            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);


            //set the position of the camera for specular calculations
            GL.UseProgram(shader.programID);
            Vector4 cameraPos = new Vector4(0f, 0f, 0f, 1f) * Matrix4.Invert(camMatrix);
            GL.Uniform3(cPosID, cameraPos.Xyz);


            if (useRenderTarget)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                scene.Render(shader, transform, Matrix4.Identity);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
        }
    }
} // namespace Template_P3