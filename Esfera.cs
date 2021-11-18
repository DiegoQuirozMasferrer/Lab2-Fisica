using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotorFisico3D
{
    public class Esfera
    {
        public string name = "";
        public Vector3 pos;
        public Vector3 vel;
        private Vector3 lastVel;
        public float radio;
        public float radioSqr { get { return radio * radio; } }
        public static Model ball;
        public Color color;
        public bool estatico;

        public Esfera(ContentManager Content, Vector3 posInicial, float radio, string name)
        {
            this.name = name;
            color = Color.White;
            if (ball == null)
            {
                ball = Content.Load<Model>("sphere");
            }
            pos = posInicial;
            this.radio = radio;
        }        
        public void Update(float deltaTime)
        {
            if (estatico)
            {
                return;
            }
            if ((lastVel + vel).LengthSquared() <= 0.01f)
            {
                pos += vel * deltaTime;
            }
            else
            {
                pos += (lastVel + vel) / 2 * deltaTime;
            }
            lastVel = vel;
        }
        public void Draw(Matrix view, Matrix projection)
        {
            foreach (var mesh in ball.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = Matrix.CreateRotationY(MathHelper.Pi / 2) * Matrix.CreateRotationZ(MathHelper.Pi / 2)* Matrix.CreateScale(radio) * Matrix.CreateTranslation(pos);
                    effect.EnableDefaultLighting();
                    //effect.TextureEnabled = true;
                    effect.DiffuseColor = color.ToVector3();
                    effect.AmbientLightColor = Color.White.ToVector3();
                    effect.Alpha = 1f;
                }
                mesh.Draw();
            }
        }
    }
}
