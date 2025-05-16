using Spell.Model.Enums;

namespace Common.Models
{
    public interface IElementProvider
    {
        ElementType Element { get; }
    }
}