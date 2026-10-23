using System.Collections.Generic;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.PL;

public enum FieldType
{
    Text,
    Boolean,
    Combo,
    StaticCombo
}

public class FieldDef
{
    public string Column { get; private set; }
    public string Label { get; private set; }
    public bool Required { get; private set; }
    public FieldType Type { get; private set; }
    public List<ComboItemDto> ComboItems { get; private set; }
    public List<string> StaticItems { get; private set; }

    public FieldDef(
        string column, 
        string label, 
        bool required, 
        FieldType type = FieldType.Text, 
        List<ComboItemDto> comboItems = null, 
        List<string> staticItems = null)
    {
        Column = column;
        Label = label;
        Required = required;
        Type = type;
        ComboItems = comboItems;
        StaticItems = staticItems;
    }
}
