using Godot;

namespace Game
{
    public static class Marker
    {
        public static readonly Vector3[] VERTICES = new Vector3[] {
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3(0.5f, 0f, -0.5f),
            new Vector3(-0.5f, 0.15f, -0.5f),
            new Vector3(0.5f, 0.15f, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(-0.5f, 0.15f, 0.5f),
            new Vector3(0.5f, 0.15f, 0.5f),
        };

        public static readonly int[] LEFT = new int[] { 6, 4, 0, 2 };
        public static readonly int[] RIGHT = new int[] { 3, 1, 5, 7 };
        public static readonly int[] FRONT = new int[] { 7, 5, 4, 6 };
        public static readonly int[] BACK = new int[] { 2, 0, 1, 3 };

        public static readonly int[][] FACES = new int[][] {
            LEFT, RIGHT, FRONT, BACK,
        };

        public static Mesh CreateMesh(Material material)
        {
            var tool = new SurfaceTool();

            var mesh = new ArrayMesh();

            tool.Begin(Mesh.PrimitiveType.Triangles);

            foreach (var face in FACES)
            {
                var vtA = VERTICES[face[0]];

                var vtB = VERTICES[face[1]];

                var vtC = VERTICES[face[2]];

                var vtD = VERTICES[face[3]];

                var uvOffset = new Vector2(7f, 0f) / 16f;

                var height = 1.0f / 16f;

                var width = 1.0f / 16f;

                var uvA = uvOffset + new Vector2(0f, 0f);

                var uvB = uvOffset + new Vector2(0f, height);

                var uvC = uvOffset + new Vector2(width, height);

                var uvD = uvOffset + new Vector2(width, 0f);

                tool.AddTriangleFan(
                    new Vector3[] {
                        vtA,
                        vtB,
                        vtC,
                    },
                    new Vector2[] {
                        uvA,
                        uvB,
                        uvC,
                    }
                );

                tool.AddTriangleFan(
                    new Vector3[] {
                        vtA,
                        vtC,
                        vtD,
                    },
                    new Vector2[] {
                        uvA,
                        uvC,
                        uvD,
                    }
                );
            }


            tool.GenerateNormals();

            tool.SetMaterial(material);

            tool.Commit(mesh);

            return mesh;
        }
    }
}
