// 주문
// 음성을 입력해서 GPT가 반환하는 json을 그대로 가져온 클래스

// init을 쓰기 위한 클래스

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {
    }
}

public record SSpell
{
    public SkillElementType Element { get; init; } // 원소타입
    public SkillTargetType To { get; init; } // 대상
    public float Cost { get; init; } // 비용

    public SkillTargetType? At { get; init; } // 위치 TODO: 이거 타입이 SkillTargetType인지는 의문이 든다
    public float? Damage { get; init; } // 피해량
    public float? Heal { get; init; } // 회복량
    public float? Speed { get; init; } // 속도
    public float? Size { get; init; } // 크기
}

public enum SkillElementType
{
    Fire,
    Ice,
    Earth,
}

public enum SkillTargetType
{
    This,
    Enemy,
    Global
}