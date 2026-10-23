namespace HuixinMotors.Shared.Models;

public class ComboItemDto
{
    public int Value { get; set; }
    public string Text { get; set; }

    public override string ToString() => Text;
}
