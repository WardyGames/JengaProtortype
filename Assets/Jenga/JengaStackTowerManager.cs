using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JengaStackTowerManager : MonoBehaviour
{
    [SerializeField] Transform blockPrefab;
    [SerializeField] Transform gradeTextPrefab;

    int _stackIndex;
    List<JengaStack> _jengaStacks = new();
    int _currentJengaStackIndex;

    void OnEnable()
    {
        ScreenHUD.NextJengaStack += OnNextJengaStack;
        ScreenHUD.PreviousJengaStack += OnPreviousJengaStack;
        ScreenHUD.TestJengaStack += OnTestJengaStack;
    }

    void OnDisable()
    {
        ScreenHUD.NextJengaStack -= OnNextJengaStack;
        ScreenHUD.PreviousJengaStack -= OnPreviousJengaStack;
        ScreenHUD.TestJengaStack -= OnTestJengaStack;
    }
    public void Init(List<StackData> stackDataList)
    {
        JengaStack newJengaStack = new JengaStack();
        int numberOfBlocks = stackDataList.Count;
        newJengaStack.stackObject = new GameObject("New Jenga Tower");
        newJengaStack.stackObject.transform.position = new Vector3(_stackIndex * 25, 0, 0);
        _jengaStacks.Add(newJengaStack);
        //int height = numberOfBlocks / 3;
        int layerSwitchCounter = 0;
        int currentLayer = 0;
        bool isVertical = true;
        for (int i = 0; i < numberOfBlocks; i++)
        {
            if (isVertical)
            {
                BuildVerticalLayer(currentLayer, layerSwitchCounter);
            }
            else
            {
                BuildHorizontalLayer(currentLayer, layerSwitchCounter);
            }

            layerSwitchCounter++;
            if (layerSwitchCounter >= 3)
            {
                layerSwitchCounter = 0;
                isVertical = !isVertical;
                currentLayer++;
            }
            newJengaStack.blocks[i].gameObject.transform.SetParent(newJengaStack.stackObject.transform, false);
            newJengaStack.blocks[i].gameObject.transform.GetComponent<Block>().Setup(stackDataList[i]);
        }
        var go = Instantiate(gradeTextPrefab);
        go.GetComponent<TextMeshPro>().text = stackDataList[0].grade;
        go.transform.position = new Vector3(4 + (25*_stackIndex), 2, -5);
        _stackIndex++;
        
        StartCoroutine(Stabilize());
    }

    IEnumerator Stabilize()
    {
        var wait = new WaitForSeconds(0.1f);

        foreach (var jengaStack in _jengaStacks)
        {
            foreach (var block in jengaStack.blocks)
            {
                var body = block.GetComponent<Rigidbody>();

                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                yield return wait;
            }
        }
    }

    
    void BuildVerticalLayer(int layerIndex, int blockCurrent)
    {
        var y = (Block.Height + Block.Deformation) * layerIndex;

        var block = Instantiate(blockPrefab);
        block.localPosition = new Vector3(blockCurrent*(Block.Width+ Block.Deformation), y, 0f);
        _jengaStacks[_stackIndex].blocks.Add(block.GetComponent<Block>());
    }

    void BuildHorizontalLayer(int layerIndex, int blockCurrent)
    {
        var rotation = Quaternion.Euler(0f, 90f, 0f);
        var y = (Block.Height + Block.Deformation) * layerIndex;

        var block = Instantiate(blockPrefab);
        block.localRotation = rotation;
        block.localPosition = new Vector3(0, y, (Block.Width+ Block.Deformation)*(blockCurrent+1));
        _jengaStacks[_stackIndex].blocks.Add(block.GetComponent<Block>());
    }

    void OnNextJengaStack()
    {
        _currentJengaStackIndex++;
        if (_currentJengaStackIndex >= _jengaStacks.Count)
            _currentJengaStackIndex = 0;
        CameraController.Instance.GoToNewStackTower(_jengaStacks[_currentJengaStackIndex].stackObject.transform);
    }
    
    void OnPreviousJengaStack()
    {
        _currentJengaStackIndex--;
        if (_currentJengaStackIndex < 0)
            _currentJengaStackIndex = _jengaStacks.Count-1;
        CameraController.Instance.GoToNewStackTower(_jengaStacks[_currentJengaStackIndex].stackObject.transform);
    }
    
    void OnTestJengaStack()
    {
        foreach (var block in _jengaStacks[_currentJengaStackIndex].blocks)
        {
            block.GetComponent<Block>().Remove();
        }
    }
}

[Serializable]
public class JengaStack
{
    public GameObject stackObject;
    public List<Block> blocks = new ();
}
