using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer11 = SharpDX.Direct3D11.Buffer;
using Template.Graphics;

namespace Template
{
    /// <summary>
    /// 3D object with mesh.
    /// </summary>
    class MeshObject : Game3DObject, IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct VertexDataStruct
        {
            public Vector4 position;
            public Vector4 normal;
            public Vector4 color;
            public Vector2 texCoord0;
            public Vector2 texCoord1;
        }

        private string _name;
        public string Name { get => _name; }
        private int _index;
        public int Index { get => _index; set => _index = value; }

        private DirectX3DGraphics _directX3DGraphics;

        /// <summary>Renderer object.</summary>
        private Renderer _renderer;

        #region Vertices and Indexes
        /// <summary>Count of object vertices.</summary>
        private int _verticesCount;

        /// <summary>Array of vertex data.</summary>
        private VertexDataStruct[] _vertices;
        /// <summary>Array of vertex data.</summary>
        public List<VertexDataStruct> Vertices { get => _vertices.ToList(); }

        public override Vector4 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _centerPosition = _position;
            }
        }

        /// <summary>Center position of the mesh</summary>
        private Vector4 _centerPosition;
        /// <summary>Center point of the mesh</summary>
        public Vector4 CenterPosition { get => _centerPosition; set => _centerPosition = value; }

        /// <summary>Center position of the mesh</summary>
        private Vector4 _center2Position;
        /// <summary>Center point of the mesh</summary>
        public Vector4 Center2Position { get => _center2Position; set => _center2Position = value; }

        /// <summary>Vertex buffer DirectX object.</summary>
        private Buffer11 _vertexBufferObject;

        private VertexBufferBinding _vertexBufferBinding;

        /// <summary>Count of object vertex Indexes.</summary>
        private int _indexesCount;

        /// <summary>Array of object vertex indexes.</summary>
        private uint[] _indexes;

        private Buffer11 _indexBufferObject;
        #endregion

        private Material _material;

        public Material Material { get => _material; set => _material = value; }
        

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="renderer">Renderer object.</param>
        /// <param name="initialPosition">Initial position in 3d scene.</param>
        /// <param name="yaw">Initial angle of rotation around 0Y axis (x - to left, y - to up, z - to back), rad.</param>
        /// <param name="pitch">Initial angle of rotation around 0X axis (x - to left, y - to up, z - to back), rad.</param>
        /// <param name="roll">Initial rotation around 0Z axis (x - to left, y - to up, z - to back), rad.</param>
        /// <param name="vertices">Array of vertex data.</param>
        public MeshObject(string name, DirectX3DGraphics directX3DGraphics, Renderer renderer,
            Vector4 initialPosition, float yaw, float pitch, float roll,
            VertexDataStruct[] vertices, uint[] indexes, Material material) :
            base(initialPosition, yaw, pitch, roll)
        {
            _name = name;
            _directX3DGraphics = directX3DGraphics;
            _renderer = renderer;
            if (null != vertices)
            {
                _vertices = vertices;
                _verticesCount = _vertices.Length;

                List<VertexDataStruct> vrts = _vertices.ToList();

                float s = 0.0f; vrts.ForEach(v => s += v.position.X);
                float x = s / vrts.Count;
                s = 0.0f; vrts.ForEach(v => s += v.position.Y);
                float y = s / vrts.Count;
                s = 0.0f; vrts.ForEach(v => s += v.position.Z);
                float z = s / vrts.Count;

                _centerPosition = new Vector4(x, y, z, 0.0f); // Определение геометрического центра фигуры

                if (_name.Contains("wheel"))
                {
                    ToPosition();
                    _center2Position = _position;
                    _position = _centerPosition;
                }
                else _center2Position = _centerPosition;

            }
            if (null != indexes)
            {
                _indexes = indexes;
                _indexesCount = _indexes.Length;
            } else
            {
                _indexesCount = _verticesCount;
                _indexes = new uint[_indexesCount];
                for (int i = 0; i <= _indexesCount; ++i) _indexes[i] = (uint)i;
            }
            _material = material;
            
            _vertexBufferObject = Buffer11.Create(_directX3DGraphics.Device, BindFlags.VertexBuffer, _vertices, Utilities.SizeOf<VertexDataStruct>() * _verticesCount);
            _vertexBufferBinding = new VertexBufferBinding(_vertexBufferObject, Utilities.SizeOf<VertexDataStruct>(), 0);
            _indexBufferObject = Buffer11.Create(_directX3DGraphics.Device, BindFlags.IndexBuffer, _indexes, Utilities.SizeOf<int>() * _indexesCount);
        }

        public override void MoveBy(float dX, float dY, float dZ)
        {
            base.MoveBy(dX, dY, dZ);
            _centerPosition.X += dX;
            _centerPosition.Y += dY;
            _centerPosition.Z += dZ;

            _center2Position.X += dX;
            _center2Position.Y += dY;
            _center2Position.Z += dZ;
        }

        public void MoveBy(Vector4 dv)
        {
            MoveBy(dv.X, dv.Y, dv.Z);
        }

        public override void MoveTo(float x, float y, float z)
        {
            float X = x - _position.X;
            float Y = y - _position.Y;
            float Z = z - _position.Z;

            base.MoveTo(x, y, z);
            _centerPosition.X += X;
            _centerPosition.Y += Y;
            _centerPosition.Z += Z;

            _center2Position.X += X;
            _center2Position.Y += Y;
            _center2Position.Z += Z;
        }

        public void MoveTo(Vector4 vector)
        {
            MoveTo(vector.X, vector.Y, vector.Z);
        }

        //public override void PitchBy(float deltaPitch)
        //{
        //    base.PitchBy(deltaPitch);

        //    Vector3 axis = new Vector3(1, 0, 0);
        //    _center2Position = Vector4.Transform(_center2Position, Matrix.RotationAxis(axis, deltaPitch));
        //}

        //public override void YawBy(float deltaYaw)
        //{
        //    base.YawBy(deltaYaw);

        //    Vector3 axis = new Vector3(0, 1, 0);
        //    _center2Position = Vector4.Transform(_center2Position, Matrix.RotationAxis(axis, deltaYaw));
        //}

        //public override void RollBy(float deltaRoll)
        //{
        //    base.RollBy(deltaRoll);

        //    Vector3 axis = new Vector3(0, 0, 1);
        //    _center2Position = Vector4.Transform(_center2Position, Matrix.RotationAxis(axis, deltaRoll));
        //}

        private void ToPosition()
        {
            Vector4[] vertices = new Vector4[_verticesCount];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = _vertices[i].position - _centerPosition;
                _vertices[i].position = _position + vertices[i];
            }
        }

        private void ToCenter()
        {
            Vector4[] vertices = new Vector4[_verticesCount];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = _vertices[i].position - _position;
                _vertices[i].position = _centerPosition + vertices[i];
            }
        }

        public virtual void Render(Matrix viewMatrix, Matrix projectionMatrix)
        {
            _renderer.UpdatePerObjectConstantBuffer(0, GetWorldMatrix(), viewMatrix, projectionMatrix);

            DeviceContext deviceContext = _directX3DGraphics.DeviceContext;
            _renderer.UpdateMaterialProperties(_material);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            deviceContext.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);
            deviceContext.InputAssembler.SetIndexBuffer(_indexBufferObject, Format.R32_UInt, 0);
            deviceContext.DrawIndexed(_indexesCount, 0, 0);
        }

        public void Dispose()
        {
            Utilities.Dispose(ref _indexBufferObject);
            Utilities.Dispose(ref _vertexBufferObject);
        }
        
        private void Rotate(Matrix m)
        {
            //float sin = (float)Math.Sin(angle);
            //float cos = (float)Math.Cos(angle);
            

            for (int v = 0; v < _verticesCount; v++)
            {
                float x = _vertices[v].position.X;
                float y = _vertices[v].position.Y;
                float z = _vertices[v].position.Z;
                float w = _vertices[v].position.W;

                //_vertices[v].position.X = m.M11 * x + m.M21 * y + m.M31 * z + m.M41 * w;
                //_vertices[v].position.Y = m.M12 * x + m.M22 * y + m.M32 * z + m.M42 * w;
                //_vertices[v].position.Z = m.M13 * x + m.M23 * y + m.M33 * z + m.M43 * w;
                //_vertices[v].position.W = m.M14 * x + m.M24 * y + m.M34 * z + m.M44 * w;

                _vertices[v].position.X = m.M11 * x + m.M12 * y + m.M13 * z + m.M14 * w;
                _vertices[v].position.Y = m.M21 * x + m.M22 * y + m.M23 * z + m.M24 * w;
                _vertices[v].position.Z = m.M31 * x + m.M32 * y + m.M33 * z + m.M34 * w;
                _vertices[v].position.W = m.M41 * x + m.M42 * y + m.M43 * z + m.M44 * w;

                //v.X = m.M11 * v.X + m.M21 * v.Y + m.M31 * v.Z + m.M41 * v.W;
                //v.Y = m.M12 * v.X + m.M22 * v.Y + m.M32 * v.Z + m.M42 * v.W;
                //v.Z = m.M13 * v.X + m.M23 * v.Y + m.M33 * v.Z + m.M43 * v.W;
                //v.W = m.M14 * v.X + m.M24 * v.Y + m.M34 * v.Z + m.M44 * v.W;

                //v.X = m.M11 * v.X + m.M12 * v.Y + m.M13 * v.Z + m.M14 * v.W;
                //v.Y = m.M21 * v.X + m.M22 * v.Y + m.M23 * v.Z + m.M24 * v.W;
                //v.Z = m.M31 * v.X + m.M32 * v.Y + m.M33 * v.Z + m.M34 * v.W;
                //v.W = m.M41 * v.X + m.M42 * v.Y + m.M43 * v.Z + m.M44 * v.W;
            }
        }

        public override Matrix GetWorldMatrix()
        {
            return Matrix.Multiply(Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll), Matrix.Translation((Vector3)_position));

        }
    }
}
