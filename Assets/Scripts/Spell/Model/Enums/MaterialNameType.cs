namespace Spell.Model.Enums
{
    /// <summary>
    /// 주문에 사용할 수 있는 머티리얼 이름 Enum.
    /// Resources/VFX/Materials 폴더 내 실제 머티리얼 파일명(확장자 제외)과 일치해야 함.
    /// </summary>
    public enum MaterialNameType
    {
        None,           // 머티리얼 없음 또는 기본값
        CrystalFree1,   // 푸른빛의 수정(크리스탈) 머티리얼
        Glow_Fire,      // 밝은 주황색 빛나는 머티리얼(불꽃/광휘)
        Glow_Ice,       // 밝은 푸른빛 빛나는 머티리얼(얼음/마법)
        Gold,           // 금속성 골드 머티리얼(노란빛)
        Ground,         // 어두운 회색/검정 땅 머티리얼
        IceMaterial     // 밝은 회청색 얼음 표면 머티리얼
    }
}
