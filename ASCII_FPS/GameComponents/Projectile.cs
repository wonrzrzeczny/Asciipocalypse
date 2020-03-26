using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS.GameComponents
{
	public class Projectile
	{
		private Vector3 direction;
		public MeshObject MeshObject { get; set; }
		private float speed;

		public Vector3 Position
		{
			get { return MeshObject.Position; }
		}

		public Projectile(Vector3 direction, float speed, MeshObject meshObject)
		{
			this.direction = Vector3.Normalize(direction);
			MeshObject = meshObject;
			this.speed = speed;
		}

		public void Update(GameTime gameTime)
		{
			MeshObject.Position += direction * speed * gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		}
	}
}
