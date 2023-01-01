using Godot;

namespace Game
{
    public static class Voxel
    {
        public static readonly Vector3[] VERTICES = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        };

        public static readonly int[] TOP = new int[] { 2, 3, 7, 6 };
        public static readonly int[] BOTTOM = new int[] { 0, 4, 5, 1 };
        public static readonly int[] LEFT = new int[] { 6, 4, 0, 2 };
        public static readonly int[] RIGHT = new int[] { 3, 1, 5, 7 };
        public static readonly int[] FRONT = new int[] { 7, 5, 4, 6 };
        public static readonly int[] BACK = new int[] { 2, 0, 1, 3 };

        public static readonly int[][] FACES = new int[][] {
            TOP, BOTTOM, LEFT, RIGHT, FRONT, BACK,
        };

        public static (Vector3, Vector3, Vector3, Vector3) GetVertices(int[] face, float x, float y, float z)
        {
            var position = new Vector3(x, y, z);

            var a = VERTICES[face[0]] + position;
            var b = VERTICES[face[1]] + position;
            var c = VERTICES[face[2]] + position;
            var d = VERTICES[face[3]] + position;

            return (a, b, c, d);
        }

        public static Mesh CreateMesh()
        {
            var surfaceTool = new SurfaceTool();

            var mesh = new ArrayMesh();

            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            for (int y = 0; y < 1; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    for (int z = 0; z < 32; z++)
                    {
                        foreach (var face in FACES)
                        {
                            var (a, b, c, d) = GetVertices(face, x, y, z);

                            surfaceTool.AddTriangleFan(new Vector3[] {
                                a,
                                b,
                                c,
                            });

                            surfaceTool.AddTriangleFan(new Vector3[] {
                                a,
                                c,
                                d,
                            });
                        }
                    }
                }
            }

            surfaceTool.GenerateNormals();

            surfaceTool.Commit(mesh);

            return mesh;
        }
    }
}
