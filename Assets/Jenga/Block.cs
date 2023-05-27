using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Block : MonoBehaviour
{
    public const float Width = 2.5f;
    public const float Height = 1.5f;
    public const float Length = 7.5f;
    public const float Deformation = 0.05f;
    public const float Weight = 1.05f;

    [SerializeField] Material stone;
    [SerializeField] Material wood;
    [SerializeField] Material glass;
    
    Mesh _mesh;
    Rigidbody _rigidbody;
    StackData _blockData;
    int _mat;

    bool _isBreakable;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Remove()
    {
        if (!_isBreakable) return;
        
        gameObject.SetActive(false);
    }
    public void Setup(StackData data)
    {
        _blockData = data;
        _mat = data.mastery;
        if (_mat == 0)
        {
            GetComponent<MeshRenderer>().sharedMaterial = glass;
            _isBreakable = true;
        }
        else if (_mat == 1)
        {
            GetComponent<MeshRenderer>().sharedMaterial = wood;
        }
        else
        {
            GetComponent<MeshRenderer>().sharedMaterial = stone;
        }
        InitMesh();
    }
    void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude < 0.1f)
        {
            _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        }

        if (_rigidbody.angularVelocity.magnitude < 0.1f)
        {
            _rigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
        }
    }
    
    Vector3 DeformRandomly(Vector3 point)
    {
        return new Vector3(
            Random.Range(point.x - Deformation, point.x + Deformation),
            Random.Range(point.y - Deformation, point.y + Deformation),
            Random.Range(point.z - Deformation, point.z + Deformation)
        );
    }

    void InitMesh()
    {
        if (_mesh != null)
        {
            return;
        }

        _mesh = new Mesh();
        _mesh.name = "Jenga block";

        var vertices = new Vector3[] {
            DeformRandomly(new Vector3(0f, 0f, 0f)),
            DeformRandomly(new Vector3(0f, Height, 0f)),
            DeformRandomly(new Vector3(Width, Height, 0f)),
            DeformRandomly(new Vector3(Width, 0f, 0f)),
            DeformRandomly(new Vector3(0f, 0f, Length)),
            DeformRandomly(new Vector3(0f, Height, Length)),
            DeformRandomly(new Vector3(Width, Height, Length)),
            DeformRandomly(new Vector3(Width, 0f, Length)),
        };

        var triangles = new int[] {
            0, 1, 2,
            2, 3, 0,

            7, 6, 5,
            5, 4, 7,

            1, 5, 6,
            6, 2, 1,

            4, 0, 3,
            3 ,7, 4,

            0, 4, 5,
            5, 1, 0,

            2, 6, 7,
            7, 3, 2,
        };

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();

        var uvs = new Vector2[vertices.Length];

        for (var i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / _mesh.bounds.size.x, vertices[i].z / _mesh.bounds.size.z);
        }
        _mesh.uv = uvs;

        GetComponent<MeshFilter>().mesh = _mesh;

        var collider = GetComponent<MeshCollider>();

        collider.convex = true;
        collider.sharedMesh = _mesh;

        var body = GetComponent<Rigidbody>();

        body.mass = Weight;
    }

    void OnMouseDown()
    {
        string dataStructured =
            $"{_blockData.grade}:{_blockData.domain}\n{_blockData.cluster}\n{_blockData.standardid}:{_blockData.standarddescription}";
        BlockDataText.Instance.SetText(dataStructured);
    }
}
