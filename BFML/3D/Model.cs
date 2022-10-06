using System;
using System.Collections.Generic;

namespace ABOBAEngine.Rendering.Models;

public class Model
{
    public readonly float[] Vertices;
    public readonly uint[] Triangles;
    public float[] AlbedoMapUVs => OptionalData[AlbedoUVsKey];
    protected const byte AlbedoUVsKey = 0;
    public float[] NormalMapUVs => OptionalData[NormalUVsKey];
    protected const byte NormalUVsKey = 1;
    public float[] VertexNormalsUVs => OptionalData[VertexNormalsKey];
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
        private Model _model;

        public ModelBuilder(float[] vertices, uint[] triangles)
        {
            _model = new Model(vertices, triangles);
        }

        public Model Build()
        {
            return _model;
        }
        
        public ModelBuilder WithAlbedoUVs(float[] albedoUVs)
        {
            if (albedoUVs.Length % 2 != 0)
                throw new ArgumentException("Odd number of uv coordinates. Correct structure: [x,y,x,y,x,y...]");
            AddOrRewrite(AlbedoUVsKey, albedoUVs);
            return this;
        }

        private void AddOrRewrite(byte key, float[] value)
        {
            if (_model.OptionalData.ContainsKey(key))
            {
                _model.OptionalData[key] = value;
            }
            else
            {
                _model.OptionalData.Add(key, value);
            }
        }
    }
}