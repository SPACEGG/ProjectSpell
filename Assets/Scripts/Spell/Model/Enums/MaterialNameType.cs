namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 머티리얼 이름 Enum.
    /// Resources/VFX/Materials 폴더 내 실제 머티리얼 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum MaterialNameType
    {
        None,                       // 머티리얼 없음 또는 기본값
        Glow_Fire,                  // 불꽃 계열의 빛나는 머티리얼
        Glow_Ice,                   // 얼음 계열의 빛나는 머티리얼
        Gold,                       // 금속성 골드 머티리얼
        Ground,                     // 일반 땅/지면 머티리얼
        IceMaterial,                // 얼음 표면 느낌의 머티리얼
        M_Stones,                   // 돌 표면 머티리얼(버전1, 구버전)
        M_StonesV1,                 // 돌 표면 머티리얼(버전1, 신버전)
        PT_Ground_Grass_Green_01    // 초록 잔디 느낌의 지면 머티리얼
    }
}
