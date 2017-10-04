using UnityEngine;

namespace Unfold
{
    public static class SubDivider
    {

        public static SmartTriangle[] SubDivideTriangle(TriangleData triangle, MeshData targetData, int subdivisionNumber)
        {
            var tds = new TriangleData[]{new TriangleData(), new TriangleData(), new TriangleData(), new TriangleData()  };

            SubDivideTriangles(ref tds);
            SubDivideVertices(triangle, ref tds);
            SubDivideNormals(triangle, ref tds);
            SubDivideUvs(triangle, ref tds);

            var sts = new SmartTriangle[4];

            for (int i = 0; i < sts.Length; i++)
            {
                sts[i] = new SmartTriangle(tds[i], targetData, subdivisionNumber);
            }

            return sts;
        }

        private static void SubDivideTriangles(ref TriangleData[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i].T0 = i*3;
                target[i].T1 = i*3 + 1;
                target[i].T2 = i*3 + 2;
            }
        }

        private static void SubDivideVertices(TriangleData source, ref TriangleData[] target)
        {
            var v0 = source.V0;
            var v1 = source.V1;
            var v2 = source.V2;
            var va = (v0 + v1) / 2.0f;
            var vb = (v1 + v2) / 2.0f;
            var vc = (v2 + v0) / 2.0f;

            target[0].V0 = v0;
            target[0].V1 = va;
            target[0].V2 = vc;

            target[1].V0 = vc;
            target[1].V1 = vb;
            target[1].V2 = v2;

            target[2].V0 = va;
            target[2].V1 = vb;
            target[2].V2 = vc;

            target[3].V0 = va;
            target[3].V1 = v1;
            target[3].V2 = vb;
        }

        private static void SubDivideNormals(TriangleData source, ref TriangleData[] target)
        {
            var n0 = source.N0;
            var n1 = source.N1;
            var n2 = source.N2;
            var na = (n0 + n1) / 2.0f;
            var nb = (n1 + n2) / 2.0f;
            var nc = (n2 + n0) / 2.0f;

            target[0].N0 = n0;
            target[0].N1 = na;
            target[0].N2 = nc;

            target[1].N0 = nc;
            target[1].N1 = nb;
            target[1].N2 = n2;

            target[2].N0 = na;
            target[2].N1 = nb;
            target[2].N2 = nc;

            target[3].N0 = na;
            target[3].N1 = n1;
            target[3].N2 = nb;
        }

        private static void SubDivideUvs(TriangleData source, ref TriangleData[] target)
        {
            var uv0 = source.Uv0;
            var uv1 = source.Uv1;
            var uv2 = source.Uv2;
            var uva = (uv0 + uv1) / 2.0f;
            var uvb = (uv1 + uv2) / 2.0f;
            var uvc = (uv2 + uv0) / 2.0f;

            target[0].Uv0 = uv0;
            target[0].Uv1 = uva;
            target[0].Uv2 = uvc;

            target[1].Uv0 = uvc;
            target[1].Uv1 = uvb;
            target[1].Uv2 = uv2;

            target[2].Uv0 = uva;
            target[2].Uv1 = uvb;
            target[2].Uv2 = uvc;

            target[3].Uv0 = uva;
            target[3].Uv1 = uv1;
            target[3].Uv2 = uvb;
        }

        public static bool TheSame(this TriangleVertices a, TriangleVertices b, float delta)
        {
            var sqrDelta = delta * delta;
            return (a.V0 - b.V0).sqrMagnitude < sqrDelta && (a.V1 - b.V1).sqrMagnitude < sqrDelta && (a.V2 - b.V2).sqrMagnitude < sqrDelta;
        }
    }
}
