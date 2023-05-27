using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockDataText : MonoBehaviour
{
   public static BlockDataText Instance;
   
   [SerializeField] TextMeshProUGUI textObject;
   [SerializeField] CanvasGroup textObjectGroup;
   [SerializeField] Button closeButton;

   void Awake()
   {
      Instance = this;
      closeButton.onClick.AddListener(Close);
   }

   public void SetText(string blockDataText)
   {
      textObjectGroup.alpha = 1;
      textObject.text = blockDataText;
   }

   public void Close()
   {
      textObjectGroup.alpha = 0;
      textObject.text = "";
   }
}
