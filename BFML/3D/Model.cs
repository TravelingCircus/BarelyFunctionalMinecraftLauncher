using System;
using System.Collections.Generic;

namespace BFML._3D;

public class Model
{
    public float[] AlbedoMapUVs => OptionalData[AlbedoUVsKey];
    public float[] NormalMapUVs => OptionalData[NormalUVsKey];
    public float[] VertexNormalsUVs => OptionalData[VertexNormalsKey];
    
    public readonly uint[] Triangles;
    public readonly float[] Vertices;
    protected const byte AlbedoUVsKey = 0;
    protected const byte NormalUVsKey = 1;
    protected const byte VertexNormalsKey = 2;
    protected readonly Dictionary<byte, float[]> OptionalData;

    private Model(float[] vertices, uint[] triangles)
    {
        Vertices = vertices;
        Triangles = triangles;
        OptionalData = new Dictionary<byte, float[]>();
    }

    public static ModelBuilder FromMesh(float[] vertices, uint[] triangles)
    {
        return new ModelBuilder(vertices, triangles);
    }

    public class ModelBuilder
    {
        private readonly Model _model;

        public ModelBuilder(float[] vertices, uint[] triangles)
        {
            _model = new Model(vertices, triangles);
        }

        public Model Build() => _model;

        public ModelBuilder WithAlbedoUVs(float[] albedoUVs)
        {
            if (albedoUVs.Length % 2 != 0)
                throw new ArgumentException("Odd number of uv coordinates. Correct structure: [x,y,x,y,x,y...]");
            _model.OptionalData[AlbedoUVsKey] = albedoUVs;
            return this;
        }
    }
}