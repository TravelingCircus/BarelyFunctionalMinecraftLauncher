using System;
using System.Collections.Generic;
using System.IO;
using ABOBAEngine.Utils;

namespace ABOBAEngine.Rendering.Models;

public sealed class ObjModelLoader : ModelLoader
{
    private const string FileExtension = ".obj";

    private ObjLineParser[] _parsers =
    {
        new VertexObjLineParser(),
        new AlbedoUVObjLineParser(),
        new FaceObjLineParser()
    };

    public ObjModelLoader(string path) : base(path)
    {
    }

    protected override bool IsValidFile(string path)
    {
        return path.EndsWith(FileExtension) && File.Exists(path);
    }

    public override Model Load()
    {
        UnfinishedObjData rawData = ReadRawData();

        SortVertexData(rawData);

        return Model.FromMesh(
                rawData.Vertices,
                rawData.VertexIndices)
            .WithAlbedoUVs(rawData.AlbedoUVs)
            .Build();
    }

    private UnfinishedObjData ReadRawData()
    {
        using LineByLineReader reader = new LineByLineReader(GetStream());
        while (!reader.IsEmpty)
        {
            ReadOnlyMemory<char> line = reader.ReadLine();
            for (int i = 0; i < _parsers.Length; i++)
            {
                if (_parsers[i].LineFits(line))
                {
                    _parsers[i].Parse(line);
                    break;
                }
            }
        }

        float[] vertices = ((VertexObjLineParser)_parsers[0]).Vertices.ToArray();
        float[] albedoUVs = ((AlbedoUVObjLineParser)_parsers[1]).AlbedoUVs.ToArray();
        uint[] vertexIndices = ((FaceObjLineParser)_parsers[2]).VertexIndices.ToArray();
        uint[] normalIndices = ((FaceObjLineParser)_parsers[2]).NormalIndices.ToArray();
        uint[] uvIndices = ((FaceObjLineParser)_parsers[2]).UVIndices.ToArray();
        return new UnfinishedObjData(vertices, albedoUVs, vertexIndices, normalIndices, uvIndices);
    }

    private void SortVertexData(UnfinishedObjData data)
    {
        List<float> uvs = new List<float>();
        List<float> vertices = new List<float>();
        for (int i = 0; i < data.VertexIndices.Length; i++)
        {
            uint uvIndex = data.UVIndices[i];
            uvs.Add(data.AlbedoUVs[2 * uvIndex]);
            uvs.Add(data.AlbedoUVs[2 * uvIndex + 1]);

            uint vertexIndex = data.VertexIndices[i];
            vertices.Add(data.Vertices[3*vertexIndex]);
            vertices.Add(data.Vertices[3*vertexIndex+1]);
            vertices.Add(data.Vertices[3*vertexIndex+2]);
        }

        data.Vertices = vertices.ToArray();
        data.AlbedoUVs = uvs.ToArray();
    }

    private class UnfinishedObjData
    {
        public float[] Vertices;
        public float[] AlbedoUVs;
        public uint[] VertexIndices;
        public uint[] NormalIndices;
        public uint[] UVIndices;

        public UnfinishedObjData(float[] vertices, float[] albedoUVs, uint[] triangles, uint[] normalIndices,
            uint[] uvIndices)
        {
            Vertices = vertices;
            AlbedoUVs = albedoUVs;
            VertexIndices = triangles;
            NormalIndices = normalIndices;
            UVIndices = uvIndices;
        }
    }
}