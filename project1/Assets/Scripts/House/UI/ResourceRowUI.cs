using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ResourceRowUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] private Image icon;
   [SerializeField] private TMP_Text nameText;
   [SerializeField] private TMP_Text amountText;

   private ResourceDefinition _def;
   public string Id => _def != null ? _def.id : "";

   public void Bind(ResourceDefinition def, int amount)
   {
      _def = def;
      nameText.text = _def.displayName;
      icon.sprite = _def.icon;
      amountText.text = amount.ToString();
   }

   public void SetAmount(int amount)
   {
      amountText.text = amount.ToString();
   }
}
