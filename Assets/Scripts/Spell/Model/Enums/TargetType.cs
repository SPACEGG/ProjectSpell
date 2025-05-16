namespace Spell.Model.Enums
{
    public enum TargetType
    {
        Caster,     // 스펠 시전한 플레이어
        Activator,  // 스펠 오브젝트에 닿은 플레이어(또는 오브젝트)
        Global,     // 모든 플레이어
    }
}