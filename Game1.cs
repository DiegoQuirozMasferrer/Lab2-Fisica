using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MotorFisico3D
{
    public class Game1 : Game
    {
        bool InMenu = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D whiteRectangle;
        SpriteFont Font;

        Matrix projection;
        Matrix view;

        VertexPositionColor[] verts;
        BasicEffect effect;
        VertexBuffer buffer;

        bool launched = false;
        Vector3 Gravity = new Vector3(0, -9.8f, 0);
        bool finished = false;

        public Vector3 BoxLimits = new Vector3(4, 4, 4);


        Vector3 position;
        float rotY;

        List<Esfera> pelotas = new List<Esfera>();
        public Esfera player;
        public bool isPDown;
        public bool Pause = true;

        Vector3 PosCamara = new Vector3(0, 0, -15);

        public Model cube;

        List<string> CollisionList = new List<string>();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);
            // for orthografic Matrix.CreateOrthographicOffCenter(-4, 4, -4/GraphicsDevice.Viewport.AspectRatio, 4/ GraphicsDevice.Viewport.AspectRatio, 0.1f, 2000);// 
            view = Matrix.CreateLookAt(PosCamara, Vector3.Zero, Vector3.Up);

            verts = new VertexPositionColor[8]
            {
                new VertexPositionColor(new Vector3(1, 1, 1), Color.Green),
                new VertexPositionColor(new Vector3(1, -1, 1), Color.Green),
                new VertexPositionColor(new Vector3(1, -1, -1), Color.Green),
                new VertexPositionColor(new Vector3(1, 1, -1), Color.Green),
                new VertexPositionColor(new Vector3(-1, 1, 1), Color.Green),
                new VertexPositionColor(new Vector3(-1, -1, 1), Color.Green),
                new VertexPositionColor(new Vector3(-1, -1, -1), Color.Green),
                new VertexPositionColor(new Vector3(-1, 1, -1), Color.Green),
            };
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position *= BoxLimits.X;
            }
            effect = new BasicEffect(GraphicsDevice);

            buffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, 8, BufferUsage.WriteOnly);
            buffer.SetData(verts);

            position = new Vector3(0, 0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Esfera(Content, new Vector3(0, 0, 0), .3f, "Player");
            pelotas.Add(player);
            pelotas.Add(new Esfera(Content, new Vector3(1, 0, 0), .5f, "Quietita"));
            pelotas.Add(new Esfera(Content, new Vector3(-1, 0, 0), .5f, "Botadora"));
            pelotas.Add(new Esfera(Content, new Vector3(1, 0, 1), .5f, "Quietita2"));
            pelotas[1].estatico = true;
            cube = Content.Load<Model>("CUBO");
            Font = Content.Load<SpriteFont>("File");
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here            
        }

        private void Start()
        {
            foreach(Esfera pelota in pelotas)
            {
                pelota.vel = new Vector3(0, 0, 0);
            }
            pelotas[0].pos = new Vector3(0, 0, 0);
            pelotas[1].pos = new Vector3(1, 0, 0);
            pelotas[2].pos = new Vector3(-1, 0, 0);
            pelotas[3].pos = new Vector3(0, 0, 1);

            verts = new VertexPositionColor[8]
            {
                new VertexPositionColor(new Vector3(1, 1, 1), Color.Green),
                new VertexPositionColor(new Vector3(1, -1, 1), Color.Green),
                new VertexPositionColor(new Vector3(1, -1, -1), Color.Green),
                new VertexPositionColor(new Vector3(1, 1, -1), Color.Green),
                new VertexPositionColor(new Vector3(-1, 1, 1), Color.Green),
                new VertexPositionColor(new Vector3(-1, -1, 1), Color.Green),
                new VertexPositionColor(new Vector3(-1, -1, -1), Color.Green),
                new VertexPositionColor(new Vector3(-1, 1, -1), Color.Green),
            };
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position *= BoxLimits.X;
            }
        }

        public void CollisionSolver()
        {
            for (int i = 0; i < pelotas.Count; i++)
            {
                Vector3 posi = pelotas[i].pos;
                float radioi = pelotas[i].radio;
                //Chocar contra murallas
                if (pelotas[i].vel.X > 0 && posi.X + radioi > BoxLimits.X || pelotas[i].vel.X < 0 && posi.X - radioi < -BoxLimits.X)
                {
                    pelotas[i].vel.X *= -1;
                }
                if (pelotas[i].vel.Y > 0 && posi.Y + radioi > BoxLimits.Y || pelotas[i].vel.Y < 0 && posi.Y - radioi < -BoxLimits.Y)
                {
                    if (pelotas[i] == player)
                    {
                        Debug.WriteLine("\n");
                        Debug.WriteLine("Punto de colision: " + pelotas[i].pos.X + " " + pelotas[i].pos.Y + pelotas[i].radio + " " + pelotas[i].pos.Z);
                        Pause = true;
                        Debug.WriteLine("Colisiones:");
                        foreach (string s in CollisionList)
                        {
                            Debug.WriteLine(s);
                        }
                        CollisionList = new List<string>();
                    }
                    pelotas[i].vel.Y *= -1;
                }
                if (pelotas[i].vel.Z > 0 && posi.Z + radioi > BoxLimits.Z || pelotas[i].vel.Z < 0 && posi.Z - radioi < -BoxLimits.Z)
                {
                    pelotas[i].vel.Z *= -1;
                }
                //chocar con otras esferas
                for (int j = i + 1; j < pelotas.Count; j++)
                {
                    pelotas[i].color = Color.White;
                    pelotas[j].color = Color.White;
                    if (Vector3.DistanceSquared(pelotas[i].pos, pelotas[j].pos) < (pelotas[i].radio + pelotas[j].radio) * (pelotas[i].radio + pelotas[j].radio))
                    {
                        Debug.WriteLine("Distancia:" + Vector3.DistanceSquared(pelotas[i].pos, pelotas[j].pos));
                        pelotas[i].color = Color.Red;
                        pelotas[j].color = Color.Red;
                        collide(pelotas[i], pelotas[j]);
                        collide(pelotas[j], pelotas[i]);
                    }
                }
            }
        }
        public void collide(Esfera pelotasA, Esfera pelotasB)
        {
            bool TrueCollision = false;
            float radioi = pelotasA.radio;
            float radioj = pelotasB.radio;
            Vector3 posi = pelotasA.pos;
            if ((pelotasA.vel.X > 0 && posi.X + radioi > pelotasB.pos.X - radioj && posi.X + radioi < pelotasB.pos.X)
                || (pelotasA.vel.X < 0 && posi.X - radioi < pelotasB.pos.X + radioj && posi.X - radioi > pelotasB.pos.X))
            {
                TrueCollision = true;
                pelotasB.vel.X += pelotasA.vel.X * 0.5f;
                pelotasA.vel.X *= -0.5f;
            }
            if ((pelotasA.vel.Y > 0 && posi.Y + radioi > pelotasB.pos.Y - radioj && posi.Y + radioi < pelotasB.pos.Y)
                || (pelotasA.vel.Y < 0 && posi.Y - radioi < pelotasB.pos.Y + radioj && posi.Y - radioi > pelotasB.pos.Y)
                )
            {
                TrueCollision = true;
                pelotasB.vel.Y += pelotasA.vel.Y * 0.5f;
                pelotasA.vel.Y *= -0.5f;
            }
            if ((pelotasA.vel.Z > 0 && posi.Z + radioi > pelotasB.pos.Z - radioj && posi.Z + radioi < pelotasB.pos.Z)
                || (pelotasA.vel.Z < 0 && posi.Z - radioi < pelotasB.pos.Z + radioj && posi.Z - radioi > pelotasB.pos.Z)
                )
            {
                TrueCollision = true;
                pelotasB.vel.Z += pelotasA.vel.Z * 0.5f;
                pelotasA.vel.Z *= -0.5f;
            }
            if (TrueCollision)
            {
                if (pelotasA == player)
                {
                    CollisionList.Add(pelotasB.name);
                }
                else if (pelotasB == player)
                {
                    CollisionList.Add(pelotasA.name);
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (!InMenu)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.O))
                {
                    InMenu = true;
                }

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                view = Matrix.CreateLookAt(PosCamara, player.pos, Vector3.Up);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                if (Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    if (!isPDown)
                    {
                        isPDown = true;
                        Pause = !Pause;
                    }
                }
                else
                {
                    isPDown = false;
                }
                if (Pause)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.X))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            player.vel.X -= deltaTime * 10;
                        }
                        else
                        {
                            player.vel.X += deltaTime * 10;

                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Y))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            player.vel.Y -= deltaTime * 10;
                        }
                        else
                        {
                            player.vel.Y += deltaTime * 10;

                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Z))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            player.vel.Z -= deltaTime * 10;
                        }
                        else
                        {
                            player.vel.Z += deltaTime * 10;

                        }
                    }
                    return;
                }


                rotY += deltaTime;

                float vel = 0.2f;

                if (finished)
                {
                    return;
                }
                Vector3 DirCameraPlayer = (player.pos - PosCamara);
                DirCameraPlayer.Normalize();
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {

                    PosCamara -= DirCameraPlayer * deltaTime * 20;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    PosCamara += DirCameraPlayer * deltaTime * 20;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    float rotDir = 1;
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        rotDir = -1;
                    }
                    Vector2 temPos = new Vector2(PosCamara.X, PosCamara.Z);
                    temPos = Rotar(temPos, rotDir * deltaTime * 100);
                    PosCamara.X = temPos.X;
                    PosCamara.Z = temPos.Y;

                }

                foreach (Esfera e in pelotas)
                {
                    e.vel += Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    e.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                CollisionSolver();
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Start();
                    InMenu = false;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Dibuja cubo

            _spriteBatch.Begin();
            if(!InMenu)
            {
                // TODO: Add your drawing code here
                //GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                effect.Projection = projection;
                effect.View = view;
                effect.World = Matrix.Identity * Matrix.CreateRotationY(0) * Matrix.CreateTranslation(position);
                effect.VertexColorEnabled = true;

                List<VertexPositionColor[]> Vertices = new List<VertexPositionColor[]>();

                Vertices.Add(new VertexPositionColor[2] { verts[0], verts[1] });
                Vertices.Add(new VertexPositionColor[2] { verts[1], verts[2] });
                Vertices.Add(new VertexPositionColor[2] { verts[2], verts[3] });
                Vertices.Add(new VertexPositionColor[2] { verts[3], verts[0] });
                Vertices.Add(new VertexPositionColor[2] { verts[4], verts[5] });
                Vertices.Add(new VertexPositionColor[2] { verts[5], verts[6] });
                Vertices.Add(new VertexPositionColor[2] { verts[6], verts[7] });
                Vertices.Add(new VertexPositionColor[2] { verts[7], verts[4] });
                Vertices.Add(new VertexPositionColor[2] { verts[0], verts[4] });
                Vertices.Add(new VertexPositionColor[2] { verts[1], verts[5] });
                Vertices.Add(new VertexPositionColor[2] { verts[2], verts[6] });
                Vertices.Add(new VertexPositionColor[2] { verts[3], verts[7] });

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    foreach (VertexPositionColor[] vc in Vertices)
                    {
                        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vc, 0, 1);
                    }
                }

                foreach (Esfera e in pelotas)
                {
                    e.Draw(view, projection);
                }


                _spriteBatch.Draw(whiteRectangle, new Rectangle(0, 0, 250, 120), Color.White);
                _spriteBatch.DrawString(Font, "Velocidad:", new Vector2(10, 5), Color.Black);
                _spriteBatch.DrawString(Font, "X: " + player.vel.X + "\nY: " + player.vel.Y + "\nZ: " + player.vel.Z, new Vector2(10, 20), Color.Black);
                _spriteBatch.DrawString(Font, "Magnitud: " + player.vel.Length(), new Vector2(10, 90), Color.Black);
                Vector3 aux = player.vel;
                aux.Normalize();
                _spriteBatch.DrawString(Font, "Vector unitario:", new Vector2(120, 5), Color.Black);
                _spriteBatch.DrawString(Font, "X: " + aux.X + "\nY: " + aux.Y + "\nZ: " + aux.Z, new Vector2(120, 20), Color.Black);
            }
            else
            {
                _spriteBatch.DrawString(Font, "EQUIPO (salon 14):\nSimon Grossermann\nDiego Quiroz\nRodrigo Sepulveda\n\nControles:\nP: Pausar/Despausar la simulacion\nZ/X/Y: Aplicar velocidad en el respectivo eje\nShift: Cambiar la orientacion de la velocidad que se aplica (usar con el de arriba)\nO: Volver al menu\nSpace: Comenzar", new Vector2(100, 100), Color.White);
            }


            //_spriteBatch.Draw(whiteRectangle, new Rectangle(0, 360, 250, 120), Color.White);
            //CollisionList

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        public static Vector2 Rotar(Vector2 v, float degrees)
        {
            degrees = (degrees / 360) * MathF.PI * 2;
            float sin = MathF.Sin(degrees);
            float cos = MathF.Cos(degrees);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}
